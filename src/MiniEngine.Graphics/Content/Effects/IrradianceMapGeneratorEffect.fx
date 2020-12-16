#include "Includes/Defines.hlsl"
#include "Includes/Coordinates.hlsl"
#include "Includes/Gamma.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float3 Position3D : TEXCOORD0;
};

struct OutputData
{
    float4 Irradiance : COLOR0;
};

texture EquirectangularTexture;
sampler equirectangularTextureSampler = sampler_state
{
    Texture = (EquirectangularTexture);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

float SampleDelta = 0.025f;
float4x4 WorldViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Position3D = input.Position;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    float3 normal = normalize(input.Position3D);

    float3 irradiance = float3(0, 0, 0);

    float3 right = normalize(cross(float3(0, 1, 0), normal));
    float3 up = normalize(cross(normal, right));

    float nrSamples = 0.0f;

    // Sample a hemisphere by rotating completely around the azimuth of the sphere
    // and going up and down from the north pole to the equator via the zenith
    for (float azimuth = 0.0f; azimuth < TWO_PI; azimuth += SampleDelta)
    {
        for (float zenith = 0.0f; zenith < PI_OVER_TWO; zenith += SampleDelta)
        {
            float3 tangentSample = float3(sin(zenith) * cos(azimuth), sin(zenith) * sin(azimuth), cos(zenith));

            float3 sampleVec = tangentSample.x * right + tangentSample.y * up + tangentSample.z * normal;

            float2 uv = WorldToSpherical(sampleVec);
            float4 diffuse = tex2D(equirectangularTextureSampler, uv);
            float4 diffuseLinear = ToLinear(diffuse);
            irradiance += diffuseLinear.rgb;
            nrSamples++;
        }
    }

    output.Irradiance = float4(PI * irradiance * (1.0f / nrSamples), 1.0f);

    return output;
}

technique IrradianceMapGeneratorTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
