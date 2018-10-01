#include "Includes/Defines.hlsl"
#include "Includes/GBuffer.hlsl"

static const int NW = 0;
static const int NE = 1;
static const int SW = 2;
static const int SE = 3;
static const int CE = 4;

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

float Strength = 2.0f;

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
    return ReadNormals(texCoord + offset);       
}

static float4 getDiffuse(int direction, float2 texCoord)
{
    float2 offset = float2(samples[direction].x * ScaleX, samples[direction].y * ScaleY);
    return ReadDiffuse(texCoord + offset);    
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{   
    const float2 frame = float2(ScaleX, ScaleY);
    const float2 texCoord = input.TexCoord;
    
    // Sample color
    float4 rgbCE = getDiffuse(CE, texCoord);   
    float4 rgbNW = getDiffuse(NW, texCoord);
    float4 rgbNE = getDiffuse(NE, texCoord);
    float4 rgbSW = getDiffuse(SW, texCoord);
    float4 rgbSE = getDiffuse(SE, texCoord);
    
    // sample normals
    float3 normalCE = getNormal(CE, texCoord);
    float dotNW = dot(getNormal(NW, texCoord), normalCE);    
    float dotNE = dot(getNormal(NE, texCoord), normalCE);
    float dotSW = dot(getNormal(SW, texCoord), normalCE);
    float dotSE = dot(getNormal(SE, texCoord), normalCE);

    // dot(v1, v2) == 1 if the vectors point in the same direction
    // dot(v1, v2) == -1 if the vectors point in opposite directions
    float mindot = min(dotNW, min(dotNE, min(dotSW, dotSE)));
    float maxdot = max(dotNW, max(dotNE, max(dotSW, dotSE)));
    
    // from [0..2] to [0..1]
    float range = (maxdot - mindot) * 0.5f;
    
    float borderWeight = clamp(range * Strength, 0, 1) * 0.2f;
    float centerWeight = 1.0f - (borderWeight * 4);
       
    return rgbCE * centerWeight + (rgbNW + rgbNE + rgbSW + rgbSE) * borderWeight;    
}

technique FXAA
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
