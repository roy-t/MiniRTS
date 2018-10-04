// Shader for rendering particles for color mapping

struct ShadowParticlesVertexShaderInput
{
    float4 Position : POSITION0; 
    float2 TexCoord : TEXCOORD0;    
};

struct ShadowParticlesVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;    
};

ShadowParticlesVertexShaderOutput ShadowParticlesMainVS(in ShadowParticlesVertexShaderInput input)
{
    ShadowParticlesVertexShaderOutput output = (ShadowParticlesVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;    

    return output;
}

struct ShadowParticlesPixelShaderOutput
{
    float4 Color : COLOR0;
};

ShadowParticlesPixelShaderOutput ShadowParticlesMainPS(ShadowParticlesVertexShaderOutput input)
{
    ShadowParticlesPixelShaderOutput output = (ShadowParticlesPixelShaderOutput)0;
    float2 texCoord = input.TexCoord;
    
    output.Color = tex2D(diffuseSampler, texCoord);       
    output.Color.rgb = (output.Color.r + output.Color.g + output.Color.b) / 3.0f;

    return output;
}

technique ShadowParticles
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL ShadowParticlesMainVS();
        PixelShader = compile PS_SHADERMODEL ShadowParticlesMainPS();
    }
}
