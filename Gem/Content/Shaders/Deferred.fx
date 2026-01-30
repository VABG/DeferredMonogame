#include "ShaderModel.hlsl"
#include "DeferredPixel.hlsl"
#include "Vertex3D.hlsl"

technique Vertex3D
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};