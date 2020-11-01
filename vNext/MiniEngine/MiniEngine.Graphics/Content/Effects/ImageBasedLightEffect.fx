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

texture Irradiance;
samplerCUBE irradianceSampler = sampler_state
{
    Texture = (Irradiance);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
    AddressW = Clamp;
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
    
    float3 albedo = ReadDiffuse(input.Texture); // Already in linear color space
    float3 N = ReadNormal(input.Texture);        
    Mat material = ReadMaterial(input.Texture);
    float3 irradiance = texCUBE(irradianceSampler, N).rgb; // Already in linear color space

    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    float3 V = normalize(CameraPosition - worldPosition);

    float3 F0 = float3(0.04f, 0.04f, 0.04f);
    F0 = lerp(F0, albedo, material.Metalicness);

    float3 kS = FresnelSchlickRoughness(clamp(dot(N, V), 0.0f, 1.0f), F0, material.Roughness);
    float3 kD = 1.0f - kS;
    
    float3 diffuse = irradiance * albedo;
    float3 ambient = (kD * diffuse) * material.AmbientOcclusion;

    output.Light = float4(ambient, 1.0f);

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