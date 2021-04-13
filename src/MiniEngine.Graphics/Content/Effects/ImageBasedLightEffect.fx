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

TextureCube Irradiance;
TextureCube Environment;
Texture2D BrdfLut;

SamplerState TextureSampler : register(s0); // Linear clamp

float3 CameraPosition;
float4x4 InverseViewProjection;
int MaxReflectionLod;
float AmbientLightFactor;

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
    // Note: this function differs slightly from the general light calculations in Lights.hlsl

    // Read data from G-Buffer
    float3 albedo = ReadAlbedo(input.Texture);
    float3 N = ReadNormal(input.Texture);
    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    Mat material = ReadMaterial(input.Texture);

    // The view vector points from the object to the camera. The closer the view vector is to the
    // original reflection direction the stronger the specular reflection.
    float3 V = normalize(CameraPosition - worldPosition);

    // The R vector is the reflection of the vector from the camera to the object over the normal
    // vector at this position. It points to what we would see if the current position is a perfect
    // mirror aligned with the normal vector.
    float3 R = reflect(-V, N);

    // F0 is the basis reflectivity of the material at a 0 degree angle. Dia-electric materials,
    // like plastic, in general have a low reflectivity. While metals, which are conductors, have a
    // high reflectivity that is tinted by surface color The reflectance at normal incidence depends
    // on the metalicness of the material.
    float3 F0 = float3(0.04f, 0.04f, 0.04f);
    F0 = lerp(F0, albedo, material.Metalicness);

    // Chance the light is reflected (instead of refracted) based on the viwing angle and roughness
    // of the material
    float NdotV = clamp(dot(N, V), 0.0f, 1.0f);
    float3 F = FresnelSchlickRoughness(NdotV, F0, material.Roughness);

    // kS is the amount of light reflected (specular light)
    float3 kS = F;

    // Logically what remains is kD, the diffuse light
    float3 kD = float3(1.0f, 1.0f, 1.0f) - kS;

    // Metalic objects do not have a diffuse light component, instead they produce mirror like
    // reflections which we will take care of here
    kD *= 1.0f - material.Metalicness;

    // Sample the incoming light from the irradiance cube map
    float3 irradiance = Irradiance.Sample(TextureSampler, N).rgb; // in linear color space
    float3 diffuse = irradiance * albedo;

    // Sample the reflections that metalic materials will mirror, taking into account the BRDF
    // (bidirectional reflectance distribution function) which is based on the angle of the viewer
    // in the relation to the normal and the roughness of the material.    
    float3 prefilteredColor = Environment.SampleLevel(TextureSampler, R, material.Roughness * MaxReflectionLod).rgb;
    float2 brdf =  BrdfLut.Sample(TextureSampler, float2(NdotV, material.Roughness)).rg;
    float3 specular = prefilteredColor * (F * brdf.x + brdf.y);

    // Combine the light (diffuse) and reflections (specular) and modify them by the general occlusion
    float3 ambient = (kD * diffuse + specular) * material.AmbientOcclusion;

    output.Light = float4(ambient * AmbientLightFactor, 1.0f);

    return output;
}

technique ImageBasedLightTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}