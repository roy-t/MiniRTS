#include "Includes/Defines.hlsl"
#include "Includes/Pack.hlsl"
#include "Includes/Gamma.hlsl"
#include "Includes/Instancing.hlsl"
#include "Includes/Noise.hlsl"

// Adpated from https://github.com/kbladin/Curl_Noise/blob/master/shaders/point_cloud_programs/update_velocities_curl_noise.frag

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
    float4 Color : COLOR0;
};

Texture2D Velocity;
Texture2D Acceleration;
Texture2D Position;
sampler dataSampler = sampler_state
{
    Texture = (Data);
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = Clamp;
    AddressV = Clamp;
};

float LengthScale;
float FieldSpeed;
float NoiseStrength;
float Elapsed; // Time step
float Time; // Global Time 
float ProgressionRate;
float3 FieldMainDirection;

// Blocking sphere
float3 SpherePosition;
float SphereRadius;

float3 Potential(float3 p)
{
    float L;      // Length scale as described by Bridson
    float speed;  // field speed
    float alpha;  // Alpha as described by Bridson
    float beta;   // amount of curl noise compared to the constant field
    float3  n;      // Normal of closest surface
    float3  pot;    // Output potential

    L = LengthScale;
    speed = FieldSpeed;
    beta = NoiseStrength;

    // Start with an empty field
    pot = float3(0, 0, 0);
    // Add Noise in each direction
    float progression_constant = 2;
    pot += L * beta * speed * float3(
        snoise(float4(p.x, p.y, p.z, Time * ProgressionRate * progression_constant) / L),
        snoise(float4(p.x, p.y + 43, p.z, Time * ProgressionRate * progression_constant) / L),
        snoise(float4(p.x, p.y, p.z + 43, Time * ProgressionRate * progression_constant) / L));

    // External directional field
    // Rotational potential gives a constant velocity field
    float3 p_parallel = dot(FieldMainDirection, p) * FieldMainDirection;
    float3 p_orthogonal = p - p_parallel;
    float3 pot_directional = cross(p_orthogonal, FieldMainDirection);

    // Add the rotational potential
    pot += (1 - beta) * speed * pot_directional;

    // Affect the field by a sphere
    // The closer to the sphere, the less of the original potential
    // and the more of a tangental potential.
    // The variable d_0 determines the distance to the sphere when the
    // particles start to become affected.
    float d_0 = L * 0.5;
    alpha = abs((smoothstep(SphereRadius, SphereRadius + d_0, length(p - SpherePosition))));
    n = normalize(p);
    pot = (alpha)*pot + (1 - (alpha)) * n * dot(n, pot);

    return pot;
}

PixelData VS(in VertexData input)
{
    PixelData output = (PixelData)0;

    output.Position = float4(input.Position, 1);
    output.Texture = input.Texture;

    return output;
}

OutputData PS_Velocity(PixelData input)
{
    OutputData output = (OutputData)0;
    
    const float epsilon = 0.0001; // TODO replace with EPSILON?

    float3 p = Velocity.SampleLevel(dataSampler, input.Texture, 0).xyz;    
    float3 potential = Potential(p);

    // Partial derivatives of different components of the potential
    float dp3_dy = (potential.z - Potential(float3(p.x, p.y + epsilon, p.z))).z / epsilon;
    float dp2_dz = (potential.y - Potential(float3(p.x, p.y, p.z + epsilon))).y / epsilon;
    float dp1_dz = (potential.x - Potential(float3(p.x, p.y, p.z + epsilon))).x / epsilon;
    float dp3_dx = (potential.z - Potential(float3(p.x + epsilon, p.y, p.z))).z / epsilon;
    float dp2_dx = (potential.y - Potential(float3(p.x + epsilon, p.y, p.z))).y / epsilon;
    float dp1_dy = (potential.x - Potential(float3(p.x, p.y + epsilon, p.z))).x / epsilon;

    // vel = nabla x potential
    // Since this the vector field has only a vector potential component
    // it is divergent free and hence contains no sources
    float3 velocity = float3(dp3_dy - dp2_dz, dp1_dz - dp3_dx, dp2_dx - dp1_dy);


    output.Color = float4(p + velocity, 1.0f);
    return output;
}

technique ParticleVelocitySimulationTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS_Velocity();
    }
}
