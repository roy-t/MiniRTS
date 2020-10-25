#include "Includes/Defines.hlsl"
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

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);    
    output.Position3D = input.Position;
    
    return output;
}

float2 SampleSphericalMap(float3 position)
{    
    float azimuth = atan2(position.x, position.z);
    float zenith = asin(position.y);
    
    float u = azimuth * ONE_OVER_TWO_PI + 0.5f;;
    float v = 1.0f - (zenith * ONE_OVER_PI + 0.5f);

    return float2(u, v);
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;    
    float2 uv = SampleSphericalMap(normalize(input.Position3D));
    float4 diffuse = tex2D(skyboxSampler, uv);
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