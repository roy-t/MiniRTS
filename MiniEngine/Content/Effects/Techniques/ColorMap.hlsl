// Similar to the shadow map effect, but instead of distance this effect maps all colors visible by the observer
// Used for transparency effects (like light shining through stained glass)

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

struct CMPixelShaderOutput
{
    float4 Color : COLOR0;
};

CMPixelShaderOutput CMMainPS(CMVertexShaderOutput input)
{
    CMPixelShaderOutput output = (CMPixelShaderOutput)0;

    float2 texCoord = input.TexCoord;

    output.Color = tex2D(diffuseSampler, texCoord);
    output.Color.rgb *= (1.0f - output.Color.a);
    
    return output;
}

technique ColorMap
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL CMMainVS();
        PixelShader = compile PS_SHADERMODEL CMMainPS();
    }
}
