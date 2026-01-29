#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

#include "ShadowPass.hlsl"

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};