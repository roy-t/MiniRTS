#include "Includes/Defines.hlsl"
#include "Includes/Gamma.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float4 Color : COLOR0;
};

struct PixelData
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float4 Color : COLOR0;
};

struct OutputData
{
    float4 Color : COLOR0;
};

Texture2D Color;
SamplerState ColorSampler : register(s0);

float4x4 WorldViewProjection;
bool ConvertColorsToLinear = false;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Texture = input.Texture;
    output.Color = input.Color;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    float4 color =  Color.Sample(ColorSampler, input.Texture) * input.Color;
    float4 colorLinear = ToLinear(color);
    output.Color = lerp(color, colorLinear, ConvertColorsToLinear);

    return output;
}

technique ImmediateTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
