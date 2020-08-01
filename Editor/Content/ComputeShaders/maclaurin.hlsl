static const uint NumThreads = 256;

#define BUFFER_SIZE;

//struct GBufferVertex
//{
//	float4 position;
//	float3 normal;
//	float2 tex;
//	float3 binormal;
//	float3 tangent;
//};
//
//StructuredBuffer<int> InputBuffer: register(t0); // Shader Resource
RWStructuredBuffer<int> OutputBuffer : register(u0); // Unordered Access View

//cbuffer settings : register(b0) // Constant Buffer
//{
//	int multiplier;
//	int _padding0;
//	int _padding1;
//	int _padding2;
//};

[numthreads(NumThreads, 1, 1)]
void Kernel(in uint3 dispatchId : SV_DispatchThreadID)
{
	//int value = InputBuffer[dispatchId.x];
	OutputBuffer[dispatchId.x] = dispatchId.x;// value * 2; // value* multiplier + dispatchId.x;
}