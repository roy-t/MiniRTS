#include "Includes/Defines.hlsl"

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

texture Diffuse;
sampler diffuseSampler = sampler_state
{
    Texture = (Diffuse);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Light;
sampler lightSampler = sampler_state
{
    Texture = (Light);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

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

    float4 diffuse = tex2D(diffuseSampler, input.Texture);
    float4 light = tex2D(lightSampler, input.Texture);
    float4 diffuseLight = float4(light.rgb, 1.0f);
    float4 specularLight = (float4(light.a, light.a, light.a, 0.0f));

    output.Diffuse = saturate(float4(diffuse * diffuseLight + specularLight));

    return output;
}

technique CombineTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}