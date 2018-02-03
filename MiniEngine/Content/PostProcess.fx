#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float ScaleX;
float ScaleY;

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

static const int NW = 0;
static const int NE = 1;
static const int SW = 2;
static const int SE = 3;
static const int CENTER = 4;

static const float2 samples[] =
{
    float2(-1.0f, -1.0f), // NW
    float2(-1.0f, 1.0f),  // NE
    float2(1.0f, -1.0f),  // SW
    float2(1.0f, 1.0f),   // SE

    float2(0.0f, 0.0f),   // Center
};

float getLuma(float3 diffuseColor) 
{
    static const float3 luma = float3(0.299, 0.587, 0.114);
    return dot(diffuseColor, luma);    
}

float3 getRGB(float2 textCoord, int direction)
{
    float2 offset = float2(samples[direction].x * ScaleX, samples[direction].y * ScaleY);
    return tex2D(colorSampler, textCoord + offset).rgb;  
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{    
    float lumaMin = 100.0f;
    float lumaMax = 0.0f;    

    for(int i = 0; i < 5; i++)
    {        
        float3 diffuseColor = getRGB(input.TexCoord, i);
        float luma = getLuma(diffuseColor);            

        lumaMin = min(lumaMin, luma);
        lumaMax = max(lumaMax, luma);
    }    

    float range = lumaMax - lumaMin;

    return float4(range, range, range, 1);
}

technique Technique1
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
