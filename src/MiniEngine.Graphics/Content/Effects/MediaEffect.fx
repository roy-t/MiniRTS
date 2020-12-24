#include "Includes/Defines.hlsl"

texture VolumeFront;
sampler volumeFrontSampler = sampler_state
{
    Texture = (VolumeFront);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture VolumeBack;
sampler volumeBackSampler = sampler_state
{
    Texture = (VolumeBack);
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

struct DensityOutputData
{
    float Depth : COLOR0;
};

DensityPixelData VS_DENSITY(in DensityVertexData input)
{
    DensityPixelData output = (DensityPixelData)0;

    output.Position = float4(input.Position, 1);
    output.Texture = input.Texture;

    return output;
}

DensityOutputData PS_DENSITY(DensityPixelData input)
{
    DensityOutputData output = (DensityOutputData)0;

    float f = tex2D(volumeFrontSampler, input.Texture).r;
    float b = tex2D(volumeBackSampler, input.Texture).r;

    // 4 scenarios
    // no faces  f:1 b:1
    // - depth should be 0
    // only back faces f:1, b:<1
    // - we're inside the fog, depth should be the distance to the back face
    // only front faces f:<1, b:1
    // - ??
    // back and front faces f:<1, b:<1    
    // - depth should be b - f

    float depth = 0.0f;
    if(f >= 1.0f && b >= 1.0f)
    { 
        depth = 0.0f;
    }

    if (f >= 1.0f && b < 1.0f)
    {
        depth = b;
    }
    
    if (f < 1.0f && b >= 1.0f)
    {
        depth = 1.0f; // WTF scenario
    }

    if (f < 1.0f && b < 1.0f)
    {
        depth = b - f;
    }

    output.Depth = depth;
    return output;
}

technique DensityMediaTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_DENSITY();
        PixelShader = compile PS_SHADERMODEL PS_DENSITY();
    }
}
