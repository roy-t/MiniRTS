#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"

float3 WorldPosition;
float3 CameraPosition;

float4 VisibleTint;
float4 ClippedTint;

float4 Color;

struct VertexShaderInput
{
    float3 Position : POSITION0;    
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;  
    float4 UVPosition: TEXCOORD0;
};


VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);        
    output.UVPosition = output.Position;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{    
    return Color;
}

float4 DepthPS(VertexShaderOutput input) : COLOR0
{
    float2 sampleCoord = ToTextureCoordinates(input.UVPosition.xy, input.UVPosition.w);
    float4 pixelWorldPosition = ReadWorldPosition(sampleCoord, InverseViewProjection);

    float4 position = mul(input.UVPosition, InverseViewProjection);
    position /= position.w;

    float pixelDistance = distance(CameraPosition, pixelWorldPosition.xyz);
    float worldDistance = distance(CameraPosition, position.xyz);

    if (worldDistance > pixelDistance)
    {
        return Color * ClippedTint;
    }

    return Color * VisibleTint;
}


technique ColorEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}

technique ColorEffectWithDepthTest
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL DepthPS();
    }
}