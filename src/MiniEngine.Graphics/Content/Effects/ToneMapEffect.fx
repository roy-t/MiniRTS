#include "Includes/Defines.hlsl"
#include "Includes/Gamma.hlsl"

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

texture Color;
sampler colorSampler = sampler_state
{
    Texture = (Color);
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

    float3 color = tex2D(colorSampler, input.Texture).rgb;
    color = color / (color + float3(1.0f, 1.0f, 1.0f));

    output.Color = float4(color, 1.0f);
    return output;
}

technique ToneMapTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
