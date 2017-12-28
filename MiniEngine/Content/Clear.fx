#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VertexShaderInput
{
	float3 Position : SV_POSITION;	
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;	
};

struct PixelShaderOutput
{
	float4 Color : COLOR0;
	float4 Normal : COLOR1;
	float4 Depth : COLOR2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.Position = float4(input.Position, 1);	
	return output;
}

PixelShaderOutput MainPS(VertexShaderOutput input)
{
	PixelShaderOutput output = (PixelShaderOutput)0;

	// black color
	output.Color.rgb = 0.0f;
	output.Color.a = 0.0f;

	// When transforming 0.5f into [-1, 1], we will get 0.0f;
	output.Normal.rgb = 0.5f;
	// Set specular power to zero
	output.Normal.a = 0.0f;

	// Max depth
	output.Depth = 0.0f;

	return output;
}

technique ClearTechnique
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};