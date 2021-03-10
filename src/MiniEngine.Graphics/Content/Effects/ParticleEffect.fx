#include "Includes/Defines.hlsl"
#include "Includes/Pack.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/Instancing.hlsl"

struct Particle
{
    float2 UV: POSITION1;
};

struct VertexData
{
    float3 Position : POSITION0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float4 ScreenPosition: TEXCOORD0;
    float4 Color : TEXCOORD1;
    float3 Normal: NORMAL0;    
};

struct OutputData
{
    float4 Albedo : COLOR0;
    float4 Material : COLOR1;
    float Depth : COLOR2;
    float4 Normal: COLOR3;
};

float Metalicness;
float Roughness;

float4x4 WorldViewProjection;
float4x4 View;

Texture2D Data;
sampler dataSampler = sampler_state
{
    Texture = (Data);
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

PixelData VS_INSTANCED(in VertexData input, in Particle particle)
{
    PixelData output = (PixelData)0;

    float4 data = Data.SampleLevel(dataSampler, particle.UV, 0);

    float4x4 world =
    {
        View._11 , View._21 , View._31 , 0.0f,
        View._12 , View._22 , View._32 , 0.0f,
        View._13 , View._23 , View._33 , 0.0f,
        data.x, data.y, data.z, 1.0f
    };

    output.Position = mul(mul(float4(input.Position, 1), world), WorldViewProjection);
    output.ScreenPosition = output.Position;
    output.Color = float4(1, 0, 0, 1);

    float3x3 rotation = (float3x3)world;
    float3 normal = float3(0, 0, 1.0f);
    output.Normal = normalize(mul(normal, rotation));
    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    output.Albedo = ToLinear(input.Color);
    output.Material = float4(Metalicness, Roughness, 1.0f, 1.0f);
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
