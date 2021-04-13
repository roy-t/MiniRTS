#ifndef __GBUFFERREADER
#define __GBUFFERREADER

#include "Pack.hlsl"
#include "Material.hlsl"

Texture2D Albedo;
Texture2D Normal;
Texture2D Depth;
Texture2D Material;

SamplerState GBufferSampler : register(s9); // Linear Clamp


float3 ReadAlbedo(float2 texCoord)
{
    return Albedo.Sample(GBufferSampler, texCoord).rgb;
}

float3 ReadNormal(float2 texCoord)
{
    float3 normal = Normal.Sample(GBufferSampler, texCoord).xyz;
    return UnpackNormal(normal);
}

float ReadDepth(float2 texCoord)
{
    return Depth.Sample(GBufferSampler, texCoord).r;
}

float3 ReadWorldPosition(float2 texCoord, float depthVal, float4x4 inverseViewProjection)
{
    // Compute screen-space position
    float4 position;
    position.x = texCoord.x * 2.0f - 1.0f;
    position.y = -(texCoord.y * 2.0f - 1.0f);
    position.z = depthVal;
    position.w = 1.0f;

    // Transform to world space
    position = mul(position, inverseViewProjection);
    position /= position.w;

    return position.xyz;
}

float3 ReadWorldPosition(float2 texCoord, float4x4 inverseViewProjection)
{
    float depthVal = ReadDepth(texCoord);
    return ReadWorldPosition(texCoord, depthVal, inverseViewProjection);
}

Mat ReadMaterial(float2 texCoord)
{
    Mat material = (Mat)0;    
    float3 mat = Material.Sample(GBufferSampler, texCoord).xyz;

    material.Metalicness = mat.x;
    material.Roughness = mat.y;
    material.AmbientOcclusion = mat.z;

    return material;
}

#endif
