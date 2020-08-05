static const uint NumThreads = 512;

struct GBufferVertex
{
	float4 position;
	float3 normal;
	float2 tex;
	float3 binormal;
	float3 tangent;
};

struct Crater
{
	float3 position;
	float radius;
	float floor;
	float smoothness;
};

StructuredBuffer<GBufferVertex> InputGeometry;
StructuredBuffer<Crater> InputCraters;
RWStructuredBuffer<GBufferVertex> OutputGeometry;


cbuffer Settings
{
	float rimWidth;
	float rimSteepness;
	int craterCount;
	int padding;
};

// From https://github.com/SebLague/Solar-System/blob/Episode_02/Assets/Celestial%20Body/Scripts/Shaders/Includes/Math.cginc
// Smooth minimum of two values, controlled by smoothing factor k
// When k = 0, this behaves identically to min(a, b)
float smoothMin(float a, float b, float k) 
{
	k = max(0, k);
	// https://www.iquilezles.org/www/articles/smin/smin.htm
	float h = max(0, min(1, (b - a + k) / (2 * k)));
	return a * h + b * (1 - h) - k * h * (1 - h);
}

// Smooth maximum of two values, controlled by smoothing factor k
// When k = 0, this behaves identically to max(a, b)
float smoothMax(float a, float b, float k) 
{
	k = min(0, -k);
	float h = max(0, min(1, (b - a + k) / (2 * k)));
	return a * h + b * (1 - h) - k * h * (1 - h);
}


float CavityShape(float x) {
	return x * x - 1;
}

float RimShape(float x) {
	x = min(x - 1 - rimWidth, 0);
	return rimSteepness * x * x;
}


// TODO: https://www.youtube.com/watch?v=lctXaT9pxA0&t=171s
// https://github.com/SebLague/Solar-System/blob/Episode_02/Assets/Celestial%20Body/Scripts/Shaders/Includes/Craters.cginc
// See calculator: https://www.desmos.com/calculator to figure out how formula's interact
float CalculateCraterDepth(float3 vertexPosition)
{
	float craterHeight = 0.0f;
	for (int i = 0; i < craterCount; i++)
	{
		Crater crater = InputCraters[i];		
		float x = length(vertexPosition - crater.position) / max(crater.radius, 0.0001);

		float cavity = x * x - 1;
		float rimX = min(x - rimWidth, 0);
		float rim = (rimX * rimX) * rimSteepness;
		float combined = 1 + smoothMin(cavity, rim, crater.smoothness);
		combined = smoothMax(crater.floor, combined, crater.smoothness);

		craterHeight += combined;
	}
	
	return craterHeight;
}

[numthreads(NumThreads, 1, 1)]
void Kernel(in uint dispatchId : SV_DispatchThreadID)
{
	GBufferVertex vertex = InputGeometry[dispatchId];

	float3 position = vertex.position.xyz;
	float3 normal = normalize(position);

	float height = CalculateCraterDepth(normal);

	vertex.position.xyz = normal * height;
	OutputGeometry[dispatchId] = vertex;
}

