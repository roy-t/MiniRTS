// Helper functions

// Maps Screen coordinates [-1, 1] with y is up to texture coordinates [0, 1] with y is down
float2 ToTextureCoordinates(float2 screenCoordinates, float w)
{
    return 0.5f * (float2(screenCoordinates.x / w, -screenCoordinates.y / w) + 1);
}

// Computes the world position without sampling any texture, see GBuffer.hlsl 
// for when you do not yet have the depth
float4 ReadWorldPosition(float2 texCoord, float depth, float4x4 inverseViewProjection)
{
    // Compute screen-space position
    float4 position;
    position.x = texCoord.x * 2.0f - 1.0f;
    position.y = -(texCoord.y * 2.0f - 1.0f);
    position.z = depth;
    position.w = 1.0f;

    // Transform to world space
    position = mul(position, inverseViewProjection);
    position /= position.w;

    return position;
}

float4 SampleSkybox(samplerCUBE skyboxSampler, float3 cameraPosition, float4 screenPosition, float4x4 inverseViewProjection, float depth, float3 normal)
{
    float2 texCoord = ToTextureCoordinates(screenPosition.xy, screenPosition.w);
    float4 position = ReadWorldPosition(texCoord, depth, inverseViewProjection);

    float3 viewDirection = cameraPosition - position.xyz;

    float3 reflection = reflect(-normalize(viewDirection), normal);
    return texCUBE(skyboxSampler, normalize(reflection));
}