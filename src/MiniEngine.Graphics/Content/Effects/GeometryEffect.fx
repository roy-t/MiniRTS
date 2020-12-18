#include "Includes/Defines.hlsl"
#include "Includes/Pack.hlsl"
#include "Includes/Gamma.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float3 Normal : NORMAL0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float4 WorldPosition: TEXCOORD0;
    float2 Texture : TEXCOORD1;
    float3 Normal : NORMAL0;
};

struct OutputData
{
    float4 Albedo : COLOR0;
    float4 Material : COLOR1;
    float Depth : COLOR2;
    float4 Normal: COLOR3;
};

texture Albedo;
sampler albedoSampler = sampler_state
{
    Texture = (Albedo);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Normal;
sampler normalSampler = sampler_state
{
    Texture = (Normal);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

float3 CameraPosition;
float4x4 World;
float4x4 WorldViewProjection;
float Metalicness;
float Roughness;
float AmbientOcclusion;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Texture = input.Texture;
    output.WorldPosition = output.Position;

    float3x3 rotation = (float3x3)World;
    output.Normal = normalize(mul(input.Normal, rotation));

    return output;
}

// Normal mapping as described by Christian Schüler in
// http://www.thetenthplanet.de/archives/1180
float3x3 CotangentFrame(float3 N, float3 p, float2 uv)
{
    // get edge vectors of the pixel triangle
    float3 dp1 = ddx(p);
    float3 dp2 = ddy(p);
    float2 duv1 = ddx(uv);
    float2 duv2 = ddy(uv);

    // solve the linear system
    float3 dp2perp = cross(dp2, N);
    float3 dp1perp = cross(N, dp1);
    float3 T = dp2perp * duv1.x + dp1perp * duv2.x;
    float3 B = dp2perp * duv1.y + dp1perp * duv2.y;

    // construct a scale-invariant frame
    float invmax = rsqrt(max(dot(T, T), dot(B, B)));
    return float3x3(T * invmax, B * invmax, N);
}

float3 PerturbNormal(float3 normal, float3 view, float2 uv)
{
    float3 map = UnpackNormal(tex2D(normalSampler, uv).xyz);
    float3x3 tbn = CotangentFrame(normal, -view, uv);
    return mul(map, tbn);
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    float4 albedo = tex2D(albedoSampler, input.Texture);
    clip(albedo.a - 1);

    output.Albedo = ToLinear(albedo);
    output.Material = float4(Metalicness, Roughness, AmbientOcclusion, 1.0f);
    output.Depth = input.WorldPosition.z / input.WorldPosition.w;

    float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
    float3 normal = PerturbNormal(input.Normal, V, input.Texture);
    output.Normal = float4(PackNormal(normal), 1.0f);

    return output;
}

technique GeometryTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
