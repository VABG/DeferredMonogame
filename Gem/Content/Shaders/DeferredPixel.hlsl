#include "DeferredShared.hlsl"
#include "NormalsEncodeDecode.hlsl"

struct DeferredTarget
{
    float4 Albedo : COLOR0;
    float2 Normals : COLOR1;
    float4 SpecularGloss : COLOR2;
    float Depth : COLOR3;
    float2 Velocity : COLOR4;
};

Texture2D AlbedoTexture : register(t0);
Texture2D NormalsTexture : register(t1);
Texture2D SpecularGlossTexture : register(t2);
//Texture2D GlowTexture : register(t3);

float3 CameraPosition;

SamplerState Sampler : register(s0);

DeferredTarget MainPS(VertexShaderOutput input) : SV_Target
{
    DeferredTarget target;
    float4 color = AlbedoTexture.Sample(Sampler, input.TexCoord);
    target.Albedo = float4(color.r, color.g, color.b, 1);
    
    float3 bumpMap =  NormalsTexture.Sample(Sampler, input.TexCoord);
    bumpMap = (bumpMap * 2.0f) - 1.0f; //Convert to '-1 +1' space
    float3 bumpNormal = normalize((bumpMap.x * input.Tangent) 
        + (-bumpMap.y * input.Binormal) 
        + (bumpMap.z * input.Normal));
    
    float4 specularGloss = SpecularGlossTexture.Sample(Sampler, input.TexCoord);
    //float4 glow =GlowTexture.Sample(Sampler, input.TexCoord);
    target.Normals.xy = Encode(bumpNormal);
    //target.Normals.zw = 1.0f;
    target.SpecularGloss = float4(specularGloss.r, specularGloss.g, specularGloss.b,  specularGloss.a);
    target.Depth = input.CurrentPosition.z;
    target.Velocity = (input.CurrentPosition.xy/input.CurrentPosition.w *.5f) - (input.PreviousPosition.xy/input.PreviousPosition.w * .5f);
    return target;
}