#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/Samplers.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

float3 Color; 
float3 LightPosition;
float3 CameraPosition;

float Radius;
float Intensity = 1.0f;

struct VertexShaderInput
{
    float3 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 ScreenPosition : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    
    // Process geometry coordinates
    float4 worldPosition = mul(float4(input.Position,1), World);
    float4 viewPosition = mul(worldPosition, View);
    
    output.Position = mul(viewPosition, Projection);
    output.ScreenPosition = output.Position;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{ 
    input.ScreenPosition.xy /= input.ScreenPosition.w;    

    float2 texCoord = ToTextureCoordinates(input.ScreenPosition.xy);
        
    float3 normal = ReadNormals(texCoord);
    float specularPower = ReadSpecularPower(texCoord);    
    float specularIntensity = ReadSpecularIntensity(texCoord);
     
    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);    
    float3 lightVector = LightPosition - position.xyz;
    
    float attenuation = saturate(1.0f - length(lightVector)/Radius); 

    lightVector = normalize(lightVector); 
    float3 diffuseLight = ComputeDiffuseLightFactor(lightVector, normal, Color);    
    float specularLight = ComputeSpecularLightFactor(lightVector, normal, position, CameraPosition, specularPower, specularIntensity);
            
    return attenuation * Intensity * float4(diffuseLight.rgb,specularLight);
}

technique PointLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
