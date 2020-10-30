#include "Includes/Defines.hlsl"
#include "Includes/Spherical.hlsl"
#include "Includes/Gamma.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float3 Position3D : TEXCOORD0;
};

struct OutputData
{
    float4 Diffuse : COLOR0;
};

texture EquirectangularTexture;
sampler equirectangularTextureSampler = sampler_state
{
    Texture = (EquirectangularTexture);
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

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);     
    output.Position3D = input.Position;
    
    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;    
    float2 uv = SampleSphericalMap(normalize(input.Position3D));
    float4 diffuse = tex2D(equirectangularTextureSampler, uv);
    float4 diffuseLinear = ToLinear(diffuse);

    output.Diffuse = diffuseLinear;
      
    return output;
}

technique EnvironmentMapGeneratorTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}