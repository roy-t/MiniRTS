#include "Includes/Defines.hlsl"
#include "Includes/GBufferReader.hlsl"
#include "Includes/Coordinates.hlsl"
#include "Includes/Lights.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct PixelData
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct OutputData
{
    float4 Light : COLOR0;
};

static const float BlendThreshold = 0.3f;
static const float bias = 0.005f; // Bias to prevent shadow acne
static const uint NumCascades = 4;

float4 Color;
float Strength;
float3 SurfaceToLight;
float3 CameraPosition;
float4x4 InverseViewProjection;

float4x4 ShadowMatrix;
float Splits[NumCascades];
float4 Offsets[NumCascades];
float4 Scales[NumCascades];

Texture2DArray ShadowMap : register(t0);
SamplerComparisonState ShadowSampler : register(s0);

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1);
    output.Texture = input.Texture;
    return output;
}

float SampleShadowMap(float2 baseUv, float u, float v, float2 shadowMapSizeInv, uint cascadeIndex, float z)
{
    float2 uv = baseUv + float2(u, v) * shadowMapSizeInv;
    return ShadowMap.SampleCmpLevelZero(ShadowSampler, float3(uv, cascadeIndex), z);
}

float SampleShadowMapPCF(float3 shadowPosition, uint cascadeIndex)
{
    float2 shadowMapSize;
    float _;
    ShadowMap.GetDimensions(shadowMapSize.x, shadowMapSize.y, _);

    float lightDepth = shadowPosition.z - bias;

    float2 uv = shadowPosition.xy * shadowMapSize;
    float2 shadowMapSizeInv = 1.0f / shadowMapSize;

    float2 baseUv;
    baseUv.x = floor(uv.x + 0.5f);
    baseUv.y = floor(uv.y + 0.5f);

    float s = (uv.x + 0.5f - baseUv.x);
    float t = (uv.y + 0.5f - baseUv.y);

    baseUv -= float2(0.5f, 0.5f);
    baseUv *= shadowMapSizeInv;

    float sum = 0.0f;

    float uw0 = (4 - 3 * s);
    float uw1 = 7;
    float uw2 = (1 + 3 * s);

    float u0 = (3 - 2 * s) / uw0 - 2;
    float u1 = (3 + s) / uw1;
    float u2 = s / uw2 + 2;

    float vw0 = (4 - 3 * t);
    float vw1 = 7;
    float vw2 = (1 + 3 * t);

    float v0 = (3 - 2 * t) / vw0 - 2;
    float v1 = (3 + t) / vw1;
    float v2 = t / vw2 + 2;

    sum += uw0 * vw0 * SampleShadowMap(baseUv, u0, v0, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw1 * vw0 * SampleShadowMap(baseUv, u1, v0, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw2 * vw0 * SampleShadowMap(baseUv, u2, v0, shadowMapSizeInv, cascadeIndex, lightDepth);

    sum += uw0 * vw1 * SampleShadowMap(baseUv, u0, v1, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw1 * vw1 * SampleShadowMap(baseUv, u1, v1, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw2 * vw1 * SampleShadowMap(baseUv, u2, v1, shadowMapSizeInv, cascadeIndex, lightDepth);

    sum += uw0 * vw2 * SampleShadowMap(baseUv, u0, v2, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw1 * vw2 * SampleShadowMap(baseUv, u1, v2, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw2 * vw2 * SampleShadowMap(baseUv, u2, v2, shadowMapSizeInv, cascadeIndex, lightDepth);

    return sum * 1.0f / 144;
}

float SampleShadowCascade(float3 shadowPosition, uint cascadeIndex)
{
    shadowPosition += Offsets[cascadeIndex].xyz;
    shadowPosition *= Scales[cascadeIndex].xyz;
   
    return SampleShadowMapPCF(shadowPosition, cascadeIndex);
}

uint GetCascadeIndex(float depth)
{
    uint cascadeIndex = 0;
    [unroll]
    for (uint i = 0; i < NumCascades - 1; ++i)
    {
        [flatten]
        if (depth > Splits[i])
        {
            cascadeIndex = i + 1;
        }
    }

    return cascadeIndex;
}

float ComputeLightFactor(float3 worldPosition, float depth)
{
    float3 position = mul(float4(worldPosition, 1.0f), ShadowMatrix).xyz;

    uint cascadeIndex = GetCascadeIndex(depth);

    float shadowVisibility = SampleShadowCascade(position, cascadeIndex);

    float nextSplit = Splits[cascadeIndex];
    float splitSize = cascadeIndex == 0 ? nextSplit : nextSplit - Splits[cascadeIndex - 1];
    float splitDist = (nextSplit - depth) / splitSize;

    [branch]
    if (splitDist <= BlendThreshold && cascadeIndex != NumCascades - 1)
    {
        float nextSplitVisibility = SampleShadowCascade(position, cascadeIndex + 1);
        float lerpAmt = smoothstep(0.0f, BlendThreshold, splitDist);
        shadowVisibility = lerp(nextSplitVisibility, shadowVisibility, lerpAmt);
    }

    return shadowVisibility;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    // Read data from G-Buffer
    float3 albedo = ReadAlbedo(input.Texture);
    float3 N = ReadNormal(input.Texture);
    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    float depth = distance(worldPosition, CameraPosition);

    Mat material = ReadMaterial(input.Texture);

    float lightFactor = ComputeLightFactor(worldPosition, depth);
    float3 Lo = float3(0.0f, 0.0f, 0.0f);
    if (lightFactor > 0)
    {
        Lo = ComputeLight(albedo, N, material, worldPosition, CameraPosition, SurfaceToLight, Color, Strength);
        // Ignore attenuation since sunlight has already crossed an extreme distance
    }

    output.Light = float4(Lo, 1.0f) * lightFactor;
    return output;
}



OutputData DEBUG_PS(PixelData input)
{
    OutputData output = (OutputData)0;

    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    float depth = distance(worldPosition, CameraPosition);
    
    uint cascadeIndex = GetCascadeIndex(depth);
    const float3 CascadeColors[NumCascades] =
    {
        float3(1.0f, 0.0f, 0.0f),
        float3(0.0f, 1.0f, 0.0f),
        float3(0.0f, 0.0f, 1.0f),
        float3(1.0f, 1.0f, 0.0f)
    };
    output.Light = float4(CascadeColors[cascadeIndex], 1.0f);
    return output;
}

technique SunlightTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

technique DebugTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL DEBUG_PS();
    }
}
