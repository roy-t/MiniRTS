#include "Includes/Defines.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct PixelData
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};


float4x4 WorldViewProjection;


PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(mul(input.Position, (float3x3)WorldViewProjection), 1);
    output.Normal = mul(input.Position, (float3x3)WorldViewProjection);

    return output;
}


float4 PS(PixelData input) : COLOR0
{    
    return float4(1, 0, 0, 1);
}

technique GeometryTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}