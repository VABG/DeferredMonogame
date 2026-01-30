#if OPENGL
#ifndef SV_POSITION
#define SV_POSITION POSITION
#endif

#ifndef PS_SHADERMODEL
#define PS_SHADERMODEL ps_3_0
#endif

#ifndef VS_SHADERMODEL
#define VS_SHADERMODEL vs_3_0
#endif

#else
#ifndef PS_SHADERMODEL
#define PS_SHADERMODEL ps_5_0
#endif

#ifndef VS_SHADERMODEL
#define VS_SHADERMODEL vs_5_0
#endif

#endif

