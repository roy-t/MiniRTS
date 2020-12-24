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
    float2 Depth : COLOR0;
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
    
    // If we don't have a distance to the front, but have a distance to the back
    // we're inside the medium
    if (f >= 1.0f && b < 1.0f) { f = 0.0f; }


    // TODO: we can run nice density variations here?
    output.Depth.x = f;
    output.Depth.y = b;
    
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
