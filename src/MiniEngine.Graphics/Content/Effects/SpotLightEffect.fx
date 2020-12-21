#include "Includes/Defines.hlsl"
#include "Includes/GBufferReader.hlsl"
#include "Includes/Coordinates.hlsl"
#include "Includes/Lights.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : POSITION0;
    noperspective float2 Texture : TEXCOORD0;
};

struct OutputData
{
    float4 Light : COLOR0;
};

// Bias to prevent shadow acne
static const float bias = 0.0005f;

float4x4 WorldViewProjection;
float4 Color;
float Strength;
float3 Position;
float3 CameraPosition;
float4x4 InverseViewProjection;
float4x4 ShadowViewProjection;

Texture2D ShadowMap : register(t0);
SamplerComparisonState ShadowSampler : register(s0);

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Texture = ScreenToTexture(output.Position.xy / output.Position.w);
    return output;
}

float SampleShadowMap(float2 baseUv, float u, float v, float2 shadowMapSizeInv, float z)
{
    float2 uv = baseUv + float2(u, v) * shadowMapSizeInv;
    return ShadowMap.SampleCmpLevelZero(ShadowSampler, uv, z);
}

float SampleShadowMapPCF(float2 shadowMapCoordinates, float z)
{
    float2 shadowMapSize;
    ShadowMap.GetDimensions(shadowMapSize.x, shadowMapSize.y);
    float2 uv = shadowMapCoordinates * shadowMapSize;
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

    sum += uw0 * vw0 * SampleShadowMap(baseUv, u0, v0, shadowMapSizeInv, z);
    sum += uw1 * vw0 * SampleShadowMap(baseUv, u1, v0, shadowMapSizeInv, z);
    sum += uw2 * vw0 * SampleShadowMap(baseUv, u2, v0, shadowMapSizeInv, z);

    sum += uw0 * vw1 * SampleShadowMap(baseUv, u0, v1, shadowMapSizeInv, z);
    sum += uw1 * vw1 * SampleShadowMap(baseUv, u1, v1, shadowMapSizeInv, z);
    sum += uw2 * vw1 * SampleShadowMap(baseUv, u2, v1, shadowMapSizeInv, z);

    sum += uw0 * vw2 * SampleShadowMap(baseUv, u0, v2, shadowMapSizeInv, z);
    sum += uw1 * vw2 * SampleShadowMap(baseUv, u1, v2, shadowMapSizeInv, z);
    sum += uw2 * vw2 * SampleShadowMap(baseUv, u2, v2, shadowMapSizeInv, z);
    return sum / 144.0f;
}

float ComputeLightFactor(float3 worldPosition)
{
    float4 position = mul(float4(worldPosition, 1.0f), ShadowViewProjection);
    float2 uv = 0.5f * (float2(position.x / position.w, -position.y / position.w) + 1);

    if (uv.x < 0.0f || uv.x > 1.0f || uv.y < 0.0f || uv.y > 1.0f)
    {
        return 0.0f;
    }

    float distance = (position.z / position.w);
    return SampleShadowMapPCF(uv, distance - bias);
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    // Read data from G-Buffer
    float3 albedo = ReadAlbedo(input.Texture);
    float3 N = ReadNormal(input.Texture);
    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    Mat material = ReadMaterial(input.Texture);

    float lightFactor = ComputeLightFactor(worldPosition);
    float3 Lo = float3(0.0f, 0.0f, 0.0f);
    if (lightFactor > 0)
    {
        float3 L = normalize(Position - worldPosition);
        Lo = ComputeLight(albedo, N, material, worldPosition, CameraPosition, L, Color, Strength);
        Lo *= Attenuation(Position, worldPosition);
    }

    output.Light = float4(Lo, 1.0f) * lightFactor;
    return output;
}

technique SpotLightTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
