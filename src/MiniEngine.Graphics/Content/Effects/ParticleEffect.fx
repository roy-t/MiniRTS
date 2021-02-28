#include "Includes/Defines.hlsl"
#include "Includes/Instancing.hlsl"
#include "Includes/Gamma.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float3 Coordinates : TEXCOORD0;
    float4 Color : TEXCOORD1;
};

struct OutputData
{
    // COLOR0 is only set so we can sample its depth buffer
    float4 Color : COLOR1;
};

float4x4 WorldViewProjection;
float4x4 View;

PixelData VS_INSTANCED(in VertexData input, in ParticleInstancingData instance)
{
    PixelData output = (PixelData)0;

    float4x4 offset =
    {
        View._11, View._21, View._31, 0.0f,
        View._12, View._22, View._32, 0.0f,
        View._13, View._23, View._33, 0.0f,
        instance.Position.x, instance.Position.y, instance.Position.z, 1.0f
    };

    float4x4 scale =
    {
        instance.Scale, 0, 0, 0,
        0, instance.Scale, 0, 0,
        0, 0, instance.Scale, 0,
        0, 0, 0, 1,
    };

    output.Position = mul(mul(mul(float4(input.Position, 1), scale), offset), WorldViewProjection);
    output.Coordinates = input.Position;
    output.Color = instance.Color;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    clip(0.5f - length(input.Coordinates));
    output.Color = ToLinear(input.Color);

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
