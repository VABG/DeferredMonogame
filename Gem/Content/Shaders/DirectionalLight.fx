#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

#include "DirectionalLight.hlsl"

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
        VertexShader = compile VS_SHADERMODEL MainVS();
    }
};
