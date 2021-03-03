#include "Includes/Defines.hlsl"
#include "Includes/Pack.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/Instancing.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float4 ScreenPosition: TEXCOORD0;
    float2 Coordinates : TEXCOORD1;
    float4 Color : TEXCOORD2;
    float Metalicness : TEXCOORD3;
    float Roughness : TEXCOORD4;
    float3 Normal: NORMAL0;    
};

struct OutputData
{
    float4 Albedo : COLOR0;
    float4 Material : COLOR1;
    float Depth : COLOR2;
    float4 Normal: COLOR3;
};

float4x4 WorldViewProjection;
float4x4 View;

PixelData VS_INSTANCED(in VertexData input, in ParticleInstancingData instance)
{
    PixelData output = (PixelData)0;

    float4x4 world =
    {
        View._11 * instance.Scale, View._21 * instance.Scale, View._31 * instance.Scale, 0.0f,
        View._12 * instance.Scale, View._22 * instance.Scale, View._32 * instance.Scale, 0.0f,
        View._13 * instance.Scale, View._23 * instance.Scale, View._33 * instance.Scale, 0.0f,
        instance.Position.x, instance.Position.y, instance.Position.z, 1.0f
    };

    output.Position = mul(mul(float4(input.Position, 1), world), WorldViewProjection);
    output.Coordinates = input.Position.xy;
    output.ScreenPosition = output.Position;

    output.Color = instance.Color;
    output.Metalicness = instance.Metalicness;
    output.Roughness = instance.Roughness;

    float3x3 rotation = (float3x3)world;
    //float3 normal = float3(input.Position.x, input.Position.y, 0.5f);
    float3 normal = float3(0, 0, 1.0f);
    output.Normal = normalize(mul(normal, rotation));
    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    
    clip(0.5f - length(input.Coordinates));

    output.Albedo = ToLinear(input.Color);
    output.Material = float4(input.Metalicness, input.Roughness, 1.0f, 1.0f);
    output.Depth = input.ScreenPosition.z / input.ScreenPosition.w;
    output.Normal = float4(PackNormal(input.Normal), 1.0f);

    return output;
}

technique ParticleTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_INSTANCED();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
