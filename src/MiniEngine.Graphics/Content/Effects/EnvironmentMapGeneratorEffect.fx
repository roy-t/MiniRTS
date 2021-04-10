#include "Includes/Defines.hlsl"
#include "Includes/Coordinates.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/BRDF.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float3 Position3D : TEXCOORD0;
};

struct OutputData
{
    float4 Color : COLOR0;
};

Texture2D EquirectangularTexture;
float Roughness;
float4x4 WorldViewProjection;

const sampler equirectangularTextureSampler = sampler_state
{
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Position3D = input.Position;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    float3 N = normalize(input.Position3D);
    float3 R = N;
    float3 V = R;

    const uint SAMPLE_COUNT = 1024u;
    float totalWeight = 0.0f;
    float3 prefilteredColor = float3(0, 0, 0);

    for (uint i = 0u; i < SAMPLE_COUNT; i++)
    {
        float2 Xi = Hammersley(i, SAMPLE_COUNT);
        float3 H = ImportanceSampleGGX(Xi, N, Roughness);
        float3 L = normalize(2.0f * dot(V, H) * H - V);

        float NdotL = dot(N, L);
        if (NdotL > 0.0f)
        {
            float2 uv = WorldToSpherical(L);
            float4 color = EquirectangularTexture.Sample(equirectangularTextureSampler, uv) * NdotL;
            prefilteredColor += ToLinear(color).rgb;
            totalWeight += NdotL;
        }
    }
    prefilteredColor = prefilteredColor / totalWeight;

    output.Color = float4(prefilteredColor, 1.0f);
    return output;
}

technique EnvironmentMapGeneratorTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
