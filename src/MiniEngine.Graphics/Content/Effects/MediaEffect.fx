#include "Includes/Defines.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float4 WorldPosition: TEXCOORD0;
};

struct OutputData
{
    float4 Depth : COLOR0;
};

float4x4 WorldViewProjection;
float2 Channel;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.WorldPosition = output.Position;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    float depth = input.WorldPosition.z / input.WorldPosition.w;
    output.Depth.xy = Channel * depth;
    output.Depth.a = 1.0f;
    return output;
}

technique VolumeMediaTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

// --------------------

texture VolumeTexture;
sampler volumeTextureSampler = sampler_state
{
    Texture = (VolumeTexture);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct DensityVertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct DensityPixelData
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

DensityPixelData VS_DENSITY(in DensityVertexData input)
{
    DensityPixelData output = (DensityPixelData)0;

    output.Position = float4(input.Position, 1);
    output.Texture = input.Texture;

    return output;
}

float4 PS_DENSITY(DensityPixelData input) : COLOR0
{
    float2 distances = tex2D(volumeTextureSampler, input.Texture).xy;

    // We clear to a black screen if the minimum is 0 we're inside the media
    // if the minimum and maximum are 0 we're not looking at the media
    // in other cases max - min will give us the density of the media
    float ma = max(distances.x, distances.y);
    float mi = min(distances.x, distances.y);
    float density = ma - mi;

    float4 all;
    all.r = density;
    all.gb = 0.0f;
    all.a = 1.0f;
    return all;
}


technique DensityMediaTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_DENSITY();
        PixelShader = compile PS_SHADERMODEL PS_DENSITY();
    }
}
