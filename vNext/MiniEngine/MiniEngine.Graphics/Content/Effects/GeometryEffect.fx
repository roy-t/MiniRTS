#include "Includes/Defines.hlsl"
#include "Includes/Pack.hlsl"
#include "Includes/Gamma.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float3 Normal : NORMAL0;    
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};

struct PixelData
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float2 Depth: TEXCOORD1;
    float3 Normal : NORMAL0;    
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;    
};

struct OutputData
{
    float4 Diffuse : COLOR0;    
    float Depth : COLOR1;
    float4 Normal: COLOR2;
};

texture Diffuse;
sampler diffuseSampler = sampler_state
{
    Texture = (Diffuse);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Normal;
sampler normalSampler = sampler_state
{
    Texture = (Normal);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 World;
float4x4 WorldViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Texture = input.Texture;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    float3x3 rotation = (float3x3)World;

    output.Normal = mul(input.Normal, rotation);
    output.Binormal = mul(input.Tangent, rotation);
    output.Tangent = mul(input.Binormal, rotation);
    
    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    float4 diffuse = tex2D(diffuseSampler, input.Texture);
    output.Diffuse = ToLinear(diffuse);
    output.Depth = input.Depth.x / input.Depth.y;
    
    float3x3 tbn = float3x3(input.Tangent, input.Binormal, input.Normal);
    float3 normal = UnpackNormal(tex2D(normalSampler, input.Texture).xyz);
    normal = mul(normal, tbn);
    output.Normal = float4(PackNormal(normal), 1.0f);
        
    return output;
}

technique GeometryTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}