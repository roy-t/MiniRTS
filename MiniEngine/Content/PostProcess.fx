#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

static const int NW = 0;
static const int NE = 1;
static const int SW = 2;
static const int SE = 3;
static const int CE = 4;

static const float3 luma = float3(0.299f, 0.587f, 0.114f);

static const float2 samples[] =
{
    float2(-1.0f, -1.0f), // NW
    float2(-1.0f, 1.0f),  // NE
    float2(1.0f, -1.0f),  // SW
    float2(1.0f, 1.0f),   // SE

    float2(0.0f, 0.0f),   // Center
};

float ScaleX;  // 1.0f / renderTarget.Width
float ScaleY;  // 1.0f / renderTarget.Height

bool EnableFXAA = true;

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
    texture = (NormalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

texture DepthMap;
sampler depthSampler = sampler_state
{
    Texture = (DepthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
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
    output.TexCoord = float2(input.TexCoord);

    return output;
}

static float3 getNormal(int direction, float2 texCoord)
{
    float2 offset = float2(samples[direction].x * ScaleX, samples[direction].y * ScaleY);
    float3 normalData = tex2D(normalSampler, texCoord + offset).xyz;
    //tranform normal back into [-1,1] range
    return 2.0f * normalData.xyz - 1.0f;
}

static float3 getRGB(int direction, float2 texCoord)
{
    float2 offset = float2(samples[direction].x * ScaleX, samples[direction].y * ScaleY);
    return tex2D(colorSampler, texCoord + offset).rgb;  
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{   
    const float2 frame = float2(ScaleX, ScaleY);
    const float2 texCoord = input.TexCoord;

    float3 rgbCE = getRGB(CE, texCoord);

    if(EnableFXAA)
    {    
        // Sample color
        float3 rgbNW = getRGB(NW, texCoord);
        float3 rgbNE = getRGB(NE, texCoord);
        float3 rgbSW = getRGB(SW, texCoord);
        float3 rgbSE = getRGB(SE, texCoord);
        
        // sample normals
        float3 normalCE = getNormal(CE, texCoord);
        float diffNW = dot(getNormal(NW, texCoord), normalCE);    
        float diffNE = dot(getNormal(NE, texCoord), normalCE);
        float diffSW = dot(getNormal(SW, texCoord), normalCE);
        float diffSE = dot(getNormal(SE, texCoord), normalCE);

        // Get the min and max values and transform them to the [0..1] range
        float minDiff = (min(diffNW, min(diffNE, min(diffSW, diffSE))) + 1.0f) * 0.5f;
        float maxDiff = (max(diffNW, max(diffNE, max(diffSW, diffSE))) + 1.0f) * 0.5f;
        
        float range = (maxDiff - minDiff);
        
        float borderWeight = clamp((range + 1.0f) / 2.0f, 0, 1) * 0.2f;
        float centerWeight = 1.0f - (borderWeight * 4);

        float3 color = rgbCE * centerWeight + (rgbNW + rgbNE + rgbSW + rgbSE) * borderWeight;
        return float4(color.rgb, 1.0f);        
    }

    return float4(rgbCE.rgb, 1.0f);
}

technique Technique1
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
