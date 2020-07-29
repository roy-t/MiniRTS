#define thread_group_size_x 32
#define thread_group_size_y 32

// uguale al valore passato al dispatch 
#define N_THREAD_GROUPS_X 64

struct BufferStruct
{
	float value;
	int x;
	float unused;
	float unused2;
};

 
RWStructuredBuffer<BufferStruct> g_OutBuff;

float FACT(int n)
{
	float tot=1.0;
	while(n>1)
	{
		tot*=(float)n;
		n--;
	}
	return tot;
}

float MacLaurin(float x)
{
	float tot=1;
	for(int i=0;i<10;i++)
	{
		tot += (pow(x,i)/ FACT(i));
	}
	return tot;
}
 
[numthreads( thread_group_size_x, thread_group_size_y, 1 )]
void CS( uint3 threadIDInGroup : SV_GroupThreadID, uint3 groupID : SV_GroupID, uint groupIndex : SV_GroupIndex,     uint3 dispatchThreadID : SV_DispatchThreadID )
{
	//valori per riga
	int stride = thread_group_size_x * N_THREAD_GROUPS_X;  
	//indice linearizzato
	int idx = dispatchThreadID.y * stride + dispatchThreadID.x;
	 
	
	g_OutBuff[ idx ].value = MacLaurin(idx/1000.0F);
	g_OutBuff[ idx ].x=idx;
	g_OutBuff[ idx ].unused=0;
	g_OutBuff[ idx ].unused2=0;
}

RWStructuredBuffer<float4> g_OutBuff2;

[numthreads( thread_group_size_x, thread_group_size_y, 1 )]
void CS2( uint3 threadIDInGroup : SV_GroupThreadID, uint3 groupID : SV_GroupID, uint groupIndex : SV_GroupIndex,     uint3 dispatchThreadID : SV_DispatchThreadID )
{
	//valori per riga
	int stride = thread_group_size_x * N_THREAD_GROUPS_X;
	//indice linearizzato
	int idx = dispatchThreadID.y * stride + dispatchThreadID.x;
	g_OutBuff2[ idx ] = float4(dispatchThreadID.x, dispatchThreadID.y, dispatchThreadID.z, 1.0f);
}