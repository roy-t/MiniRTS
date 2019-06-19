// Shader for writing particles to the diffuse target

struct TexturedVertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct TexturedVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

TexturedVertexShaderOutput TexturedMainVS(in TexturedVertexShaderInput input)
{
    TexturedVertexShaderOutput output = (TexturedVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;

    return output;
}

struct TexturedPixelShaderOutput
{
    float4 Color : COLOR0;
};

TexturedPixelShaderOutput TexturedMainPS(TexturedVertexShaderOutput input)
{
    TexturedPixelShaderOutput output = (TexturedPixelShaderOutput)0;    
    output.Color = tex2D(diffuseSampler, input.TexCoord);       

    return output;
}

technique Textured
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL TexturedMainVS();
        PixelShader = compile PS_SHADERMODEL TexturedMainPS();
    }
}
