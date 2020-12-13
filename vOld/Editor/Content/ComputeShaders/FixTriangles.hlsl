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

// Calculate 'Mean Weighted Equally' normals in two steps

[numthreads(NumThreads, 1, 1)]
void SumNormals(in uint dispatchId : SV_DispatchThreadID)
{
	int indexA = InputIndices[dispatchId * 3 + 0];
	int indexB = InputIndices[dispatchId * 3 + 1];
	int indexC = InputIndices[dispatchId * 3 + 2];

	GBufferVertex vertexA = InputGeometry[indexA];
	GBufferVertex vertexB = InputGeometry[indexB];
	GBufferVertex vertexC = InputGeometry[indexC];
	


	float3 aa = vertexA.position;
	float3 bb = vertexB.position;
	float3 cc = vertexC.position;

	float3 faceNormal = cross(cc - bb, bb - aa);
	float len = faceNormal.Length();
	if (len > 0)
	{
		faceNormal = normalize(faceNormal);

		vertexA.normal += faceNormal;
		vertexB.normal += faceNormal;
		vertexC.normal += faceNormal;
	}

	OutputGeometry[indexA] = vertexA;
	OutputGeometry[indexB] = vertexB;
	OutputGeometry[indexC] = vertexC;
}

[numthreads(NumThreads, 1, 1)]
void AverageNormals(in uint dispatchId : SV_DispatchThreadID)
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

