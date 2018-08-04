// Samplers used as input by shaders in the deferred rendering pipeline
// and helper functions to read the data from these samplers.

texture ColorMap;
sampler colorSampler = sampler_state
{
	Texture = (ColorMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
	Texture = (NormalMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

texture DepthMap;
sampler depthSampler = sampler_state
{
	Texture = (DepthMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};


float3 ReadNormals(float2 texCoord)
{
	// Get normal data from the NormalMap	
	float4 data = tex2D(normalSampler, texCoord);
	//tranform normal back into [-1,1] range
	return 2.0f * data.xyz - 1.0f;
}

float ReadShininess(float2 texCoord)
{
	return tex2D(normalSampler, texCoord).a;	
}

float4 ReadWorldPosition(float2 texCoord, float4x4 inverseViewProjection)
{
	// Read depth
	float depthVal = tex2D(depthSampler, texCoord).r;

	// Compute screen-space position
	float4 position;
	position.x = texCoord.x * 2.0f - 1.0f;
	position.y = -(texCoord.y * 2.0f - 1.0f);
	position.z = depthVal;
	position.w = 1.0f;
	
	// Transform to world space
	position = mul(position, inverseViewProjection);
	position /= position.w;

	return position;
}