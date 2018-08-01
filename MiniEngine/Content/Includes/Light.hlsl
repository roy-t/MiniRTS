// Functions for lighting

float3 ComputeDiffuseLightFactor(float3 lightVector, float3 normal, float3 color)
{
	float NdL = max(0, dot(normal, lightVector));
	return NdL * color.rgb;
}

float ComputeSpecularLightFactor(float3 lightVector, float3 normal, float3 position, float3 cameraPosition, float specularPower, float specularIntensity)
{	
	float3 reflectionVector = normalize(reflect(-lightVector, normal));		
	float3 directionToCamera = normalize(cameraPosition - position.xyz);
	
	float rdot = clamp(dot(reflectionVector, directionToCamera), 0, abs(specularIntensity));
	return pow(abs(rdot), specularPower);
}