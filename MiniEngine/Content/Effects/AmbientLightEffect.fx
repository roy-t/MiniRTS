#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

// Bias to prevent shadow acne
static const float bias = 0.0001f;

static const int KERNEL_SIZE = 64;

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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = float4(input.Position,1);       
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
        // Generate a random position near the original position        
        float4 sampleWorld = float4(position.xyz + Kernel[i], 1.0f);

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

float4 WorldPositionPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;
    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);
    float depth = ReadDepth(texCoord) / 3;

    float d = 0.0f;

    for (int i = 0; i < KERNEL_SIZE; i++)
    {
        // Generate a random position near the original position        
        float4 sampleWorld = float4(position.xyz + Kernel[i], 1.0f);

        // Figure out the corresponding depth        
        float4 sampleProjection = mul(mul(sampleWorld, View), Projection) / 50.0f;
        float2 sampleTex = ToTextureCoordinates(sampleProjection.xy, sampleProjection.w);
        float sampleDepth = ReadDepth(sampleTex.xy);

        d += sampleDepth / 3.0f;
    }

    d /= (KERNEL_SIZE);

    return float4(depth, depth, depth, 0.0f);
}

technique AmbientLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }    
}