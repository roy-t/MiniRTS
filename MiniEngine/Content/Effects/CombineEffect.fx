// Combines the information in the geometry-buffer into a correctly lit picture

#include "Includes/Defines.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

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

    float4 diffuseColor = ReadDiffuse(texCoord);
    float4 light = ReadLight(texCoord);    
    float4 diffuseLight = float4(light.rgb, 1.0f);
    float4 specularLight = float4(light.a, light.a, light.a, 0.0f);
   
    return saturate(float4(diffuseColor * diffuseLight + specularLight));    
}

technique Combine
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
