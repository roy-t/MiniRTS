#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"

float Index;

Texture2DArray Texture;
sampler textureSampler = sampler_state
{
    Texture = (Texture);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;   
    float2 TexCoord : TEXCOORD0;    
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;    
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;    
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);   
    output.TexCoord = input.TexCoord;
    output.Color = input.Color;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{    
    /*float3 texCoord = float3(input.TexCoord, Index);
    return tex2D(textureSampler, texCoord) * input.Color;*/

    return Texture.Sample(textureSampler, float3(input.TexCoord, Index), 0) * input.Color;
}


technique UIEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}

