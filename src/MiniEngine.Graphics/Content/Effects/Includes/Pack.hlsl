#ifndef __PACK
#define __PACK

float3 PackNormal(float3 normal)
{
    return 0.5f * (normalize(normal) + 1.0f);
}

float3 UnpackNormal(float3 normal)
{
    return normalize((2.0f * normal) - 1.0f);
}

#endif
