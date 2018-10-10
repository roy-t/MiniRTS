#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

// Bias to prevent shadow acne
static const float bias = 0.0001f;

float4x4 LightViewProjection;

float SpecularPower = 20;

float3 LightPosition;
float3 LightDirection;
float3 CameraPosition;
float3 Color; 

Texture2D ShadowMap : register(t0);
SamplerComparisonState ShadowSampler : register(s0);

Texture2D ColorMap;
sampler colorSampler = sampler_state
{
    Texture = (ColorMap);
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

float3 SampleShadowMap(float2 baseUv, float u, float v, float2 shadowMapSizeInv, float z)
{
    float2 uv = baseUv + float2(u, v) * shadowMapSizeInv;
    float shadow = ShadowMap.SampleCmpLevelZero(ShadowSampler, uv, z);                
    float3 color = ColorMap.Sample(colorSampler, uv, 0).rgb;

    return color * shadow;
}

float3 SampleShadowMapPCF(float2 shadowMapCoordinates, float z)
{
    float2 shadowMapSize;
    ShadowMap.GetDimensions(shadowMapSize.x, shadowMapSize.y);
    float2 uv = shadowMapCoordinates * shadowMapSize;
    float2 shadowMapSizeInv = 1.0f / shadowMapSize;

    float2 baseUv;
    baseUv.x = floor(uv.x + 0.5f);
    baseUv.y = floor(uv.y + 0.5f);

    float s = (uv.x + 0.5f - baseUv.x);
    float t = (uv.y + 0.5f - baseUv.y);

    baseUv -= float2(0.5f, 0.5f);
    baseUv *= shadowMapSizeInv;

    float3 sum = 0.0f;

    float uw0 = (4 - 3 * s);
    float uw1 = 7;
    float uw2 = (1 + 3 * s);

    float u0 = (3 - 2 * s) / uw0 - 2;
    float u1 = (3 + s) / uw1;
    float u2 = s / uw2 + 2;

    float vw0 = (4 - 3 * t);
    float vw1 = 7;
    float vw2 = (1 + 3 * t);

    float v0 = (3 - 2 * t) / vw0 - 2;
    float v1 = (3 + t) / vw1;
    float v2 = t / vw2 + 2;

    sum += uw0 * vw0 * SampleShadowMap(baseUv, u0, v0, shadowMapSizeInv, z);
    sum += uw1 * vw0 * SampleShadowMap(baseUv, u1, v0, shadowMapSizeInv, z);
    sum += uw2 * vw0 * SampleShadowMap(baseUv, u2, v0, shadowMapSizeInv, z);
    
    sum += uw0 * vw1 * SampleShadowMap(baseUv, u0, v1, shadowMapSizeInv, z);
    sum += uw1 * vw1 * SampleShadowMap(baseUv, u1, v1, shadowMapSizeInv, z);
    sum += uw2 * vw1 * SampleShadowMap(baseUv, u2, v1, shadowMapSizeInv, z);
    
    sum += uw0 * vw2 * SampleShadowMap(baseUv, u0, v2, shadowMapSizeInv, z);
    sum += uw1 * vw2 * SampleShadowMap(baseUv, u1, v2, shadowMapSizeInv, z);
    sum += uw2 * vw2 * SampleShadowMap(baseUv, u2, v2, shadowMapSizeInv, z);
    return sum / 144.0f;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;

    float3 normal = ReadNormals(texCoord);
    float shininess = ReadShininess(texCoord);    

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
        float3 lightFactor = SampleShadowMapPCF(shadowMapCoordinates, distanceToLightSource - bias);
        
        float3 lightVector = normalize(-LightDirection);

        float3 diffuseLight = ComputeDiffuseLightFactor(lightVector, normal, Color);
        float specularLight = ComputeSpecularLightFactor(lightVector, normal, position, CameraPosition, shininess, SpecularPower);
            
        float colorMapGrayScale = (lightFactor.r + lightFactor.g + lightFactor.b) / 3.0f;

        return float4(diffuseLight.rgb * lightFactor, specularLight * colorMapGrayScale);                
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