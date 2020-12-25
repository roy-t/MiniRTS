#include "Includes/Defines.hlsl"
#include "Includes/GBufferReader.hlsl"

static const float BlendThreshold = 0.3f;
static const float bias = 0.005f; // Bias to prevent shadow acne
static const uint NumCascades = 4;

texture Light;
sampler lightSampler = sampler_state
{
    Texture = (Light);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Volume;
sampler volumeSampler = sampler_state
{
    Texture = (Volume);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 InverseViewProjection;
float3 CameraPosition;
float3 FogColor;
float Strength;


Texture2DArray ShadowMap : register(t0);
SamplerComparisonState ShadowSampler : register(s0);
float4x4 ShadowViewProjection;

float4x4 ShadowMatrix;
float Splits[NumCascades];
float4 Offsets[NumCascades];
float4 Scales[NumCascades];

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
    float4 Color : COLOR0;
};

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

    float4 color = tex2D(lightSampler, input.Texture).rgba;
        float2 fb = tex2D(volumeSampler, input.Texture).xy;
    if (fb.x >= 1.0f && fb.y >= 1.0f)
    {
        output.Color = color;
        return output;
    }

    float3 world = ReadWorldPosition(input.Texture, InverseViewProjection);
    float3 volumeFront = ReadWorldPosition(input.Texture, fb.x, InverseViewProjection);
    float3 volumeBack = ReadWorldPosition(input.Texture, fb.y, InverseViewProjection);

    float dWorld = distance(world, CameraPosition);
    float dFront = distance(volumeFront, CameraPosition);
    float dBack = distance(volumeBack, CameraPosition);

    
    float3 c = color.rgb;        

    float dMax = 100.0f;// max(dWorld, max(dFront, dBack));
    if (dWorld > dFront)
    {
        float dInside = 0;
        if (dWorld < dBack)
        {                       
            dInside = dWorld - dFront;            
        }
        else
        {
            dInside = dBack - dFront;
        }

        c = lerp(c, FogColor, clamp(dInside * Strength / dMax, 0.0f, 1.0f));        
    }

    // Compute fog shadows
    float lightness = 1.0f;
    if (dWorld > dFront)
    {                
        lightness = 0.0f;
        const uint steps = 20;        
        float3 startPosition = dWorld < dBack ? world : volumeBack;
        float3 surfaceToLight = normalize(CameraPosition - startPosition);
        float totalDistance = distance(startPosition, volumeFront);
        float step = totalDistance / steps;

        [unroll]
        for (uint i = 0; i < steps; i++)
        {
            float3 worldPosition = startPosition + (surfaceToLight * step * i);
            float depth = distance(worldPosition, CameraPosition);
            lightness += ComputeLightFactor(worldPosition, depth);
        }

        lightness /= steps;
    }

    output.Color = float4(c * lightness, color.a);    
    return output;
}

technique LightPostProcessTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
