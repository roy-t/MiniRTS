#include "Includes/Defines.hlsl"

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
    float4 Diffuse : COLOR0;    
};

texture Diffuse;
sampler diffuseSampler = sampler_state
{
    Texture = (Diffuse);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Clamp;
    AddressV = Clamp;
};

float SampleRadius;

static float2 Offsets[16] =
{
    float2(0.2770745f, 0.6951455f),
    float2(0.1874257f, -0.02561589f),
    float2(-0.3381929f, 0.8713168f),
    float2(0.5867746f, 0.1087471f),
    float2(-0.3078699f, 0.188545f),
    float2(0.7993396f, 0.4595091f),
    float2(-0.09242552f, 0.5260149f),
    float2(0.3657553f, -0.5329605f),
    float2(-0.3829718f, -0.2476171f),
    float2(-0.01085108f, -0.6966301f),
    float2(0.8404155f, -0.3543923f),
    float2(-0.5186161f, -0.7624033f),
    float2(-0.8135794f, 0.2328489f),
    float2(-0.784665f, -0.2434929f),
    float2(0.9920505f, 0.0855163f),
    float2(-0.687256f, 0.6711345f)
};


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

    float sum = 0;
    float4 color = float4(0, 0, 0, 0);
    for (int i = 0; i < 16; i++)
    {
        float2 tc = input.Texture;
        tc.x += (Offsets[i].x * SampleRadius);
        tc.y += (Offsets[i].y * SampleRadius);

        sum += 1.0f;
        color += tex2D(diffuseSampler, tc);
    }

    output.Diffuse = color / sum;
    return output;
}

technique BlurTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}