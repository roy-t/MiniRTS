#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

// Bias to prevent shadow acne
static const float bias = 0.0001f;

static const int KERNEL_SIZE = 128;

float SampleRadius = 0.005f;
float Strength = 1.0f;
float3 Color;
float3 Kernel[KERNEL_SIZE];

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

Texture2D ShadowMap : register(t0);
SamplerComparisonState ShadowSampler : register(s0);

texture NoiseMap;
sampler noiseSampler = sampler_state
{
    Texture = (NoiseMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = float4(input.Position, 1);
    output.TexCoord = input.TexCoord;

    return output;
}

// Inspired by: http://ogldev.atspace.co.uk/www/tutorial45/tutorial45.html
float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;
    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);
    float depth = ReadDepth(texCoord) - bias;

    float ambientLight = 0.0f;
    float sum = 0.0f;
    for (int i = 0; i < KERNEL_SIZE; i++)
    {
        float3 noise = tex2D(noiseSampler, Kernel[i].xy + texCoord).rgb * 0.10f;
        // Generate a random position near the original position        
        float4 sampleWorld = float4(position.xyz + Kernel[i] + noise, 1.0f);

        // Transform to view space
        float4 sampleView = mul(mul(sampleWorld, View), Projection);

        // Transform to texture coordinates
        float2 sampleTex = ToTextureCoordinates(sampleView.xy, sampleView.w);
        if (sampleTex.x >= 0.0f && sampleTex.x <= 1.0f &&
            sampleTex.y >= 0.0f && sampleTex.y <= 1.0f)
        {
            sum += 1.0f;
            ambientLight += ShadowMap.SampleCmpLevelZero(ShadowSampler, sampleTex, depth);
        }
    }

    ambientLight /= sum;
    ambientLight = pow(ambientLight, Strength);
    return float4(Color.rgb * ambientLight, 0.0f);
}

technique AmbientLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}