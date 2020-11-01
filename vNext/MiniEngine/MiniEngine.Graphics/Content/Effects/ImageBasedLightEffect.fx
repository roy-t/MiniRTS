#include "Includes/Defines.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/GBufferReader.hlsl"
#include "Includes/Lights.hlsl"

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
    float4 Light : COLOR0;    
};

float4 Color;
float Strength;
float3 Position;
float3 CameraPosition;
float4x4 InverseViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1.0f);
    output.Texture = input.Texture;
    
    return output;
}


// Inspired by https://learnopengl.com/PBR/

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;            
    output.Light = float4(0.0f, 0.0f, 0.0f, 1.0f);

    return output;
}

technique ImageBasedLightTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}