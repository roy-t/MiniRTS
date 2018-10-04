// Shader for writing particles to the diffuse target

struct ParticleVertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct ParticleVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

ParticleVertexShaderOutput ParticleMainVS(in ParticleVertexShaderInput input)
{
    ParticleVertexShaderOutput output = (ParticleVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;

    return output;
}

struct ParticlePixelShaderOutput
{
    float4 Color : COLOR0;
};

ParticlePixelShaderOutput ParticleMainPS(ParticleVertexShaderOutput input)
{
    ParticlePixelShaderOutput output = (ParticlePixelShaderOutput)0;
    float2 texCoord = input.TexCoord;

    // Diffuse    
    output.Color = tex2D(diffuseSampler, texCoord);       
    return output;
}

technique Particles
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL ParticleMainVS();
        PixelShader = compile PS_SHADERMODEL ParticleMainPS();
    }
}
