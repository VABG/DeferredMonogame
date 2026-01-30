#include "ShaderModel.hlsl"
#include "MotionBlur.hlsl"

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
        VertexShader = compile VS_SHADERMODEL MainVS();
    }
};
