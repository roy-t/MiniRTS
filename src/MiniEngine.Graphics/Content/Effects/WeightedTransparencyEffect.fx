#include "Includes/Defines.hlsl"
#include "Includes/Instancing.hlsl"
#include "Includes/Gamma.hlsl"

// Inspired by http://casual-effects.blogspot.com/2015/03/implemented-weighted-blended-order.html

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float2 Texture : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float4 ParticlePosition : TEXCOORD2;
    float4 Tint : TEXCOORD3;
};

struct OutputData
{
    // COLOR0 is only set so we can sample its depth buffer
    float4 Color : COLOR1;
    float Weight : COLOR2;
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
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;
    output.ParticlePosition = output.Position;
    output.Tint = instance.Color;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;   

    float4 color = ToLinear(tex2D(textureSampler, input.Texture)) * ToLinear(input.Tint);
    float depth = input.Depth.x / input.Depth.y;
    float w = clamp(pow(1.0f + 0.01f, 3.0) * 1e8 * pow(1.0f - depth * 0.9f, 3.0f), 1e-2, 3e3);

    output.Color = float4(color.rgb * color.a, color.a) * w;
    output.Weight = color.a;
    return output;
}

technique WeightedTransparencyTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_INSTANCED();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
