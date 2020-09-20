#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

matrix WorldViewProjection;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord: TEXCOORD0;
};

float SampleRadius = 0.04f;

static float2 Offsets[16] =
{
	float2(0.2770745f, 0.6951455f),
	float2(0.1874257f, -0.02561589f),
	float2(-0.3381929f, 0.8713168f),
	float2(0.5867746f, 0.1087471f),
	float2(-0.3078699f, 0.188545f),
	float2(0.7993396f, 0.4595091f),
	float2(-0.09242552f, 0.5260149f),
	float2(0.3657553f, -0.5329605f),
	float2(-0.3829718f, -0.2476171f),
	float2(-0.01085108f, -0.6966301f),
	float2(0.8404155f, -0.3543923f),
	float2(-0.5186161f, -0.7624033f),
	float2(-0.8135794f, 0.2328489f),
	float2(-0.784665f, -0.2434929f),
	float2(0.9920505f, 0.0855163f),
	float2(-0.687256f, 0.6711345f)
};

texture Texture;
sampler diffuseSampler = sampler_state
{
	Texture = (Texture);
	MinFilter = ANISOTROPIC;
	MagFilter = ANISOTROPIC;
	MipFilter = LINEAR;
	MaxAnisotropy = 16;
	AddressU = Clamp;
	AddressV = Clamp;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 TexturePS(VertexShaderOutput input) : COLOR
{
	return tex2D(diffuseSampler, input.TexCoord);
}

float4 BlurrPS(VertexShaderOutput input) : COLOR
{
	float sum = 0;
	float4 color = float4(0, 0, 0, 0);
	for (int i = 0; i < 16; i++)
	{
		float2 tc = input.TexCoord;
		tc.x += (Offsets[i].x * SampleRadius);
		tc.y += (Offsets[i].y * SampleRadius);

		sum += 1.0f;
		color += tex2D(diffuseSampler, tc);
	}

	float4 average = color / sum;
	return average;
}

static const float gamma = 2.2f;
static const float inverseGamma = 0.45454545455;

static float4 ToLinear(float4 v)
{
	float3 rgb = pow(abs(v.rgb), float3(gamma, gamma, gamma));
	return float4(rgb.rgb, v.a);
}

static float4 ToGamma(float4 v)
{
	float3 rgb = pow(abs(v.rgb), float3(inverseGamma, inverseGamma, inverseGamma));
	return float4(rgb.rgb, v.a);
}

float4 BlurrLinearPS(VertexShaderOutput input) : COLOR
{
	float sum = 0;
	float4 color = float4(0, 0, 0, 0);
	for (int i = 0; i < 16; i++)
	{
		float2 tc = input.TexCoord;
		tc.x += (Offsets[i].x * SampleRadius);
		tc.y += (Offsets[i].y * SampleRadius);

		sum += 1.0f;
		color += ToLinear(tex2D(diffuseSampler, tc));
	}

	float4 average = color / sum;
	return ToGamma(average);
}

technique Textured
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL TexturePS();
	}
};

technique Blurred
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL BlurrPS();
	}
};

technique BlurredInLinearSpace
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL BlurrLinearPS();
	}
};