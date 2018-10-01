// Clears the geometry-buffer

#include "Includes/Defines.hlsl"

struct VertexShaderInput
{
    float3 Position : SV_POSITION;	
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;	
};

struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float4 Depth : COLOR2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Position = float4(input.Position, 1);	
    return output;
}

PixelShaderOutput MainPS(VertexShaderOutput input)
{
    PixelShaderOutput output = (PixelShaderOutput)0;

    // Clear the Color buffer, which stores the diffuse color in .rgb and nothing in .a	
    output.Color.rgb = float3(0, 0, 0);
    output.Color.a = 0.0f;

    // Clear the Normal buffer, which stores the normal in .rgb and the specular power in .a	
    // (When transforming 0.5f into [-1, 1], we will get 0.0f)
    output.Normal.rgb = 0.5f;
    // Set specular power to zero
    output.Normal.a = 0.0f;

    // Set the depth to the zero (max distance)
    output.Depth = 0.0f;

    return output;
}

technique ClearTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};