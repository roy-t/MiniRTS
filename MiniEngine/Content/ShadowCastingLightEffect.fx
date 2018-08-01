#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/Samplers.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

// Bias to prevent shadow acne
static const float bias = 0.0001f;

float4x4 LightViewProjection;

float3 LightPosition;
float3 LightDirection;
float3 CameraPosition;
float3 Color; 

texture ShadowMap;
sampler shadowSampler = sampler_state
{
    Texture = (ShadowMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = float4(input.Position, 1);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;

    float3 normal = ReadNormals(texCoord);
    float specularPower = ReadSpecularPower(texCoord);
    float specularIntensity = ReadSpecularIntensity(texCoord);

    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);

    // Move from world position to the reference frame of the light
    float4 positionInLightReferenceFrame = mul(position, LightViewProjection);

    // Figure out where on the shadow map the current pixel is
    float2 shadowMapCoordinates = ToTextureCoordinates(positionInLightReferenceFrame.xy, positionInLightReferenceFrame.w);
        
    // Only if the current pixel is on the shadow map (inside legal texture coordinates) the pixel is seen by the light
    if(shadowMapCoordinates.x >= 0.0f && shadowMapCoordinates.x <= 1.0f &&
        shadowMapCoordinates.y >= 0.0f && shadowMapCoordinates.y <= 1.0f)
    {        		                
        float distanceToLightSource = (positionInLightReferenceFrame.z / positionInLightReferenceFrame.w);
        float shadowMapSample = tex2D(shadowSampler, shadowMapCoordinates).r;
                
        if((distanceToLightSource - bias) <= shadowMapSample)
        {               
            float3 lightVector = normalize(-LightDirection);

            float3 diffuseLight = ComputeDiffuseLightFactor(lightVector, normal, Color);
            float specularLight = ComputeSpecularLightFactor(lightVector, normal, position, CameraPosition, specularPower, specularIntensity);

            return float4(diffuseLight.rgb, specularLight);        
        }                
    }

    return float4(0.0f, 0.0f, 0.0f, 0.0f);
}

technique DirectionalLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}