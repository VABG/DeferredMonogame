#include "DeferredShared.hlsl"

matrix ModelViewProjection;
matrix LastModelViewProjection;
matrix ModelToWorld;

struct VertexShaderInput
{
    float3 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float3 Tangent : TANGENT0;
    float3 Binormal : BINORMAL0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 pos = mul(float4(input.Position, 1), ModelViewProjection);
    output.Position = pos;
    output.CurrentPosition = pos; 
    output.PreviousPosition = mul(float4(input.Position, 1), LastModelViewProjection);
    //output.WorldPosition = mul(float4(input.Position, 1), ModelToWorld).xyz;
    output.TexCoord = input.TexCoord;
    
    float3x3 rotationMatrix = ModelToWorld;
    
    output.Normal = normalize(mul(float4(input.Normal, 1), rotationMatrix).xyz);
    output.Tangent = normalize(mul(float4(input.Tangent, 1), rotationMatrix).xyz);
    output.Binormal =  normalize(mul(float4(input.Binormal, 1), rotationMatrix).xyz);
    return output;
}