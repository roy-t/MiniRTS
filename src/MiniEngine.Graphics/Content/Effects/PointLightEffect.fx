#include "Includes/Defines.hlsl"
#include "Includes/GBufferReader.hlsl"
#include "Includes/Coordinates.hlsl"
#include "Includes/Lights.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : POSITION0;
    noperspective float2 Texture : TEXCOORD0;
};

struct OutputData
{
    float4 Light : COLOR0;
};

float4x4 WorldViewProjection;
float4 Color;
float Strength;
float3 Position;
float3 CameraPosition;
float4x4 InverseViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Texture = ScreenToTexture(output.Position.xy / output.Position.w);

    return output;
}

// Inspired by https://learnopengl.com/PBR/ and http://www.codinglabs.net/article_physically_based_rendering_cook_torrance.aspx

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    // Read data from G-Buffer
    float3 albedo = ReadAlbedo(input.Texture);
    float3 N = ReadNormal(input.Texture);
    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    Mat material = ReadMaterial(input.Texture);

    float3 Lo = ComputeLight(albedo, N, material, worldPosition, CameraPosition, Position, Color, Strength);
    output.Light = float4(Lo, 1.0f);

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
