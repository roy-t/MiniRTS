static const uint NumThreads = 256;

#define BUFFER_SIZE;

struct GBufferVertex
{
	float4 position;
	float3 normal;
	float2 tex;
	float3 binormal;
	float3 tangent;
};


StructuredBuffer<int> InputBuffer: register(t0);
RWStructuredBuffer<int> OutputBuffer : register(u1);

[numthreads(NumThreads, 1, 1)]
void Kernel (in uint3 dispatchId : SV_DispatchThreadID )
{
	/*GBufferVertex vertex = InputBuffer[dispatchId.x];

	vertex.xyz *= 1;*/

	int value = InputBuffer[dispatchId.x];
	OutputBuffer[dispatchId.x] = value * 2 + dispatchId.x;
}