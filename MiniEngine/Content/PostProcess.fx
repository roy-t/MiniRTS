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

static const float2 samples[] =
{
    float2(-1.0f, -1.0f),
    float2(-1.0f, 1.0f),
    float2(1.0f, -1.0f),
    float2(1.0f, 1.0f),

    float2(0.0f, 0.0f),
};

float getLuma(float3 diffuseColor) 
{
    float r = diffuseColor.r;
    float g = diffuseColor.g;
    float b = diffuseColor.b;

    return 0.375f * r + 0.5f * g + 0.125f * b;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{    
    float lumaMin = 100.0f;
    float lumaMax = 0.0f;

    for(int i = 0; i < 5; i++)
    {
        float2 offset = float2(samples[i].x * ScaleX, samples[i].y * ScaleY);
        float3 diffuseColor = tex2D(colorSampler, input.TexCoord + offset).rgb;  

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
