#include "Includes/Defines.hlsl"
#include "Includes/GBufferReader.hlsl"
#include "Includes/Coordinates.hlsl"
#include "Includes/Lights.hlsl"
#include "Includes/Shadows.hlsl"

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
float3 SurfaceToLight;
float3 CameraPosition;
float4x4 InverseViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1);
    output.Texture = input.Texture;
    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    // Read data from G-Buffer
    float3 albedo = ReadAlbedo(input.Texture);
    float3 N = ReadNormal(input.Texture);
    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    float depth = distance(worldPosition, CameraPosition);
    
    Mat material = ReadMaterial(input.Texture);

    float lightFactor = ComputeLightFactorPCF(worldPosition, depth);
    float3 Lo = float3(0.0f, 0.0f, 0.0f);
    if (lightFactor > 0)
    {
        Lo = ComputeLight(albedo, N, material, worldPosition, CameraPosition, SurfaceToLight, Color, Strength);        
        // Ignore attenuation since sunlight has already crossed an extreme distance
    }
    
    output.Light = float4(Lo, 1.0f) * lightFactor;    
    return output;
}


OutputData DEBUG_PS(PixelData input)
{
    OutputData output = (OutputData)0;

    float3 worldPosition = ReadWorldPosition(input.Texture, InverseViewProjection);
    float depth = distance(worldPosition, CameraPosition);
    
    uint cascadeIndex = GetCascadeIndex(depth);
    const float3 CascadeColors[NumCascades] =
    {
        float3(1.0f, 0.0f, 0.0f),
        float3(0.0f, 1.0f, 0.0f),
        float3(0.0f, 0.0f, 1.0f),
        float3(1.0f, 1.0f, 0.0f)
    };
    output.Light = float4(CascadeColors[cascadeIndex], 1.0f);
    return output;
}

technique SunlightTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

technique DebugTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL DEBUG_PS();
    }
}
