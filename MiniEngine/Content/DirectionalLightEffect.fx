#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/Samplers.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

float SpecularPower = 20;

float3 LightDirection;
float3 CameraPosition;
float3 Color; 

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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = float4(input.Position,1);       
    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{    
    float2 texCoord = input.TexCoord;

    float3 normal = ReadNormals(texCoord);
    float shininess = ReadShininess(texCoord);    

    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);
        
    float3 lightVector = normalize(-LightDirection);

    float3 diffuseLight = ComputeDiffuseLightFactor(lightVector, normal, Color);
    float specularLight = ComputeSpecularLightFactor(lightVector, normal, position, CameraPosition, shininess, SpecularPower);
  
    return float4(diffuseLight.rgb, specularLight);
}

technique DirectionalLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}