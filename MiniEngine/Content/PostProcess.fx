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

float FXAA_REDUCE_MIN = 1.0f/128.0f;
float FXAA_REDUCE_MUL = 1.0f/8.0f;
float FXAA_SPAN_MAX = 8.0f;

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

static float toLuma(float3 diffuseColor) 
{
    return dot(diffuseColor, luma);    
}

static float3 getRGB(int direction, float2 texCoord)
{
    float2 offset = float2(samples[direction].x * ScaleX, samples[direction].y * ScaleY);
    return tex2D(colorSampler, texCoord + offset).rgb;  
}

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;    
    output.Position = float4(input.Position,1);
    output.TexCoord = float2(input.TexCoord);

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{   
    const float2 frame = float2(ScaleX, ScaleY);
    const float2 texCoord = input.TexCoord;
    
    // Sample color values
    float3 rgbNW = getRGB(NW, texCoord);
    float3 rgbNE = getRGB(NE, texCoord);
    float3 rgbSW = getRGB(SW, texCoord);
    float3 rgbSE = getRGB(SE, texCoord);
    float3 rgbCE = getRGB(CE, texCoord);

    // Compute luma values
    float lumaNW = toLuma(rgbNW);
    float lumaNE = toLuma(rgbNE);
    float lumaSW = toLuma(rgbSW);
    float lumaSE = toLuma(rgbSE);
    float lumaCE = toLuma(rgbCE);  

    float lumaMin = min(lumaCE, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaCE, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

    // Compute sampling direction based on luma slope
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

    float dirMin = 1.0f / (min(abs(dir.x), abs(dir.y)) + dirReduce);

    dir = min(float2(FXAA_SPAN_MAX,  FXAA_SPAN_MAX),
          max(float2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX), 
          dir * dirMin)) * frame.xy;

    float3 rgbA = (1.0f/2.0f) * (
        tex2D(colorSampler, texCoord.xy + dir * (1.0f/3.0f - 0.5f)).xyz +
        tex2D(colorSampler, texCoord.xy + dir * (2.0f/3.0f - 0.5f)).xyz);

    float3 rgbB = rgbA * (1.0f/2.0f) + (1.0f/4.0f) * (
        tex2D(colorSampler, texCoord.xy + dir * (0.0f/3.0f - 0.5f)).xyz +
        tex2D(colorSampler, texCoord.xy + dir * (3.0f/3.0f - 0.5f)).xyz);

    float lumaB = toLuma(rgbB);

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
