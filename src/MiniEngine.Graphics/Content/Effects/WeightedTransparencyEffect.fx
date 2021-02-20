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
    float4 WorldPosition : TEXCOORD4;
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
    output.WorldPosition = worldPosition;

    float4 world2 = worldPosition / worldPosition.w;
    float depth = distance(CameraPosition, world2.xyz);
    float lightFactor = ComputeLightFactor(world2.xyz, depth);

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

    float depth = input.Depth.x / input.Depth.y;
    float a = min(1.0, premultipliedReflect.a) * 8.0 + 0.01;    
    float b = -depth * 0.95 + 1.0;
    float w = clamp(a * a * a * 1e8 * b * b * b, 1e-2, 3e2);
    
    float lightFactor = ComputeLightFactor(input.WorldPosition.xyz, 300);
    premultipliedReflect.rgb *= lightFactor; // TODO: why doesn't it work well with `input.LightFactor;` ???

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
