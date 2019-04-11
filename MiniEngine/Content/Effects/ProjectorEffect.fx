#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/GBuffer.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Light.hlsl"

float4x4 ProjectorViewProjection;
float4 Tint;
float3 ProjectorPosition;
float3 ProjectorForward;
float MaxDistance;

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

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = float4(input.Position, 1);
    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float2 texCoord = input.TexCoord;    
    float4 position = ReadWorldPosition(texCoord, InverseViewProjection);

    // Move from world position to the reference frame of the projector
    float4 positionInProjectorReferenceFrame = mul(position, ProjectorViewProjection);

    // Figure out where on the projector map the current pixel is
    float2 projectorMapCoordinates = ToTextureCoordinates(positionInProjectorReferenceFrame.xy, positionInProjectorReferenceFrame.w);
        
    // Distance between pixel and projector
    float dist = distance(ProjectorPosition, position.xyz);

    // Angle between pixel and projector, dir > 0 means the pixel is in fron of the projector
    // while dir < 0 means its behind it.
    float3 direction = normalize(position.xyz - ProjectorPosition);
    float dir = dot(ProjectorForward, direction);

    // Only apply the projector if the it is inside the bounds of the projector texture, close enough, and in front of the projector
    if (dir > 0 && dist < MaxDistance &&
        projectorMapCoordinates.x >= 0.0f && projectorMapCoordinates.x <= 1.0f &&
        projectorMapCoordinates.y >= 0.0f && projectorMapCoordinates.y <= 1.0f)
    {        		       
        return tex2D(projectorSampler, projectorMapCoordinates) * Tint;        
    }
    
    return float4(1.0f, 0.0f, 0.0f, 0.0f);
}

technique ProjectorEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}