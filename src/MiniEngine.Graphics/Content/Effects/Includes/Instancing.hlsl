#ifndef __INSTANCING
#define __INSTANCING

struct InstancingData
{
    float4x4 Offset : TEXCOORD1;
};

struct ParticleInstancingData
{
    float4x4 Offset : TEXCOORD1;
    float4 Color : TEXCOORD5;
};

#endif
