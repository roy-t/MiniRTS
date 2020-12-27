#include "Includes/Defines.hlsl"

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
    float4 Color: COLOR0;
};

float3 MediaColor;

texture Media;
sampler mediaSampler = sampler_state
{
    Texture = (Media);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1);
    output.Texture = input.Texture;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    float visibility = tex2D(mediaSampler, input.Texture).r;
  
    output.Color = float4(MediaColor * visibility, visibility);
    return output;
}

technique ParticipatingMediaPostProcessTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
