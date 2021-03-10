#include "Includes/Defines.hlsl"
#include "Includes/Pack.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/Instancing.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float4 WorldPosition: TEXCOORD0;
};

struct OutputData
{
    float Depth : COLOR0;
};

float4x4 WorldViewProjection;

Texture2D Data;
sampler dataSampler = sampler_state
{
    Texture = (Data);
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

PixelData VS_INSTANCED(in VertexData input, in Particle particle)
{
    PixelData output = (PixelData)0;

    float4 data = Data.SampleLevel(dataSampler, particle.UV, 0);

    float4x4 world =
    {
        1.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        data.x, data.y, data.z, 1.0f
    };

    output.Position = mul(mul(float4(input.Position, 1), world), WorldViewProjection);
    output.WorldPosition = output.Position;
    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    output.Depth = input.WorldPosition.z / input.WorldPosition.w;
    return output;
}

technique ParticleShadowMapTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_INSTANCED();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
