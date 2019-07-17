// Combines all techniques for rendering to the geometry buffer, and rendering shadow maps
// into one shader

#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/Helpers.hlsl"
#include "Includes/Pack.hlsl"

texture Texture;
sampler diffuseSampler = sampler_state
{
    Texture = (Texture);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture SpecularMap;
sampler specularSampler = sampler_state
{
    Texture = (SpecularMap);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture = (NormalMap);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Mask;
sampler maskSampler = sampler_state
{
    Texture = (Mask);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Skybox;
samplerCUBE skyboxSampler = sampler_state
{
    Texture = (Skybox);
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = Mirror;
    AddressV = Mirror;
};

float3 CameraPosition;

#include "Techniques/ShadowMap.hlsl"
#include "Techniques/Textured.hlsl"
#include "Techniques/Deferred.hlsl"

// The Effect Compiler doesn't like files without an empty new line at the end.