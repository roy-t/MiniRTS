struct CMVertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct CMVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

CMVertexShaderOutput CMMainVS(in CMVertexShaderInput input)
{
    CMVertexShaderOutput output = (CMVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 CMMainPS(CMVertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;

    float4 color = tex2D(diffuseSampler, texCoord);
    color.rgb *= (1.0f - color.a);
    return color;
}

technique ColorMap
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL CMMainVS();
        PixelShader = compile PS_SHADERMODEL CMMainPS();
    }
}
