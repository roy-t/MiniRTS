#include "Includes/Defines.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Texture : TEXCOORD0;
};

struct PixelData
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Texture : TEXCOORD0;
};

texture Diffuse;
sampler diffuseSampler = sampler_state
{
    Texture = (Diffuse);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 WorldViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Normal = mul(input.Position, (float3x3)WorldViewProjection);
    output.Texture = input.Texture;

    return output;
}

float4 PS(PixelData input) : COLOR0
{
    float4 color = tex2D(diffuseSampler, input.Texture);
    return color;// +float4(input.Texture.x, input.Texture.y, 0.0f, 1.0f);
}

technique GeometryTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}