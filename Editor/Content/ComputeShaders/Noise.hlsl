static const uint NumThreads = 512;

struct GBufferVertex
{
	float4 position;
	float3 normal;
	float2 tex;
	float3 binormal;
	float3 tangent;
};

StructuredBuffer<GBufferVertex> InputGeometry;
RWStructuredBuffer<GBufferVertex> OutputGeometry;

cbuffer Settings
{
	float multiplierA;
	float multiplierB;
	int _padding1;
	int _padding2;
};

[numthreads(NumThreads, 1, 1)]
void Kernel(in uint dispatchId : SV_DispatchThreadID)
{
	GBufferVertex vertex = InputGeometry[dispatchId];

	float3 position = vertex.position.xyz;
	float height = length(position);
	float3 normal = normalize(position);

	height = 1 + sin(position.y * multiplierA) * multiplierB;

	vertex.position.xyz = normal * height;
	OutputGeometry[dispatchId] = vertex;
}