#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"

// Inspired by http://casual-effects.blogspot.com/2015/03/implemented-weighted-blended-order.html

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
};

struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Weight : COLOR1;
};

texture Texture;
sampler textureSampler = sampler_state
{
    Texture = (Texture);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

float4 Tint;

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    return output;
}

PixelShaderOutput MainPS(VertexShaderOutput input)
{   
    PixelShaderOutput output = (PixelShaderOutput)0;

    float2 texCoord = input.TexCoord;
    float depth = input.Depth.x / input.Depth.y;

    float4 color = tex2D(textureSampler, texCoord) * Tint;    
    
    float w = clamp(pow(1.0f + 0.01f, 3.0) * 1e8 * pow(1.0f - depth * 0.9f, 3.0f), 1e-2, 3e3);

    output.Color = float4(color.rgb * color.a, color.a) * w;
    output.Weight = float4(color.a, 0, 0, 1.0f);    

    return output;
}

technique WeightedParticlesTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}