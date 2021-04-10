#include "Includes/Defines.hlsl"
#include "Includes/Instancing.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float4 WorldPosition: TEXCOORD0;
    float2 Texture : TEXCOORD1;
};

struct OutputData
{
    float Depth : COLOR0;
};

float4x4 WorldViewProjection;
Texture2D Albedo;

const sampler anisotropicSampler = sampler_state
{
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Texture = input.Texture;
    output.WorldPosition = output.Position;

    return output;
}


PixelData VS_INSTANCED(in VertexData input, in InstancingData instance)
{
    PixelData output = (PixelData)0;

    float4x4 offsetT = transpose(instance.Offset);

    output.Position = mul(mul(float4(input.Position, 1), offsetT), WorldViewProjection);
    output.Texture = input.Texture;
    output.WorldPosition = output.Position;

    return output;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;
    
    float4 albedo = Albedo.Sample(anisotropicSampler, input.Texture);
    clip(albedo.a - 1.0f);

    output.Depth = input.WorldPosition.z / input.WorldPosition.w;    
    return output;
}

technique ShadowMapTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}

technique InstancedShadowMapTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS_INSTANCED();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
