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
    float4 Color : COLOR0;
};

Texture2D EquirectangularTexture;
float4x4 WorldViewProjection;

const sampler equirectangularTextureSampler = sampler_state
{    
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

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
    float2 uv = WorldToSpherical(normalize(input.Position3D));
    float4 color = EquirectangularTexture.Sample(equirectangularTextureSampler, uv);
    output.Color = ToLinear(color);

    return output;
}

technique CubeMapGeneratorTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
