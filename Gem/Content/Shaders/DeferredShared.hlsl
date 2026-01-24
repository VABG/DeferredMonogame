#ifndef VS_SHADERMODEL
#define VS_SHADERMODEL vs_5_0
#endif

#ifndef PS_SHADERMODEL
#define PS_SHADERMODEL ps_5_0
#endif

#ifndef COMMON_STRUCTS
#define COMMON_STRUCTS

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 PreviousPosition : POSITION1;
    float4 CurrentPosition : POSITION2;
    float3 WorldPosition : POSITION3;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : NORMAL0;
    float3 Tangent : TANGENT0;
    float3 Binormal: BINORMAL0;
};

#endif