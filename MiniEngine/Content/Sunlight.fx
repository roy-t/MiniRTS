#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

float4x4 InverseViewProjection;
float4x4 LightView;
float4x4 LightProjection;

float3 LightDirection;
float3 LightColor;
float3 LightPosition;

float3 CameraPosition;

// Shadow stuff
static const float Bias = 0.01f;
static const float OffsetScale = 0.0f;
static const uint NumCascades = 4;

float4x4 ShadowMatrix;
float4 CascadeSplits;
float4 CascadeOffsets[NumCascades];
float4 CascadeScales[NumCascades];

// debug stuff
bool visualizeCascades = false;

Texture2DArray ShadowMap : register(t0);
SamplerComparisonState ShadowSampler : register(s0);

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

    output.Position = float4(input.Position, 1);
    //align texture coordinates
    output.TexCoord = input.TexCoord;
    return output;
}

float SampleShadowMap(float3 shadowPosition, float3 shadowPosDX, float3 shadowPosDY, uint cascadeIndex)
{

    float lightDepth = shadowPosition.z -Bias;	    
    return ShadowMap.SampleCmpLevelZero(ShadowSampler, float3(shadowPosition.xy, cascadeIndex), lightDepth);
}


float3 SampleShadowCascade(float3 shadowPosition, float3 shadowPosDX, float3 shadowPosDY, uint cascadeIndex)
{
    shadowPosition += CascadeOffsets[cascadeIndex].xyz;
    shadowPosition *= CascadeScales[cascadeIndex].xyz;

    shadowPosDX *= CascadeScales[cascadeIndex].xyz;
    shadowPosDY *= CascadeScales[cascadeIndex].xyz;

    if (visualizeCascades) {

        const float3 CascadeColors[NumCascades] =
        {
            float3(1.0f, 0.0f, 0.0f),
            float3(0.0f, 1.0f, 0.0f),
            float3(0.0f, 0.0f, 1.0f),
            float3(1.0f, 1.0f, 0.0f)
        };

        return CascadeColors[cascadeIndex];
    }

    float shadow = SampleShadowMap(shadowPosition, shadowPosDX, shadowPosDY, cascadeIndex);
    return float3(shadow, shadow, shadow);
}

float3 GetLightFactor(float3 positionWS, float depthVS, float2 texCoord)
{
    float3 shadowVisibility = float3(1.0f, 1.0f, 1.0f);
    uint cascadeIndex = 0;

    [unroll]
    for (uint i = 0; i < NumCascades - 1; ++i)
    {
        [flatten]
        if (depthVS > CascadeSplits[i])
            cascadeIndex = i + 1;
    }

    // Project into shadow space
    float3 samplePos = positionWS;
    float3 shadowPosition = mul(float4(samplePos, 1.0f), ShadowMatrix).xyz;
    float3 shadowPosDX = ddx_fine(shadowPosition);
    float3 shadowPosDY = ddy_fine(shadowPosition);

    shadowVisibility = SampleShadowCascade(shadowPosition, shadowPosDX, shadowPosDY, cascadeIndex);

    return shadowVisibility;
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

    //transform to world space, store view space distance
    position = mul(position, InverseViewProjection);
    float distance = depthVal / position.w;
    position /= position.w;
    
    float3 lightFactor = GetLightFactor(position.xyz, distance, input.TexCoord);
    
    //surface-to-light vector
    float3 lightVector = -normalize(LightDirection);

    //compute diffuse light
    float NdL = saturate(dot(normal, lightVector));
    float3 diffuseLight = NdL * LightColor.rgb;

    //reflection vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));
    //camera-to-surface vector
    float3 directionToCamera = normalize(CameraPosition - position.xyz);
    //compute specular light
    float rdot = clamp(dot(reflectionVector, directionToCamera), 0, abs(specularIntensity));
    float specularLight = pow(abs(rdot), specularPower);

    return float4(diffuseLight.rgb * lightFactor, specularLight * lightFactor.r);

    float4 special = float4(diffuseLight.rgb * lightFactor, specularLight * lightFactor.r);
    //return float4(diffuseLight.rgb, specularLight) +special * 0.000001f;    
    return float4(lightFactor.x, lightFactor.y, lightFactor.z, 1.0f) + (special * 0.0001f);
}

technique DirectionalLightTechnique
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}