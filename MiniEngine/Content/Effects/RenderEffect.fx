#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/Pack.hlsl"

texture Texture;
sampler diffuseSampler = sampler_state
{
    Texture = (Texture);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture SpecularMap;
sampler specularSampler = sampler_state
{
    Texture = (SpecularMap);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture = (NormalMap);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Mask;
sampler maskSampler = sampler_state
{
    Texture = (Mask);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;    
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz,1), World);
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

struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float4 Depth : COLOR2;
};

PixelShaderOutput MainPS(VertexShaderOutput input)
{
    PixelShaderOutput output = (PixelShaderOutput)0;
    float2 texCoord = input.TexCoord;

    float mask = tex2D(maskSampler, texCoord).r;
    if (mask < 0.5f)
    {
        clip(-1);
        return output;
    }

    // Diffuse
    output.Color = tex2D(diffuseSampler, texCoord);    

    // Normal   
    float3 normal = UnpackNormal(tex2D(normalSampler, texCoord).xyz);
    normal = normalize(mul(normal, input.tangentToWorld));
    output.Normal.rgb = PackNormal(normal);

    // Specular
    float specularPower = tex2D(specularSampler, texCoord).r;

    // Shininess is stored in textures with black is most shiney, and white is non-shiney
    // make 1.0f most shiney here
    output.Normal.a = 1.0f - specularPower;

    output.Depth = input.Depth.x / input.Depth.y;

    return output;
}

technique RenderTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
