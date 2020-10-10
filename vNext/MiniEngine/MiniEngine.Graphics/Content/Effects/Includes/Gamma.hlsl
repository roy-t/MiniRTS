static const float gamma = 2.2f;
static const float inverseGamma = 0.45454545455;

static float4 ToLinear(float4 v)
{
	float3 rgb = pow(abs(v.rgb), float3(gamma, gamma, gamma));
	return float4(rgb.rgb, v.a);
}

static float4 ToGamma(float4 v)
{
	float3 rgb = pow(abs(v.rgb), float3(inverseGamma, inverseGamma, inverseGamma));
	return float4(rgb.rgb, v.a);
}