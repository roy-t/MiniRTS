float CalculateAttenuation(float3 lightPosition, float3 worldPosition)
{
    float dist = distance(lightPosition, worldPosition);
    return 1.0f / (dist * dist);
}

float ScaleLight(float3 lightPosition, float3 worldPosition, float3 normal)
{
    float3 lightVector = normalize(lightPosition - worldPosition);
    return  max(dot(normal, lightVector), 0.0f);
}

float3 CalculateRadiance(float3 light, float attenuation, float scale)
{
    return light * attenuation * scale;
}