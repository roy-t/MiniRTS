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
float3 LightColor;
float LightInfluence;

float2 ScreenDimensions;

Texture2D Media;
sampler mediaSampler = sampler_state
{
    Texture = (Media);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
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

float Bilinear(float2 uv)
{       
    float2 dimensions;
    Media.GetDimensions(dimensions.x, dimensions.y);

    float x = uv.x * dimensions.x;
    float y = uv.y * dimensions.y;

    int px = (int)x;
    int py = (int)y;
        
    int3 uvi = int3(px, py, 0);

    float tl = Media.Load(uvi + int3(0, 0, 0)).r;
    float tr = Media.Load(uvi + int3(1, 0, 0)).r;
    float bl = Media.Load(uvi + int3(0, 1, 0)).r;
    float br = Media.Load(uvi + int3(1, 1, 0)).r;

    float lx = frac(x);
    float ly = frac(y);

    float top = lerp(tl, tr, lx);
    float bottom = lerp(bl, br, lx);

    return lerp(top, bottom, ly);
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

    float visibility = Bilinear(input.Texture);
   
    // Subtely dither the upscaled values because we often lack enough colours 
    // to represent a smooth gradient when inside thick participating media.
    float dither = Dither(input.Texture);
    float visibilityDithered = visibility * dither;
    
    float3 color = lerp(MediaColor, LightColor, visibility * LightInfluence);

    output.Color = float4(color * visibilityDithered, visibilityDithered);
    
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
