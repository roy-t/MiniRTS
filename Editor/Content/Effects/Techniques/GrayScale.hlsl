// Shader for rendering particles for color mapping

struct GrayScaleVertexShaderInput
{
    float4 Position : POSITION0; 
    float2 TexCoord : TEXCOORD0;    
};

struct GrayScaleVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;    
};

GrayScaleVertexShaderOutput GrayScaleMainVS(in GrayScaleVertexShaderInput input)
{
    GrayScaleVertexShaderOutput output = (GrayScaleVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;    

    return output;
}

struct GrayScalePixelShaderOutput
{
    float4 Color : COLOR0;
};

GrayScalePixelShaderOutput GrayScaleMainPS(GrayScaleVertexShaderOutput input)
{
    GrayScalePixelShaderOutput output = (GrayScalePixelShaderOutput)0;
    float2 texCoord = input.TexCoord;
    
    output.Color = tex2D(diffuseSampler, texCoord);       
    output.Color.rgb = (output.Color.r + output.Color.g + output.Color.b) / 3.0f;

    return output;
}

technique GrayScale
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL GrayScaleMainVS();
        PixelShader = compile PS_SHADERMODEL GrayScaleMainPS();
    }
}
