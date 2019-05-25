#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"

float4x4 ProjectorViewProjection;
float4 Tint;
float3 ProjectorPosition;
float3 ProjectorForward;
float MaxDistance;
float FadeLength;

Texture2D ProjectorMap;
sampler projectorSampler = sampler_state
{
    Texture = (ProjectorMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
};

Texture2D Mask;
sampler maskSampler= sampler_state
{
    Texture = (Mask);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = ANISOTROPIC;
};

struct VertexShaderInput
{
    float3 Position : POSITION0;    
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;    
    float4 ScreenPosition : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);    
    output.ScreenPosition = output.Position;

    return output;
}

// Scalar that indicates how close to an edge a texture coordinate is [0..1]
static float TextureEdge(float2 uv)
{
    // Convert to [-1, 1] domain
    float2 norm = (uv - 0.5f) * 2.0f;
    
    return max(abs(norm.x), abs(norm.y));
}


float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = ToTextureCoordinates(input.ScreenPosition.xy, input.ScreenPosition.w);
    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);

    // Move from world position to the reference frame of the projector
    float4 positionInProjectorReferenceFrame = mul(position, ProjectorViewProjection);

    // Figure out where on the projector map the current pixel is
    float2 projectorMapCoordinates = ToTextureCoordinates(positionInProjectorReferenceFrame.xy, positionInProjectorReferenceFrame.w);
        
    // Distance between pixel and projector
    float dist = distance(ProjectorPosition, position.xyz);

    // Angle between pixel and projector, dir > 0 means the pixel is in front of the projector
    // while dir < 0 means its behind it.
    float3 direction = normalize(position.xyz - ProjectorPosition);
    float dir = dot(ProjectorForward, direction);

    // Only apply the projector if the it is inside the bounds of the projector texture, close enough, and in front of the projector
    if (dir > 0 && dist < MaxDistance &&
        projectorMapCoordinates.x >= 0.0f && projectorMapCoordinates.x <= 1.0f &&
        projectorMapCoordinates.y >= 0.0f && projectorMapCoordinates.y <= 1.0f)
    {   
        // [0..1] how close the closest boundary is where 1 means the pixel is at the boundary
        float closestBoundary = max(dist / MaxDistance, TextureEdge(projectorMapCoordinates));
        float fadeOut = clamp(FadeLength - (1.0f - closestBoundary), 0, FadeLength);                

        float mask = tex2D(maskSampler, projectorMapCoordinates).r;
        return tex2D(projectorSampler, projectorMapCoordinates) * Tint * mask * fadeOut;
    }
    
    return float4(0, 0, 0, 0);
}

// Copied MainPS that displays texture coordinates when out of projector space
float4 OverdrawPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = ToTextureCoordinates(input.ScreenPosition.xy, input.ScreenPosition.w);
    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);

    // Move from world position to the reference frame of the projector
    float4 positionInProjectorReferenceFrame = mul(position, ProjectorViewProjection);

    // Figure out where on the projector map the current pixel is
    float2 projectorMapCoordinates = ToTextureCoordinates(positionInProjectorReferenceFrame.xy, positionInProjectorReferenceFrame.w);

    // Distance between pixel and projector
    float dist = distance(ProjectorPosition, position.xyz);

    // Angle between pixel and projector, dir > 0 means the pixel is in front of the projector
    // while dir < 0 means its behind it.
    float3 direction = normalize(position.xyz - ProjectorPosition);
    float dir = dot(ProjectorForward, direction);


    // Only apply the projector if the it is inside the bounds of the projector texture, close enough, and in front of the projector
    if (dir > 0 && dist < MaxDistance &&
        projectorMapCoordinates.x >= 0.0f && projectorMapCoordinates.x <= 1.0f &&
        projectorMapCoordinates.y >= 0.0f && projectorMapCoordinates.y <= 1.0f)
    {
        float nMax = 1.0f - max(dist / MaxDistance, TextureEdge(projectorMapCoordinates));

        float fadeOut = 1.0f - (clamp(FadeLength - nMax, 0, 0.1f) / FadeLength);

        //float fadeOut = clamp(MaxDistance - dist, 0, FadeLength) / FadeLength;

        float mask = tex2D(maskSampler, projectorMapCoordinates).r;
        return tex2D(projectorSampler, projectorMapCoordinates) * Tint * mask * fadeOut;
    }

    return float4(texCoord.x, texCoord.y, 0.0f, 0.0f);    
}

technique ProjectorEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}

technique ProjectorOverdrawEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL OverdrawPS();
    }
}