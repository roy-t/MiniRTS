#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif
