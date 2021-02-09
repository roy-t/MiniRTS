﻿#include "Includes/Defines.hlsl"
#include "Includes/Instancing.hlsl"
#include "Includes/Gamma.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float2 Texture : TEXCOORD0;
    float4 Tint : TEXCOORD1;
};

struct OutputData
{
    // COLOR0 is only set so we can sample its depth buffer
    float4 Color : COLOR1;
};

texture Texture;
sampler textureSampler = sampler_state
{
    Texture = (Texture);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 WorldViewProjection;

PixelData VS_INSTANCED(in VertexData input, in ParticleInstancingData instance)
{
    PixelData output = (PixelData)0;

    float4x4 offsetT = transpose(instance.Offset);

    output.Position = mul(mul(float4(input.Position, 1), offsetT), WorldViewProjection);
    output.Texture = input.Texture;
    output.Tint = instance.Color;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    output.Color = ToLinear(tex2D(textureSampler, input.Texture)) * ToLinear(input.Tint);

    return output;
}

technique ParticleTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_INSTANCED();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
