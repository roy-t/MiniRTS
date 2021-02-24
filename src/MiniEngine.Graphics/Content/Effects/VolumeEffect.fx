#include "Includes/Defines.hlsl"
#include "Includes/Pack.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/Instancing.hlsl"

struct VertexData
{
    float3 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float3 Normal : NORMAL0;
};

struct PixelData
{
    float4 Position : SV_POSITION;
    float4 ScreenPosition: TEXCOORD0;
    float3 WorldPosition: TEXCOORD1;
    float2 Texture : TEXCOORD2;
    float3 Coordinates : TEXCOORD3;
    float3 CoordinateNormal : NORMAL1;
    float3 Normal : NORMAL0;
};

struct OutputData
{
    float4 Albedo : COLOR0;
    float4 Material : COLOR1;
    float Depth : COLOR2;
    float4 Normal: COLOR3;
};

texture Albedo;
sampler albedoSampler = sampler_state
{
    Texture = (Albedo);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Normal;
sampler normalSampler = sampler_state
{
    Texture = (Normal);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Metalicness;
sampler metalicnessSampler = sampler_state
{
    Texture = (Metalicness);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Roughness;
sampler roughnessSampler = sampler_state
{
    Texture = (Roughness);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture AmbientOcclusion;
sampler ambientOcclusionSampler = sampler_state
{
    Texture = (AmbientOcclusion);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

float3 CameraPosition;
float4x4 World;
float4x4 WorldViewProjection;

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = mul(float4(input.Position, 1), WorldViewProjection);
    output.Texture = input.Texture;
    
    output.ScreenPosition = output.Position;
    output.WorldPosition = mul(float4(input.Position, 1), World).xyz;

    output.Coordinates = input.Position;
    output.CoordinateNormal = input.Normal;

    float3x3 rotation = (float3x3)World;
    output.Normal = normalize(mul(input.Normal, rotation));

    return output;
}

// Normal mapping as described by Christian Schüler in
// http://www.thetenthplanet.de/archives/1180
float3x3 CotangentFrame(float3 N, float3 p, float2 uv)
{
    // get edge vectors of the pixel triangle
    float3 dp1 = ddx(p);
    float3 dp2 = ddy(p);
    float2 duv1 = ddx(uv);
    float2 duv2 = ddy(uv);

    // solve the linear system
    float3 dp2perp = cross(dp2, N);
    float3 dp1perp = cross(N, dp1);
    float3 T = dp2perp * duv1.x + dp1perp * duv2.x;
    float3 B = dp2perp * duv1.y + dp1perp * duv2.y;

    // construct a scale-invariant frame
    float invmax = rsqrt(max(dot(T, T), dot(B, B)));
    return float3x3(T * invmax, B * invmax, N);
}

float3 PerturbNormal(float3 normal, float3 view, float2 uv)
{
    float3 map = UnpackNormal(tex2D(normalSampler, uv).xyz);
    float3x3 tbn = CotangentFrame(normal, -view, uv);
    return mul(map, tbn);
}


float PlaneIntersection(float3 rayOrigin, float3 rayDirection, float3 center, float3 normal)
{
    float denom = dot(normal, rayDirection);
    if (abs(denom) > EPSILON)
    {
        float t = dot((center - rayOrigin), normal) / denom;
        return t;
    }

    return 0.0f;
}

float PlaneIntersection2(float3 rayOrigin, float3 rayDirection, float3 center, float3 normal)
{
    float3 diff = rayOrigin - center;
    float prod1 = dot(diff, normal);
    float prod2 = dot(rayDirection, normal);
    float prod3 = prod1 / prod2;

    return prod3;
}


static const float4 F[] =
{
    {1, 0, 0, 0},
    {0, 1, 0, 0},
    {0, 0, 1, 0},
    {-1, 0, 0, 0},
    {0, -1, 0, 0},
    {0, 0, -1, 0},
};

// https://stackoverflow.com/questions/4248090/finding-the-length-of-a-ray-within-a-cube
// TODO: this assumes the ray starts somewhere randomly
// we only need to check tExit since we start on a position on the cube
float DistanceInsideCube(float3 rayOrigin, float3 rayDirection)
{    
    float tExit = 99;
    float3x3 rotation = (float3x3)World;

    rayOrigin = mul(rayOrigin, rotation).xyz;

    for (int i = 0; i < 6; i++)
    {                
        float3 normal = normalize(mul(F[i].xyz, rotation));
        float3 center = mul(F[i], World).xyz;

        float denom = dot(normal, rayDirection);
        if (abs(denom) > EPSILON)
        {
            float t = dot((center - rayOrigin), normal) / denom;

            float ee = dot(normal, rayDirection); // can also do above if sure

            if (t >= 0 && ee >= 0)
            {
                tExit = min(t, tExit);
            }
        }
    }

    return tExit;
}

float DistanceToLine(float3 p, float3 lineStart, float3 lineEnd)
{
    float3 d = normalize(lineEnd - lineStart);
    float3 v = p - lineStart;
    float t = dot(v, d);
    float3 P = lineStart + t * d;

    return distance(P, p);
}

float2 VolumeToTexture(float3 volume)
{
    float y = (volume.y + 1.0f) / 2.0f;
    float xs = DistanceToLine(volume, float3(0, -1.0f, 0), float3(0, 1.0f, 0));

    float x = (xs + 1.0f) / 2.0f;

    return float2(x, 1.0f - y);
}


OutputData PS(PixelData input)
{
    OutputData output = (OutputData)0;

    // START STANDARD STUFF
    float4 albedo = tex2D(albedoSampler, input.Texture);
    float metalicness = tex2D(metalicnessSampler, input.Texture).r;
    float roughness = tex2D(roughnessSampler, input.Texture).r;
    float ambientOcclusion = tex2D(ambientOcclusionSampler, input.Texture).r;

    output.Albedo = ToLinear(albedo);
    output.Material = float4(metalicness, roughness, ambientOcclusion, 1.0f);
    output.Depth = input.ScreenPosition.z / input.ScreenPosition.w;

    float3 V = normalize(CameraPosition - input.WorldPosition);
    float3 normal = PerturbNormal(input.Normal, V, input.Texture);
    output.Normal = float4(PackNormal(normal), 1.0f);
    // END STANDARD STUFF


    // TODO: Assume a unit cube for now
    float3 startPosition = input.Coordinates.xyz;
    //float3 direction = CameraPosition;

    float3 direction = normalize(input.WorldPosition - CameraPosition);
    
    float t = DistanceInsideCube(startPosition, direction);
    //output.Albedo = float4(t, t, t, max(output.Albedo.a, 1.0f));

    float3 endPosition = startPosition + direction * t;

    float3 accum = float3(0.0f, 0.0f, 0.0f);
    float weight = 0.0f;
    float step = 100;
    for (int i = 0; i < step; i++)
    {
        float fraction = i / step;
        float3 position = lerp(startPosition, endPosition, fraction);

        float2 uv = VolumeToTexture(position);
        float4 color = ToLinear(tex2D(albedoSampler, uv));

        if (color.a > 0.0f)
        {
            accum += color.rgb;
            weight += 1.0f;
        }
    }

    if (weight > 0.0f)
    {
        output.Albedo = float4(accum.rgb / weight, 1.0f);
    }    
    
    if (input.Coordinates.x > 0 && input.Coordinates.x < 0.01f)
    {
        output.Albedo = float4(1, 0, 0, 1);
    }
    else if (input.Coordinates.y > 0 && input.Coordinates.y < 0.01f)
    {
        output.Albedo = float4(0, 1, 0, 1);
    }
    else if (input.Coordinates.z > 0 && input.Coordinates.z < 0.01f)
    {
        output.Albedo = float4(0, 0, 1, 1);
    }
    else if (weight <= 0.0f)
    {
       clip(-1);
    }

    return output;
}

technique VolumeTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS();
    }
}
