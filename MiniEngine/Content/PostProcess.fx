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
static const int CE = 4;

static const float3 luma = float3(0.299, 0.587, 0.114);

static const float2 samples[] =
{
    float2(-1.0f, -1.0f), // NW
    float2(-1.0f, 1.0f),  // NE
    float2(1.0f, -1.0f),  // SW
    float2(1.0f, 1.0f),   // SE

    float2(0.0f, 0.0f),   // Center
};

static const float FXAA_REDUCE_MIN = 1.0f/128.0f;
static const float FXAA_REDUCE_MUL = 1.0f/8.0f;
static const float FXAA_SPAN_MAX = 8.0f;

float getLuma(float3 diffuseColor) 
{
    return dot(diffuseColor, luma);    
}

float3 getRGB(int direction, float2 texCoord)
{
    float2 offset = float2(samples[direction].x * ScaleX, samples[direction].y * ScaleY);
    return tex2D(colorSampler, texCoord + offset).rgb;  
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{   
    float2 posPos = input.TexCoord;
    float2 rcpFrame = float2(ScaleX, ScaleY);

    // Sample color values
    float3 rgbNW = getRGB(NW, input.TexCoord);
    float3 rgbNE = getRGB(NE, input.TexCoord);
    float3 rgbSW = getRGB(SW, input.TexCoord);
    float3 rgbSE = getRGB(SE, input.TexCoord);
    float3 rgbCE = getRGB(CE, input.TexCoord);

    // Compute luma values
    float lumaNW = getLuma(rgbNW);
    float lumaNE = getLuma(rgbNE);
    float lumaSW = getLuma(rgbSW);
    float lumaSE = getLuma(rgbSE);
    float lumaCE = getLuma(rgbCE);  

    float lumaMin = min(lumaCE, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaCE, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

    // Compute luma slope
    float2 dir = float2
    (
        -((lumaNW + lumaNE) - (lumaSW + lumaSE)),
        ((lumaNW + lumaSW) - (lumaNE + lumaSE))
    );

    float dirReduce = max
    (
        (lumaNW + lumaNE + lumaSW + lumaSE) * (0.25f * FXAA_REDUCE_MUL),
        FXAA_REDUCE_MIN
    );

    float rcpDirMin = 1.0f / (min(abs(dir.x), abs(dir.y)) + dirReduce);

    dir = min(float2(FXAA_SPAN_MAX,  FXAA_SPAN_MAX),
          max(float2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX), 
          dir * rcpDirMin)) * rcpFrame.xy;

    float3 rgbA = (1.0f/2.0f) * (
        tex2D(colorSampler, posPos.xy + dir * (1.0f/3.0f - 0.5f)).xyz +
        tex2D(colorSampler, posPos.xy + dir * (2.0f/3.0f - 0.5f)).xyz);

    float3 rgbB = rgbA * (1.0f/2.0f) + (1.0f/4.0f) * (
        tex2D(colorSampler, posPos.xy + dir * (0.0f/3.0f - 0.5f)).xyz +
        tex2D(colorSampler, posPos.xy + dir * (3.0f/3.0f - 0.5f)).xyz);

    float lumaB = getLuma(rgbB);

    if((lumaB < lumaMin) || (lumaB > lumaMax))
    {
        return float4(rgbA, 1.0f);
    } 

    return float4(rgbB, 1.0f);
}

technique Technique1
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
