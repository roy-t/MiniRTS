#include "Includes/Defines.hlsl"
#include "Includes/GBufferReader.hlsl"

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
float3 Position;
float4x4 InverseViewProjection;

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
    
    float3 normal = ReadNormal(input.Texture);
    Mat material = ReadMaterial(input.Texture);
    float4 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);

    float dist = distance(worldPosition.xyz, Position);
    float4 color = float4(1 / dist, 0, 0, 1);
    output.Light = color + float4(normal.xyz, material.Metalicness) * 0.0001f;
    
    return output;
}

technique PointLightTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}