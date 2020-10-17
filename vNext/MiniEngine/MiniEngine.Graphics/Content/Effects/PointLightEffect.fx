#include "Includes/Defines.hlsl"
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

float3 Color;
float Strength;
float3 Position;
float4x4 InverseViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1.0f);
    output.Texture = input.Texture;
    
    return output;
}


OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    
    float3 normal = ReadNormal(input.Texture);
    Mat material = ReadMaterial(input.Texture);
    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);

    float3 light = Color * Strength;
    float attenuation = CalculateAttenuation(Position, worldPosition);
    float scale = ScaleLight(Position, worldPosition, normal);    
    float3 radiance = CalculateRadiance(light, attenuation, scale);
                 
    output.Light = float4(radiance, 1.0f);
    
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