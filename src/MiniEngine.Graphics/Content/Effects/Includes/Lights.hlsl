#ifndef __LIGHTS
#define __LIGHTS

#include "Material.hlsl"
#include "Gamma.hlsl"

// The distribution function compute the chance that a micro facet is pointing in the direction
// of the halfway vector. Remember that if a micro facet points in the direction of the halfway
// vector it would cause light to be reflected into the direction of the view vector. Which
// would lead to a stronger specular reflection. The rougher the surface is the smaller this
// chance will be.
float DistributionGGX(float3 N, float3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float nom = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / max(denom, EPSILON); // prevent divide by zero for roughness=0.0 and NdotH=1.0
}

// Distribution function for GeometrySmith
float GeometrySchlickGGX(float NdotV, float roughness, float k)
{
    float nom = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;
}

// Models the probability that micro facets are shadowing each other. This leads to the light
// needing multiple bounces before it can reach the viewer. Each bounce costing a little bit of
// energy. Rougher surfaces have a higher chance of self-shadowing and will thus appear darker.
float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);

    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float ggx2 = GeometrySchlickGGX(NdotV, roughness, k);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness, k);

    return ggx1 * ggx2;
}

// GeometrySmith function for Image Based Lighting, here k is rescaled differently. I could not find
// an explanation why the rescaling is different.
float GeometrySmithIBL(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);

    float r = roughness;
    float k = (r * r) / 2.0f;

    float ggx2 = GeometrySchlickGGX(NdotV, roughness, k);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness, k);

    return ggx1 * ggx2;
}

// Compute the precentage of the light that gets reflected (instead of refracted) based on the
// viewing angle. At a more extreme angle more light is reflected than refracted. Which leads to a
// sort of outline effect.
float3 FresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

// Fresnel approximation that takes roughness into account, for image based lighting
float3 FresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    float invRoughness = 1.0 - roughness;
    return F0 + (max(float3(invRoughness, invRoughness, invRoughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}

float3 ComputeLight(
    // Object properties
    float3 diffuse,
    float3 N,
    Mat material,
    float3 worldPosition,
    // Camera properties
    float3 cameraPosition,
    // Light properties
    float3 lightPosition,
    float4 color,
    float strength)
{
    // The view vector points from the object to the camera. The closer the view vector is to the
   // original reflection direction the stronger the specular reflection.
    float3 V = normalize(cameraPosition - worldPosition);

    // F0 is the basis reflectivity of the material at a 0 degree angle. Dia-electric materials,
    // like plastic, in general have a low reflectivity. While metals, which are conductors, have a
    // high reflectivity that is tinted by surface color The reflectance at normal incidence depends
    // on the metalicness of the material.
    float3 F0 = float3(0.04f, 0.04f, 0.04f);
    F0 = lerp(F0, diffuse, material.Metalicness);

    // The light vector points from the object to the light
    float3 L = normalize(lightPosition - worldPosition);

    // The halfway vector sits halfway between the L and V vectors. The closer it aligns with the
    // normal vector (including the modelled roughness of the material) the closer the view
    // direction is to the original reflection direction, which leads to a stronger specular reflection.
    float3 H = normalize(V + L);

    // Attenuation models how the influence of a light grows weaker over distance due to scattering
    // in the air.
    float dist = distance(lightPosition, worldPosition);
    float attenuation = 1.0f / (dist * dist);

    // The input color is in sRGB color space and needs to convert to linear color space first

    // Radiance is how much light the light source is producing. Strength in this case does not have
    // a real unit. But think of it as the amount of Lumen produced by the light.
    float3 radiance = ToLinear(color).rgb * strength * attenuation;

    // Compute the distribution of the diffuse and specular reflection using the Cook-Torrance BRDF model

    // Chance micro facet reflects light to viewer
    float NDF = DistributionGGX(N, H, material.Roughness);

    // Chance geometry does not obscure reflected light
    float G = GeometrySmith(N, V, L, material.Roughness);

    // Chance the light is reflected (instead of refracted) based on the viewing angle
    float3 F = FresnelSchlick(clamp(dot(H, V), 0.0f, 1.0f), F0);

    // Combine all parts of the Cook-Torrance model and compute the specular lighting
    float3 nominator = NDF * G * F;
    float denominator = 4 * clamp(dot(N, V), 0.0f, 1.0f) * clamp(dot(N, L), 0.0f, 1.0f);
    float3 specular = nominator / max(denominator, EPSILON);

    // kS is the amount of light reflected (specular light)
    float3 kS = F;

    // Logically what remains is kD, the diffuse light
    float3 kD = float3(1.0f, 1.0f, 1.0f) - kS;

    // Metalic objects do not have a diffuse light component, instead they produce mirror like
    // reflections, which is taken care of in the ImageBasedLightEffect shader.
    kD *= 1.0f - material.Metalicness;

    // scale light by NdotL
    float NdotL = clamp(dot(N, L), 0.0f, 1.0f);

    // The final light color is based on a diffuse component and a specular component. It is scaled
    // by radiance (light strength) and how much the outgoing light vector aligns in the direction
    // of the viewer. Or in other words how much the light is shining in the viewer's direction.
    float3 Lo = (kD * diffuse / PI + specular) * radiance * NdotL;

    return Lo;
}

#endif
