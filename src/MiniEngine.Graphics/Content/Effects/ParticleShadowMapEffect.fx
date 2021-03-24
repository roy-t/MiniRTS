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
    float4 WorldPosition: TEXCOORD0;
    float Age : TEXCOORD1;
};

struct OutputData
{
    float Depth : COLOR0;
};

float4x4 WorldViewProjection;
Texture2D Data;

PixelData VS_INSTANCED(in VertexData input, in Particle particle)
{
    PixelData output = (PixelData)0;

    float2 dimensions;
    Data.GetDimensions(dimensions.x, dimensions.y);

    int3 uvi = int3((dimensions * particle.UV), 0);
    float4 data = Data.Load(uvi);

    float4x4 world =
    {
        1.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        data.x, data.y, data.z, 1.0f
    };

    output.Position = mul(mul(float4(input.Position, 1), world), WorldViewProjection);
    output.WorldPosition = output.Position;
    output.Age = data.w;
    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    clip(input.Age);
    output.Depth = input.WorldPosition.z / input.WorldPosition.w;
    return output;
}

technique ParticleShadowMapTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_INSTANCED();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
