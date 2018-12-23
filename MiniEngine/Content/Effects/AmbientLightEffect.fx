#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

static const int KERNEL_SIZE = 64;

float SampleRadius = 10.0f;
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
    float depth = ReadDepth(texCoord);

    float ambientOcclusion = 0.0f;

    for (int i = 0; i < KERNEL_SIZE; i++)
    {
        // Generate a random position near the original position        
        float4 sampleWorld = float4(position.xyz + Kernel[i], 1.0f);
        
        // Figure out the corresponding depth        
        float4 sampleProjection = mul(mul(sampleWorld, View), Projection);
        
        // Sample the depth camera to see if there is an object between
        // position and sampleWorld
        float2 sampleTex = ToTextureCoordinates(sampleProjection.xy, sampleProjection.w);
        float sampleDepth = ReadDepth(sampleTex.xy);
        
        // If something is closer position is (partially)-occluded
        if (abs(depth - sampleDepth) < SampleRadius)
        {
            ambientOcclusion += step(sampleDepth, depth);
        }
    }

    ambientOcclusion = 1.0f - (ambientOcclusion / 64.0f);    
    ambientOcclusion = pow(ambientOcclusion, Strength);
    float x = (Color.r * 0.000001f) + ambientOcclusion;
    return float4(x, x, x, 0.0f);

    // TODO: real term here, above is for debugging!
    //return float4(Color.rgb * ambientOcclusion, 0.0f);
}

technique AmbientLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}