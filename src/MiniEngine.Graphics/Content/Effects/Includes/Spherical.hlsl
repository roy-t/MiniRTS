float2 SampleSphericalMap(float3 position)
{
    float azimuth = atan2(position.x, position.z);
    float zenith = asin(position.y);

    float u = (azimuth * ONE_OVER_TWO_PI + 0.5f);
    float v = 1.0f - (zenith * ONE_OVER_PI + 0.5f);

    return float2(u, v);
}