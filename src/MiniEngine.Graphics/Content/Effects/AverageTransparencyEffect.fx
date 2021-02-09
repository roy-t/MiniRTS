#include "Includes/Defines.hlsl"
#include "Includes/Instancing.hlsl"
#include "Includes/Gamma.hlsl"

// Inspired by http://casual-effects.blogspot.com/2015/03/implemented-weighted-blended-order.html

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float2 Texture : TEXCOORD0;   
};

struct OutputData
{
    float4 Color : COLOR0;
};

texture Albedo;
sampler albedoSampler = sampler_state
{
    Texture = (Albedo);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Weights;
sampler weightsSampler = sampler_state
{
    Texture = (Weights);
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

    float revealage = tex2D(weightsSampler, input.Texture).r;    
    // clip if revealage >= 1.0f
    clip(0.999f - revealage);

    float4 accum = tex2D(albedoSampler, input.Texture);
    float3 average = accum.rgb / max(accum.a, EPSILON);

    output.Color = float4(average, 1.0f - revealage);

    return output;
}

technique AverageTransparencyTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
