// Functions for lighting

float3 ComputeDiffuseLightFactor(float3 lightVector, float3 normal, float3 color)
{
	float NdL = max(0, dot(normal, lightVector));
	return NdL * color.rgb;
}

float ComputeSpecularLightFactor(float3 lightVector, float3 normal, float4 position, float3 cameraPosition, float shininess, float specularPower)
{		
	float3 reflectionVector = normalize(reflect(-lightVector, normal));
	float3 surfaceToCamera = normalize(cameraPosition - position.xyz);
	float cosAngle = saturate(dot(reflectionVector, surfaceToCamera));	

	return pow(cosAngle, specularPower) * shininess;
}