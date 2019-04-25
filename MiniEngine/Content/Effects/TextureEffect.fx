#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"

float3 WorldPosition;
float3 CameraPosition;

float4 VisibleTint;
float4 ClippedTint;

texture Texture;
sampler textureSampler = sampler_state
{
    Texture = (Texture);
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
    float4 UVPosition: TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);   
    output.UVPosition = output.Position;
    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{    
    float2 texCoord = input.TexCoord;
    return tex2D(textureSampler, texCoord) * VisibleTint;
}

float4 DepthPS(VertexShaderOutput input) : COLOR0
{
    float2 sampleCoord = ToTextureCoordinates(input.UVPosition.xy, input.UVPosition.w);
    float4 pixelWorldPosition = ReadWorldPosition(sampleCoord, InverseViewProjection);

    float4 position = mul(input.UVPosition, InverseViewProjection);
    position /= position.w;

    float pixelDistance = distance(CameraPosition, pixelWorldPosition.xyz);
    float worldDistance = distance(CameraPosition, position.xyz);    

    if (worldDistance > pixelDistance)
    {
        return tex2D(textureSampler, input.TexCoord) * ClippedTint;
    }
        
    return tex2D(textureSampler, input.TexCoord) * VisibleTint;
}

float4 PointDepthPS(VertexShaderOutput input) : COLOR0
{
    float2 sampleCoord = ToTextureCoordinates(input.UVPosition.xy, input.UVPosition.w);
    float4 pixelWorldPosition = ReadWorldPosition(sampleCoord, InverseViewProjection);

    float pixelDistance = distance(CameraPosition, pixelWorldPosition.xyz);
    float worldDistance = distance(CameraPosition, WorldPosition);

    if (worldDistance > pixelDistance)
    {
        return tex2D(textureSampler, input.TexCoord) * ClippedTint;
    }

    return tex2D(textureSampler, input.TexCoord) * VisibleTint;
}

technique TextureEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}

technique TextureGeometryDepthTestEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL DepthPS();
    }
}

technique TexturePointDepthTestEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL PointDepthPS();
    }
}