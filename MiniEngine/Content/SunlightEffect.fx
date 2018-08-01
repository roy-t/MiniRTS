#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/Samplers.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

// For debugging: turn on to give each cascade a different color
static const bool visualizeCascades = false;

float3 SurfaceToLightVector;
float3 LightColor;
float3 LightPosition;
float3 CameraPosition;

// Shadow stuff
static const float BlendThreshold = 0.3f;
static const float Bias = 0.005f;
static const uint NumCascades = 4;

float4x4 ShadowMatrix;
float4 CascadeSplits;
float4 CascadeOffsets[NumCascades];
float4 CascadeScales[NumCascades];

Texture2DArray ShadowMap : register(t0);
SamplerComparisonState ShadowSampler : register(s0);

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

    output.Position = float4(input.Position, 1);
    output.TexCoord = input.TexCoord;
    return output;
}

float SampleShadowMap(float2 baseUv, float u, float v, float2 shadowMapSizeInv, uint cascadeIndex, float depth)
{
    float2 uv = baseUv + float2(u, v) * shadowMapSizeInv;
    float z = depth;

    return ShadowMap.SampleCmpLevelZero(ShadowSampler, float3(uv, cascadeIndex), z);
}

float SampleShadowMapPCF(float3 shadowPosition, uint cascadeIndex)
{
    float2 shadowMapSize;
    float numSlices;
    ShadowMap.GetDimensions(shadowMapSize.x, shadowMapSize.y, numSlices);

    float lightDepth = shadowPosition.z -Bias;	        

    float2 uv = shadowPosition.xy * shadowMapSize;
    float2 shadowMapSizeInv = 1.0f / shadowMapSize;
    
    float2 baseUv;
    baseUv.x = floor(uv.x + 0.5f);
    baseUv.y = floor(uv.y + 0.5f);

    float s = (uv.x + 0.5f - baseUv.x);
    float t = (uv.y + 0.5f - baseUv.y);

    baseUv -= float2(0.5f, 0.5f);
    baseUv *= shadowMapSizeInv;

    float sum = 0.0f;

    float uw0 = (4 - 3 * s);
    float uw1 = 7;
    float uw2 = (1 + 3 * s);

    float u0 = (3 - 2 * s) / uw0 - 2;
    float u1 = (3 + s) / uw1;
    float u2 = s / uw2 + 2;

    float vw0 = (4 - 3 * t);
    float vw1 = 7;
    float vw2 = (1 + 3 * t);

    float v0 = (3 - 2 * t) / vw0 - 2;
    float v1 = (3 + t) / vw1;
    float v2 = t / vw2 + 2;

    sum += uw0 * vw0 * SampleShadowMap(baseUv, u0, v0, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw1 * vw0 * SampleShadowMap(baseUv, u1, v0, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw2 * vw0 * SampleShadowMap(baseUv, u2, v0, shadowMapSizeInv, cascadeIndex, lightDepth);

    sum += uw0 * vw1 * SampleShadowMap(baseUv, u0, v1, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw1 * vw1 * SampleShadowMap(baseUv, u1, v1, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw2 * vw1 * SampleShadowMap(baseUv, u2, v1, shadowMapSizeInv, cascadeIndex, lightDepth);

    sum += uw0 * vw2 * SampleShadowMap(baseUv, u0, v2, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw1 * vw2 * SampleShadowMap(baseUv, u1, v2, shadowMapSizeInv, cascadeIndex, lightDepth);
    sum += uw2 * vw2 * SampleShadowMap(baseUv, u2, v2, shadowMapSizeInv, cascadeIndex, lightDepth);

    return sum * 1.0f / 144;
}


float3 SampleShadowCascade(float3 shadowPosition, uint cascadeIndex)
{
    shadowPosition += CascadeOffsets[cascadeIndex].xyz;
    shadowPosition *= CascadeScales[cascadeIndex].xyz;
    
    if (visualizeCascades) {

        const float3 CascadeColors[NumCascades] =
        {
            float3(1.0f, 0.0f, 0.0f),
            float3(0.0f, 1.0f, 0.0f),
            float3(0.0f, 0.0f, 1.0f),
            float3(1.0f, 1.0f, 0.0f)
        };

        return CascadeColors[cascadeIndex];
    }
    
    float shadow = SampleShadowMapPCF(shadowPosition, cascadeIndex);	
    return float3(shadow, shadow, shadow);
}

float3 GetLightFactor(float3 positionWS, float depthVS, float2 texCoord)
{
    float3 shadowVisibility = float3(1.0f, 1.0f, 1.0f);
    uint cascadeIndex = 0;

    [unroll]
    for (uint i = 0; i < NumCascades - 1; ++i)
    {
        [flatten]
        if (depthVS > CascadeSplits[i])
            cascadeIndex = i + 1;
    }

    // Project into shadow space
    float3 samplePos = positionWS;
    float3 shadowPosition = mul(float4(samplePos, 1.0f), ShadowMatrix).xyz;    

    shadowVisibility = SampleShadowCascade(shadowPosition, cascadeIndex);
    
    // Sample the next cascade, and blend between the two results to smooth the transition    
    float nextSplit = CascadeSplits[cascadeIndex];
    float splitSize = cascadeIndex == 0 ? nextSplit : nextSplit - CascadeSplits[cascadeIndex - 1];
    float splitDist = (nextSplit - depthVS) / splitSize;

    [branch]
    if (splitDist <= BlendThreshold && cascadeIndex != NumCascades - 1)
    {
        float3 nextSplitVisibility = SampleShadowCascade(shadowPosition, cascadeIndex + 1);
        float lerpAmt = smoothstep(0.0f, BlendThreshold, splitDist);
        shadowVisibility = lerp(nextSplitVisibility, shadowVisibility, lerpAmt);
    }

    return shadowVisibility;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;

    float3 normal = ReadNormals(texCoord);
    float specularPower = ReadSpecularPower(texCoord);
    float specularIntensity = ReadSpecularIntensity(texCoord);
    
    float depthVal = tex2D(depthSampler,input.TexCoord).r;

    // Compute the world position without using the helper function
    // because we need also need the scaled distance stored in the depth sampler
    float4 position;
    position.x = input.TexCoord.x * 2.0f - 1.0f;
    position.y = -(input.TexCoord.y * 2.0f - 1.0f);
    position.z = depthVal;
    position.w = 1.0f;

    position = mul(position, InverseViewProjection);
    float distance = depthVal / position.w;
    position /= position.w;
        
    float3 lightFactor = GetLightFactor(position.xyz, distance, input.TexCoord);
          
    //compute diffuse light
    float3 diffuseLight = ComputeDiffuseLightFactor(SurfaceToLightVector, normal, LightColor);
    float specularLight = ComputeSpecularLightFactor(SurfaceToLightVector, normal, position, CameraPosition, specularPower, specularIntensity);

    return float4(diffuseLight.rgb * lightFactor, specularLight * lightFactor.r);    
}

technique DirectionalLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}