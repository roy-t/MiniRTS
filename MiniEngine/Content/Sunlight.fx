#include "ShadowFunctions.hlsl"

#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0
    #define PS_SHADERMODEL ps_4_0
#endif

float4x4 InverseViewProjection; 
float4x4 LightView;
float4x4 LightProjection;

float3 LightDirection;
float3 Color; 
float3 LightPosition;

float3 CameraPosition;

texture ColorMap; 
sampler colorSampler = sampler_state
{
    Texture = (ColorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture = (NormalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
};

texture DepthMap;
sampler depthSampler = sampler_state
{
    Texture = (DepthMap);
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

    output.Position = float4(input.Position,1);
    //align texture coordinates
    output.TexCoord = input.TexCoord;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    //get normal data from the normalMap
    float4 normalData = tex2D(normalSampler,input.TexCoord);
    //tranform normal back into [-1,1] range
    float3 normal = 2.0f * normalData.xyz - 1.0f;
    //get specular power, and get it into [0,255] range]
    float specularPower = normalData.a * 255;
    //get specular intensity from the colorMap
    float specularIntensity = tex2D(colorSampler, input.TexCoord).a;
    
    //read depth
    float depthVal = tex2D(depthSampler,input.TexCoord).r;

    //compute screen-space position
    float4 position;
    position.x = input.TexCoord.x * 2.0f - 1.0f;
    position.y = -(input.TexCoord.y * 2.0f - 1.0f);
    position.z = depthVal;
    position.w = 1.0f;

    //transform to world space
    position = mul(position, InverseViewProjection);
    position /= position.w;

    ShadowData shadowData = GetShadowData(position);    
    float shadowFactor = GetShadowFactor(shadowData);

    //Do the lighting calculations

    //surface-to-light vector
    float3 lightVector = -normalize(LightDirection);

    //compute diffuse light
    float NdL = max(0,dot(normal,lightVector));
    float3 diffuseLight = NdL * Color.rgb;

    //reflection vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));
    //camera-to-surface vector
    float3 directionToCamera = normalize(CameraPosition - position.xyz);
    //compute specular light
    float rdot = clamp(dot(reflectionVector, directionToCamera), 0, abs(specularIntensity));    
    float specularLight = pow(abs(rdot), specularPower);

    // ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);
    // float3 a = float3(0, 0, 0);    
    // if(splitInfo.SplitIndex == 0)
    // {
    //     a = float3(0.5f, 0, 0);
    // }
    // if(splitInfo.SplitIndex == 1)
    // {
    //     a = float3(0, 0.5f, 0);
    // }
    // if(splitInfo.SplitIndex == 2)
    // {
    //     a = float3(0, 0, 0.5f);
    // }    
    // return float4(diffuseLight.rgb * shadowFactor + a, specularLight * shadowFactor);        
    return float4(diffuseLight.rgb * shadowFactor, specularLight * shadowFactor);        
}

technique DirectionalLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}