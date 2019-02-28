#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"

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

struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Weight : COLOR1;
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

float SmoothFade(float Input, float ContrastPower)
{
    float Output = 0.5*pow(saturate(2 * ((Input > 0.5) ? 1 - Input : Input)), ContrastPower);
    Output = (Input > 0.5) ? 1 - Output : Output;

    return Output;
}

PixelShaderOutput MainPS(VertexShaderOutput input)
{   
    PixelShaderOutput output = (PixelShaderOutput)0;

    float2 texCoord = input.TexCoord;
    float depth = input.Depth.x / input.Depth.y;

    // TODO: I'm trying to read the depth of the current pixel in the depth buffer
    // by taking our position in view space, converting it to texture coordinates
    // and then using it to sample the depth buffer.
    // But the math seems off, I sample the wrong part of the input texture here!
    float2 texCoord2 = 0.5f * (float2(input.ParticlePosition.x, -input.ParticlePosition.y) + 1);
    float worldDepth = ReadDepth(texCoord2);
    
    float diff = abs(depth - worldDepth);
    //float fade = SmoothFade((worldDepth * depth), 10.0f);    
    float fade = 1.0f; 
    //float fade = saturate((depth * worldDepth) * 1.0f);
    /*if (diff < 0.01)
    {
        fade = 0.0f;
    }    */
    float4 color = (tex2D(textureSampler, texCoord) * fade) * saturate(1.0 + Tint);
    color.r = worldDepth;
    color.gb *= 0.00001f;
    color.a = 1;
    //color.b = worldDepth;
    float w = clamp(pow(1.0f + 0.01f, 3.0) * 1e8 * pow(1.0f - depth * 0.9f, 3.0f), 1e-2, 3e3);

    output.Color = float4(color.rgb * color.a, color.a) * w;
    output.Weight = float4(color.a, 0, 0, 1.0f);    

    return output;
}

technique WeightedParticlesTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}