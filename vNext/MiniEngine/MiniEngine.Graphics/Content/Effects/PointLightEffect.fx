#include "Includes/Defines.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/GBufferReader.hlsl"
#include "Includes/Lights.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct PixelData
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct OutputData
{
    float4 Light : COLOR0;    
};

float4 Color;
float Strength;
float3 Position;
float3 CameraPosition;
float4x4 InverseViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1.0f);
    output.Texture = input.Texture;
    
    return output;
}


// Inspired by https://learnopengl.com/PBR/

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    
    float3 albedo = ReadDiffuse(input.Texture); // Already in linear color space
    float3 N = ReadNormal(input.Texture);
    Mat material = ReadMaterial(input.Texture);
    
    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);    
    float3 V = normalize(CameraPosition - worldPosition);

    // calculate reflectance at normal incidence; if dia-electric (like plastic) use F0 
    // of 0.04 and if it's a metal, use the albedo color as F0 (metallic workflow)    
    float3 F0 = float3(0.04f, 0.04f, 0.04f);
    F0 = lerp(F0, albedo, material.Metalicness);
    
    float3 L = normalize(Position - worldPosition);
    float3 H = normalize(V + L);
    float dist = distance(Position, worldPosition);

    float attenuation = 1.0f / (dist * dist);    
    float3 radiance = ToLinear(Color).rgb * Strength * attenuation; // Convert light color to linear color space first

    // Cook-Torrance BRDF
    float NDF = DistributionGGX(N, H, material.Roughness);
    float G = GeometrySmith(N, V, L, material.Roughness);
    float3 F = FresnelSchlick(clamp(dot(H, V), 0.0f, 1.0f), F0);

    float3 nominator = NDF * G * F;
    float denominator = 4 * max(dot(N, V), 0.0f) * max(dot(N, L), 0.0f);
    float3 specular = nominator / max(denominator, EPSILON);

    // kS is equal to Fresnel
    float3 kS = F;

    // for energy conservation, the diffuse and specular light can't
    // be above 1.0 (unless the surface emits light); to preserve this
    // relationship the diffuse component (kD) should equal 1.0 - kS.
    float3 kD = float3(1.0f, 1.0f, 1.0f) - kS;

    // multiply kD by the inverse metalness such that only non-metals 
    // have diffuse lighting, or a linear blend if partly metal (pure metals
    // have no diffuse light).
    kD *= 1.0f - material.Metalicness;

    // scale light by NdotL
    float NdotL = max(dot(N, L), 0.0f);

    float3 Lo = (kD * albedo / PI + specular) * radiance * NdotL;

    // Replace ambient later
    float a = 0.03f / 4.0f;
    float3 ambient = float3(a, a, a) * albedo * material.AmbientOcclusion;
    
    float3 color = ambient + Lo;  
    
    output.Light = float4(color, 1.0f);

    return output;
}

technique PointLightTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}