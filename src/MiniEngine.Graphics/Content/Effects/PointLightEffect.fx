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

// Inspired by https://learnopengl.com/PBR/ and http://www.codinglabs.net/article_physically_based_rendering_cook_torrance.aspx

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    // Read data from G-Buffer
    float3 diffuse = ReadDiffuse(input.Texture);
    float3 N = ReadNormal(input.Texture);
    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    Mat material = ReadMaterial(input.Texture);

    // The view vector points from the object to the camera. The closer the view vector is to the
    // original reflection direction the stronger the specular reflection.
    float3 V = normalize(CameraPosition - worldPosition);

    // F0 is the basis reflectivity of the material at a 0 degree angle. Dia-electric materials,
    // like plastic, in general have a low reflectivity. While metals, which are conductors, have a
    // high reflectivity that is tinted by surface color The reflectance at normal incidence depends
    // on the metalicness of the material.
    float3 F0 = float3(0.04f, 0.04f, 0.04f);
    F0 = lerp(F0, diffuse, material.Metalicness);

    // The light vector points from the object to the light
    float3 L = normalize(Position - worldPosition);

    // The halfway vector sits halfway between the L and V vectors. The closer it aligns with the
    // normal vector (including the modelled roughness of the material) the closer the view
    // direction is to the original reflection direction, which leads to a stronger specular reflection.
    float3 H = normalize(V + L);

    // Attenuation models how the influence of a light grows weaker over distance due to scattering
    // in the air.
    float dist = distance(Position, worldPosition);
    float attenuation = 1.0f / (dist * dist);

    // The input color is in sRGB color space and needs to convert to linear color space first

    // Radiance is how much light the light source is producing. Strength in this case does not have
    // a real unit. But think of it as the amount of Lumen produced by the light.
    float3 radiance = ToLinear(Color).rgb * Strength * attenuation;

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

    output.Light = float4(Lo, 1.0f);

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
