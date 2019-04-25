#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"

// Inspired by http://casual-effects.blogspot.com/2015/03/implemented-weighted-blended-order.html

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float4 ParticlePosition : TEXCOORD2;
};

texture Texture;
sampler textureSampler = sampler_state
{
    Texture = (Texture);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

float4 Tint;

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;
    output.ParticlePosition = output.Position;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{        
    float depth = input.Depth.x / input.Depth.y;
    float2 screenCoord = ToTextureCoordinates(input.ParticlePosition.xy, input.ParticlePosition.w);
    float worldDepth = ReadDepth(screenCoord);    

    // Manual depth test with the z-buffer
    clip(worldDepth - depth);
    
    float4 particleWorld = ReadWorldPosition(screenCoord, depth, InverseViewProjection);
    float4 depthWorld = ReadWorldPosition(screenCoord, worldDepth, InverseViewProjection);
        
    // Fade out near other objects
    float diff = distance(particleWorld.xyz, depthWorld.xyz);
    float fade = min(diff, 1.0);
    
    float2 texCoord = input.TexCoord;
    return (tex2D(textureSampler, texCoord) * fade) * Tint;        
}

technique WeightedParticlesTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
