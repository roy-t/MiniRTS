// Stores the color, normal and depth information in three separate render targets
// to construct a geometry-buffer.

struct ParticleVertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};

struct ParticleVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;
};

ParticleVertexShaderOutput ParticleMainVS(in ParticleVertexShaderInput input)
{
    ParticleVertexShaderOutput output = (ParticleVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(float4(input.Tangent, 0), World).xyz;
    output.tangentToWorld[1] = mul(float4(input.Binormal, 0), World).xyz;
    output.tangentToWorld[2] = mul(float4(input.Normal, 0), World).xyz;

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

technique Particle
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL ParticleMainVS();
        PixelShader = compile PS_SHADERMODEL ParticleMainPS();
    }
}
