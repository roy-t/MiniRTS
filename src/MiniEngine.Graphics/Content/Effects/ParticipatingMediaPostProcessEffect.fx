#include "Includes/Defines.hlsl"

static const float2 DitherDimensions = float2(8, 8);

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
    float4 Color: COLOR0;
};

float3 MediaColor;

float2 ScreenDimensions;

Texture2D Media;
sampler mediaSampler = sampler_state
{
    Texture = (Media);
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D DitherPattern;
sampler ditherPatternSampler = sampler_state
{
    Texture = (DitherPattern);
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1);
    output.Texture = input.Texture;

    return output;
}

// Same pattern as Texture2D.Gather(...) 
// See: https://wojtsterna.blogspot.com/2018/02/directx-11-hlsl-gatherred.html
static const uint SampleCount = 4;
static const float2 SamplePattern[SampleCount] =
{
    float2(0, 1),
    float2(1, 1),
    float2(1, 0),
    float2(0, 0)    
};

float Bilinear(float2 uv)
{
    float2 dimensions;
    Media.GetDimensions(dimensions.x, dimensions.y);
    float2 steps = float2(1.0f / dimensions.x, 1.0f / dimensions.y);

    float4 visibility;    
    [unroll]
    for (uint i = 0; i < SampleCount; i++)
    {
        float2 uva = uv + (steps * SamplePattern[i]);
        visibility[i] = tex2D(mediaSampler, uva).r;
    }
    
    float tl = visibility.w;
    float tr = visibility.z;
    float bl = visibility.x;
    float br = visibility.y;
    
    // TODO: determine weights based on depth differences
    float wHorizontalTop = 0.5f;
    float wHorizontalBottom = 0.5f;
    float wVertical = 0.5f;

    float top = lerp(tl, tr, wHorizontalTop);
    float bottom = lerp(bl, br, wHorizontalBottom);

    return lerp(top, bottom, wVertical);        
}

float Dither(float2 uv)
{    
    float2 ditherCoordinate = uv * ScreenDimensions / DitherDimensions;
    float ditherValue = tex2D(ditherPatternSampler, ditherCoordinate).r;

    return (ditherValue * 0.125f) + 0.9375; 
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    // Compute the visibility using a bilinear upscale
    float visibility = Bilinear(input.Texture);  

    // Subtely dither the upscaled values because we often lack enough colours 
    // to represent a smooth gradient when inside thick participating media.
    float dither = Dither(input.Texture);
    float visibilityDithered = visibility * dither;    

    output.Color = float4(MediaColor * visibilityDithered, visibilityDithered);
    return output;
}

technique ParticipatingMediaPostProcessTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
