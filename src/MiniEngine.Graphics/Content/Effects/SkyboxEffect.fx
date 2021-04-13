#include "Includes/Defines.hlsl"
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
    // COLOR0 is only set so we can sample its depth buffer
    float4 Color : COLOR1;
};

TextureCube Skybox;
SamplerState SkyboxSampler : register(s0);

float4x4 WorldViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    float4 position = mul(float4(input.Position, 1), WorldViewProjection);
    // always set the distance to 1.0 so the skybox will be drawn behind everything else
    output.Position = position.xyww;
    output.Position3D = input.Position;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    output.Color =  Skybox.Sample(SkyboxSampler, input.Position3D);

    return output;
}

technique SkyboxTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
