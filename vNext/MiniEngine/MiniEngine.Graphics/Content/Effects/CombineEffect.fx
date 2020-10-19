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

    // TODO: take out the diffuse component? Or do all diffuse parts in the light shader target? That might be more efficient?    

    float4 diffuse = tex2D(diffuseSampler, input.Texture);
    float4 light = tex2D(lightSampler, input.Texture);
    float4 diffuseLight = float4(light.rgb, 1.0f);
    float4 specularLight = (float4(light.a, light.a, light.a, 0.0f));

    
    // This converts the light from HDR to LDR
    float3 color = light.rgb;
    color = color / (color + float3(1.0f, 1.0f, 1.0f));    
   
    // Until we post-process the light is in linear format.
    // Always render to an SRgb render target, DirectX will do the right color correction for us
    float4 colorSRgb = ToGamma(float4(color, 1.0f));
    output.Diffuse = colorSRgb + (diffuse * 0.000001f);    
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