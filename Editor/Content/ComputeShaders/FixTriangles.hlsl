static const uint NumThreads = 512;

struct GBufferVertex
{
	float4 position;
	float3 normal;
	float2 tex;
	float3 binormal;
	float3 tangent;
};

StructuredBuffer<int> InputIndices;
StructuredBuffer<GBufferVertex> InputGeometry;
RWStructuredBuffer<GBufferVertex> OutputGeometry;

[numthreads(NumThreads, 1, 1)]
void Kernel(in uint dispatchId : SV_DispatchThreadID)
{
	int indexA = InputIndices[dispatchId * 3 + 0];
	int indexB = InputIndices[dispatchId * 3 + 1];
	int indexC = InputIndices[dispatchId * 3 + 2];

	GBufferVertex vertexA = InputGeometry[indexA];
	GBufferVertex vertexB = InputGeometry[indexB];
	GBufferVertex vertexC = InputGeometry[indexC];
	
	OutputGeometry[indexA] = vertexA;
	OutputGeometry[indexB] = vertexB;
	OutputGeometry[indexC] = vertexC;
}

