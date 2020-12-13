// The distribution function compute the chance that a micro facet is pointing in the direction
// of the halfway vector. Remember that if a micro facet points in the direction of the halfway
// vector it would cause light to be reflected into the direction of the view vector. Which
// would lead to a stronger specular reflection. The rougher the surface is the smaller this
// chance will be.
float DistributionGGX(float3 N, float3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float nom = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / max(denom, EPSILON); // prevent divide by zero for roughness=0.0 and NdotH=1.0
}

// Distribution function for GeometrySmith
float GeometrySchlickGGX(float NdotV, float roughness, float k)
{
    float nom = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;
}

// Models the probability that micro facets are shadowing each other. This leads to the light
// needing multiple bounces before it can reach the viewer. Each bounce costing a little bit of
// energy. Rougher surfaces have a higher chance of self-shadowing and will thus appear darker.
float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);

    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float ggx2 = GeometrySchlickGGX(NdotV, roughness, k);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness, k);

    return ggx1 * ggx2;
}

// GeometrySmith function for Image Based Lighting, here k is rescaled differently. I could not find
// an explanation why the rescaling is different.
float GeometrySmithIBL(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);

    float r = roughness;
    float k = (r * r) / 2.0f;

    float ggx2 = GeometrySchlickGGX(NdotV, roughness, k);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness, k);

    return ggx1 * ggx2;
}

// Compute the precentage of the light that gets reflected (instead of refracted) based on the
// viewing angle. At a more extreme angle more light is reflected than refracted. Which leads to a
// sort of outline effect.
float3 FresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

// Fresnel approximation that takes roughness into account, for image based lighting
float3 FresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    float invRoughness = 1.0 - roughness;
    return F0 + (max(float3(invRoughness, invRoughness, invRoughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}
