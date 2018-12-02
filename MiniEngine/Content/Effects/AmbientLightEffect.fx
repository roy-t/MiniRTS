// Copies the color information in the diffuse texture to the currently set render target

#include "Includes/Defines.hlsl"

float3 Color;

float4 Offsets = float4(-1.5f, -0.5f, 0.5f, 1.5f);

Texture2D AmbientOcclusionMap;
sampler ambientOcclusionSampler = sampler_state
{
    Texture = (AmbientOcclusionMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = float4(input.Position,1);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;
    float2 dimensions;
    AmbientOcclusionMap.GetDimensions(dimensions.x, dimensions.y);

    float occlusion = 0.0f;
    for (int x = 0; x < 4; x++)
    {
        for (int y = 0; y < 4; y++)
        {
            float2 tc = texCoord;
            tc.x = texCoord.x + (Offsets[x] / dimensions.x);
            tc.y = texCoord.y + (Offsets[y] / dimensions.y);
            occlusion += tex2D(ambientOcclusionSampler, tc).r;
        }
    }        

    occlusion /= 16.0f;
    /*occlusion = tex2D(ambientOcclusionSampler, texCoord).r;*/

    return float4(Color.r * occlusion, Color.g * occlusion, Color.b * occlusion, 0.0f);
}

technique AmbientLightTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
