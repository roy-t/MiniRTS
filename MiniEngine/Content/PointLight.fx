#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InvertViewProjection; 

float3 Color; 
float3 LightPosition;
float Radius;
float Intensity = 1.0f;

float3 CameraPosition; 

texture ColorMap; 
sampler colorSampler = sampler_state
{
    Texture = (ColorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture = (NormalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

texture DepthMap;
sampler depthSampler = sampler_state
{
    Texture = (DepthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};


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
    //obtain screen position
    input.ScreenPosition.xy /= input.ScreenPosition.w;

    //obtain textureCoordinates corresponding to the current pixel
    //the screen coordinates are in [-1,1]*[1,-1]
    //the texture coordinates need to be in [0,1]*[0,1]
    float2 texCoord = 0.5f * (float2(input.ScreenPosition.x,-input.ScreenPosition.y) + 1);    
    
    //get normal data from the normalMap
    float4 normalData = tex2D(normalSampler,texCoord);
    //tranform normal back into [-1,1] range
    float3 normal = 2.0f * normalData.xyz - 1.0f;
    //get specular power
    float specularPower = normalData.a * 255;
    //get specular intensity from the colorMap
    float specularIntensity = tex2D(colorSampler, texCoord).a;

    //read depth
    float depthVal = tex2D(depthSampler,texCoord).r;

    //compute screen-space position
    float4 position;
    position.xy = input.ScreenPosition.xy;
    position.z = depthVal;
    position.w = 1.0f;
    //transform to world space
    position = mul(position, InvertViewProjection);
    position /= position.w;

    //surface-to-light vector
    float3 lightVector = LightPosition - position.xyz;

    //compute attenuation based on distance - linear attenuation
    float attenuation = saturate(1.0f - length(lightVector)/Radius); 

    //normalize light vector
    lightVector = normalize(lightVector); 

    //compute diffuse light
    float NdL = max(0,dot(normal,lightVector));
    float3 diffuseLight = NdL * Color.rgb;

    //reflection vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));
    //camera-to-surface vector
    float3 directionToCamera = normalize(CameraPosition - position.xyz);
    //compute specular light
    float specularLight = specularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), specularPower);    

    //return float4(Color, specularLight);
    //take into account attenuation and intensity.
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
