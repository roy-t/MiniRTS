#include "Includes/Defines.hlsl"
#include "Includes/GBufferReader.hlsl"
#include "Includes/Shadows.hlsl"

texture Volume;
sampler volumeSampler = sampler_state
{
    Texture = (Volume);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Noise;
sampler noiseSampler = sampler_state
{
    Texture = (Noise);
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4x4 InverseViewProjection;
float3 CameraPosition;
float Strength;
float MinLight;
float ViewDistance;

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct PixelData
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

struct OutputData
{
    float Media : COLOR0;
};

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1);
    output.Texture = input.Texture;

    return output;
}

float random(float2 uv)
{    
    float4 sa = float4(uv * 17, 0, 0);
    return tex2Dlod(noiseSampler, sa).r;
}

OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    // The media is only visible if it ends somewhere
    float2 fb = tex2D(volumeSampler, input.Texture).xy;
    if (fb.y == 0.0f)
    {
        output.Media = 0.0f;
        return output;
    }

    float3 world = ReadWorldPosition(input.Texture, InverseViewProjection);
    float3 volumeFront = ReadWorldPosition(input.Texture, fb.x, InverseViewProjection);
    
    float dWorld = distance(world, CameraPosition);
    float dFront = distance(volumeFront, CameraPosition);

    // Exit if the object is in front of the start of the media
    if (dWorld <= dFront)
    {
        output.Media = 0.0f;
        return output;
    }

    // Now the object can either be inside, or behind, the media
    float3 volumeBack = ReadWorldPosition(input.Texture, fb.y, InverseViewProjection);
    float dBack = distance(volumeBack, CameraPosition);
        
    // Calculate how much media is between the camera and the object
    float dInside = min(dWorld - dFront, dBack - dFront);    

    // Base the density on the strength and total viewable distance
    float mediaDensity = saturate(dInside * Strength / ViewDistance);
    
    // Compute how much light is hitting the media (so how visible it is)
    float lightness = 0.0f;            
    const uint steps = 50;

    // Start inside the volume
    float3 startPosition = dWorld < dBack ? world : volumeBack;
    float3 surfaceToLight = normalize(CameraPosition - startPosition);
    float totalDistance = distance(startPosition, volumeFront);
    float step = totalDistance / steps;

    // Randomize starting offset to reduce artefacts
    float fudge = random(input.Texture) * step;
                
    [unroll] // comment for faster compile times when trying stuff out
    for (uint i = 0; i < steps; i++)
    {          
        // Take samples along a ray from the object/back of the volume (which ever is closer)
        // to the front of the volume.         
        float3 worldPosition = startPosition + (surfaceToLight * (fudge + (step * i)));
        float depth = distance(worldPosition, CameraPosition);
        float lightFactor = ComputeLightFactor(worldPosition, depth);
        lightness += lightFactor;
    }

    lightness /= steps; 
    lightness = max(lightness, MinLight);
    

    // Don't show the fog if there's no light shining on it     
    output.Media = lerp(0, mediaDensity, lightness);
    return output;
}

technique ParticipatingMediaTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
