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

float smin(float a, float b, float k) 
{
	k = max(0, k);
	float h = max(0, min(1, (b - a + k) / (2 * k)));
	return a * h + b * (1 - h) - k * h * (1 - h);
}

float smax(float a, float b, float k) 
{
	k = min(0, -k);
	float h = max(0, min(1, (b - a + k) / (2 * k)));
	return a * h + b * (1 - h) - k * h * (1 - h);
}

float Cavity(float x)
{
	return x * x - 1;
}

float Rim(float x)
{
	float rimX = min(x - rimWidth, 0);
	return (rimX * rimX) * rimSteepness;
}

float CalculateCraterDepth(float3 vertexPosition)
{
	float craterHeight = 0.0f;
	for (int i = 0; i < craterCount; i++)
	{
		Crater crater = InputCraters[i];		
		float x = distance(vertexPosition, crater.position) / crater.radius;
		float cavity = Cavity(x);		
		float rim = Rim(x);
		float height = smin(cavity, rim, crater.smoothness);
		height = smax(crater.floor, height, crater.smoothness);

		craterHeight += height;
	}
	
	return craterHeight;
}

[numthreads(NumThreads, 1, 1)]
void Kernel(in uint dispatchId : SV_DispatchThreadID)
{
	GBufferVertex vertex = InputGeometry[dispatchId];

	float3 position = vertex.position.xyz;
	float3 normal = normalize(position);

	float height = 1.0f + CalculateCraterDepth(normal);

	vertex.position.xyz = normal * height;
	OutputGeometry[dispatchId] = vertex;
}

