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
	float multiplierA;
	float multiplierB;
	int craterCount;
	int _padding2;
};

float CalculateCraterDepth(float3 vertexPosition)
{
	for (int i = 0; i < craterCount; i++)
	{
		Crater crater = InputCraters[i];		
		if (distance(vertexPosition, crater.position) < crater.radius)
		{
			return crater.floor;
		}
		else
		{
			return 1.0f;
		}
	}
	return 1.0f;
}

[numthreads(NumThreads, 1, 1)]
void Kernel(in uint dispatchId : SV_DispatchThreadID)
{
	GBufferVertex vertex = InputGeometry[dispatchId];

	float3 position = vertex.position.xyz;
	float height = length(position);
	float3 normal = normalize(position);

	float floor = CalculateCraterDepth(position);
	height = floor + sin(position.y * multiplierA) * multiplierB;

	vertex.position.xyz = normal * height;
	OutputGeometry[dispatchId] = vertex;
}

