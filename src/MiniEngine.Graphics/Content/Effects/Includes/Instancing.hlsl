#ifndef __INSTANCING
#define __INSTANCING

struct InstancingData
{
    float4x4 Offset : TEXCOORD1;
};

struct Particle
{
    float2 UV: POSITION1;
};

#endif
