float3 UnpackNormal(float3 normal)
{
    return (2.0f * normal) - 1.0f;
}

float3 PackNormal(float3 normal)
{
    return 0.5f * (normal + 1.0f);
}