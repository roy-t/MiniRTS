#include "Includes/Defines.hlsl"
#include "Includes/Instancing.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/Shadows.hlsl"

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
    float2 Depth : TEXCOORD1;
    float LightFactor : TEXCOORD2;
    float4 Tint : TEXCOORD3;
};

struct OutputData
{
    // COLOR0 is only set so we can sample its depth buffer
    float4 Color : COLOR1;
    float Weight : COLOR2;
};

texture Texture;
sampler textureSampler = sampler_state
{
    Texture = (Texture);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 WorldViewProjection;
float3 CameraPosition;

PixelData VS_INSTANCED(in VertexData input, in ParticleInstancingData instance)
{
    PixelData output = (PixelData)0;    

    float4x4 offsetT = transpose(instance.Offset);
    float4 worldPosition = mul(float4(input.Position, 1), offsetT);

    float depth = distance(worldPosition.xyz, CameraPosition);
    float lightFactor = ComputeLightFactor(worldPosition.xyz, depth);  

    output.Position = mul(worldPosition, WorldViewProjection);
    output.Texture = input.Texture;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;
    output.LightFactor = lightFactor;
    output.Tint = instance.Color;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    float4 premultipliedReflect = ToLinear(tex2D(textureSampler, input.Texture)) * ToLinear(input.Tint);
    premultipliedReflect.rgb *= input.LightFactor;

    float a = min(1.0, premultipliedReflect.a) * 8.0 + 0.01;
    float depth = input.Depth.x / input.Depth.y;
    float b = -depth * 0.95 + 1.0;
    float w = clamp(a * a * a * 1e8 * b * b * b, 1e-2, 3e2);

    output.Color = premultipliedReflect * w;
    output.Weight = premultipliedReflect.a;
    return output;
}

technique WeightedTransparencyTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_INSTANCED();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
