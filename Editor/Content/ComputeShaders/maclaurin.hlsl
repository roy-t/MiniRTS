static const uint NumThreads = 256;

StructuredBuffer<int> InputBuffer;
RWStructuredBuffer<int> OutputBuffer;

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
	int value = InputBuffer[dispatchId.x];
	OutputBuffer[dispatchId.x] = value * multiplier + dispatchId.x;
}