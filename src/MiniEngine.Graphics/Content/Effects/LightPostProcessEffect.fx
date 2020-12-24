#include "Includes/Defines.hlsl"
#include "Includes/GBufferReader.hlsl"

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

texture Volume;
sampler volumeSampler = sampler_state
{
    Texture = (Volume);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 InverseViewProjection;
float3 CameraPosition;
float3 FogColor;
float Strength;

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
    float4 Color : COLOR0;
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

    float3 world = ReadWorldPosition(input.Texture, InverseViewProjection);

    float2 fb = tex2D(volumeSampler, input.Texture).xy;
    float3 volumeFront = ReadWorldPosition(input.Texture, fb.x, InverseViewProjection);
    float3 volumeBack = ReadWorldPosition(input.Texture, fb.y, InverseViewProjection);

    float dWorld = distance(world, CameraPosition);
    float dFront = distance(volumeFront, CameraPosition);
    float dBack = distance(volumeBack, CameraPosition);

    float4 color = tex2D(lightSampler, input.Texture).rgba;
    float3 c = color.rgb;    

    float dMax = 100.0f;// max(dWorld, max(dFront, dBack));
    if (dWorld > dFront)
    {
        float dInside = 0;
        if (dWorld < dBack)
        {
            dInside = dWorld - dFront;            
        }
        else
        {
            dInside = dBack - dFront;
        }

        c = lerp(c, FogColor, clamp(dInside * Strength / dMax, 0.0f, 1.0f));
    }

    output.Color = float4(c, color.a);
    return output;
}

technique LightPostProcessTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
