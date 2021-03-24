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
Texture2D Position;

static const float3 FieldMainDirection = float3(0, 0, -1);

float Elapsed;
float Time;
float MaxLifeTime;

float LengthScale;      
float NoiseStrength;

float ProgressionRate;
float FieldSpeed;

float EmitterSize;

float3 SpherePosition;
float SphereRadius;

float3 Potential(float3 p)
{
    float L;      // Length scale as described by Bridson
    float speed;  // field speed
    float alpha;  // Alpha as described by Bridson
    float beta;   // amount of curl noise compared to the constant field
    float3 n;     // Normal of closest surface
    float3 pot;   // Output potential

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

float SmoothstepPolynomial(float edge0, float edge1, float x)
{
    // Scale, bias and saturate x to 0..1 range
    x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
    // Evaluate polynomial
    return x * x * (3 - 2 * x);
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
    
    const float epsilon = 0.0001;
    
    float2 dimensions;
    Position.GetDimensions(dimensions.x, dimensions.y);

    int3 uvi = int3((dimensions * input.Texture), 0);
    float3 p = Position.Load(uvi).xyz;
    float3 v = Velocity.Load(uvi).xyz;
    
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

    output.Color = float4(velocity, 1.0f);
    return output;
}

OutputData PS_Position(PixelData input)
{
    OutputData output = (OutputData)0;

    float2 dimensions;
    Position.GetDimensions(dimensions.x, dimensions.y);

    int3 uvi = int3((dimensions * input.Texture), 0);
    float4 p = Position.Load(uvi).xyzw;
    float3 v = Velocity.Load(uvi).xyz;
    
    // The lifetime is stored in the fourth element of position    
    float3 position = p.xyz;    
    float age = p.w;

    // Euler integration
    float3 new_pos = position;

    if (age < 0)
    {
        new_pos = position;
    }
    else if (age < MaxLifeTime)
    {
        float3 delta_p = v * Elapsed;
        new_pos = position + delta_p;
    }
    else
    {        
        float a = rand(new_pos.yx) * TWO_PI;
        float r = sqrt(rand(new_pos.yz)) * EmitterSize;
        float x = r * cos(a);
        float y = r * sin(a);
        
        new_pos = float3(x, y, 0.0f);        
        age = 0.0f;
    }

    // Write output
    output.Color =  float4(new_pos, age + Elapsed);
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

technique ParticlePositionSimulationTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VS();
        PixelShader = compile PS_SHADERMODEL PS_Position();
    }
}