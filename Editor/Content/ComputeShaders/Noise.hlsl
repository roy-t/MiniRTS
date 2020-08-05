static const uint NumThreads = 256;

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
	int multiplier;
	int _padding0;
	int _padding1;
	int _padding2;
};

[numthreads(NumThreads, 1, 1)]
void Kernel(in uint3 dispatchId : SV_DispatchThreadID)
{
	GBufferVertex vertex = InputGeometry[dispatchId.x];
	vertex.position.x *= multiplier;
	OutputGeometry[dispatchId.x] = vertex;
}