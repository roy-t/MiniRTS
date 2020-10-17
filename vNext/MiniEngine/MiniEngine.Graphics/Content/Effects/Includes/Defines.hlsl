#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

static const float PI = 3.14159265f;
static const float EPSILON = 0.00000001f;