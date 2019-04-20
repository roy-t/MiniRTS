#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"

float4 Color;

struct VertexShaderInput
{
    float3 Position : POSITION0;    
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;    
};


VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);        

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{    
    return Color;
}

technique ColorEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}