#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

// Bias to prevent shadow acne
static const float bias = 0.0001f;
static const float Pi = 3.1415926535f;
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

texture NoiseMap;
sampler noiseSampler = sampler_state
{
    Texture = (NoiseMap);
    AddressU = WRAP;
    AddressV = WRAP;
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
    float ambientLight = 0.0f;
    for (int i = 0; i < KERNEL_SIZE; i++)
    {
        // Rotate the kernel using the pre-generated noise, seeded by the world position
        // of the object
        int ix = (int)(position.x * 73856093);
        int iy = (int)(position.y * 50000059);
        int iz = (int)(position.z * 83492791);
        float2 uv = (ix ^ iy ^ iz) * 0.0001f;         
        
        float3 noise = tex2D(noiseSampler, uv).rgb * (Pi / 2.0f);
        
        float3 offset;
        offset.x = (cos(noise.x) - sin(noise.x)) * Kernel[i].x;
        offset.y = (sin(noise.y) + cos(noise.y)) * Kernel[i].y;
        offset.z = (cos(noise.x) - sin(noise.z)) * Kernel[i].z;

        // Generate a random position near the original position        
        float4 sampleWorld = float4(position.xyz + offset, 1.0f);

        // Transform to view space
        float4 sampleView = mul(mul(sampleWorld, View), Projection);
                
        // Check if the random point is occluded or not
        float depth = sampleView.z / sampleView.w;
        float2 sampleTex = ToTextureCoordinates(sampleView.xy, sampleView.w);
        ambientLight += ShadowMap.SampleCmpLevelZero(ShadowSampler, sampleTex, depth);
    }

    ambientLight /= KERNEL_SIZE;
    ambientLight = pow(ambientLight, Strength);
    return float4(Color.rgb * ambientLight, 0.0f);
}

// Inspired by: http://ogldev.atspace.co.uk/www/tutorial45/tutorial45.html
float4 OldPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;
    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);
    float depth = ReadDepth(texCoord) - bias;

    float ambientLight = 0.0f;
    float sum = 0.0f;
    for (int i = 0; i < KERNEL_SIZE; i++)
    {
        float2 noiseTex = float2(position.x, position.y * 0.5f + position.z * 0.5f) * Pi * Pi * Pi * Pi * Pi * Pi * Pi;
        float3 noise = tex2D(noiseSampler, noiseTex).rgb * (Pi / 2.0f);
        float3 offset;
        offset.x = (cos(noise.x) - sin(noise.x)) * Kernel[i].x;
        offset.y = (sin(noise.y) + cos(noise.y)) * Kernel[i].y;
        offset.z = (cos(noise.x) - sin(noise.z)) * Kernel[i].z;


        // Generate a random position near the original position        
        float4 sampleWorld = float4(position.xyz + offset, 1.0f);

        // Transform to view space
        float4 sampleView = mul(mul(sampleWorld, View), Projection);

        // Transform to texture coordinates
        float2 sampleTex = ToTextureCoordinates(sampleView.xy, sampleView.w);
        /* if (sampleTex.x >= 0.0f && sampleTex.x <= 1.0f &&
             sampleTex.y >= 0.0f && sampleTex.y <= 1.0f)*/
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


    pass OldPass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL OldPS();
    }
}