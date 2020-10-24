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
    float4 Diffuse : COLOR0;    
};

texture Skybox;
sampler skyboxSampler = sampler_state
{
    Texture = (Skybox);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;    
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 WorldViewProjection;


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
    float4 diffuse = tex2D(skyboxSampler, input.Texture);
    float4 diffuseLinear = ToLinear(diffuse);
    output.Diffuse = diffuseLinear;

    return output;
}

technique SkyboxTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}