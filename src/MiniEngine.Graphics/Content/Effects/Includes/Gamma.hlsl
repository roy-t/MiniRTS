#ifndef __GAMMA
#define __GAMMA

static const float GAMMA = 2.2f;
static const float INVERSE_GAMMA = 0.45454545455f;

static float4 ToLinear(float4 v)
{
    float3 rgb = pow(abs(v.rgb), float3(GAMMA, GAMMA, GAMMA));
    return float4(rgb.rgb, v.a);
}

static float4 ToGamma(float4 v)
{
    float3 rgb = pow(abs(v.rgb), float3(INVERSE_GAMMA, INVERSE_GAMMA, INVERSE_GAMMA));
    return float4(rgb.rgb, v.a);
}

#endif
