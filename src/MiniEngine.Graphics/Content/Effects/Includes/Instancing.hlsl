#ifndef __INSTANCING
#define __INSTANCING

struct InstancingData
{
    float4x4 Offset : TEXCOORD1;
};

struct ParticleInstancingData
{
    float3 Position: POSITION1;
    float Color : COLOR1;
    float Scale : TEXCOORD1;
};

#endif
