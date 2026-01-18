#include "PBRShared.hlsl"

Texture2D Albedo : register(t0);
Texture2D NormalsGloss: register(t1);
Texture2D SpecularGlow: register(t2);
Texture2D WorldSpace: register(t3);

float3 LightDirection;
float4 LightColorStrength;
float3 CameraPosition;

//TODO: Shadow map somehow

sampler2D AlbedoSampler = sampler_state
{
    Texture = <Albedo>;
};

sampler2D NormalsGlossSampler = sampler_state
{
    Texture = <NormalsGloss>;
};

sampler2D SpecularGlowSampler = sampler_state
{
    Texture = <SpecularGlow>;
};

sampler2D WorldSpaceSampler = sampler_state
{
    Texture = <WorldSpace>;
};

struct VertexShaderInput
{
    float3 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD1;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position, 1);
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
    float2 coords = input.TextureCoordinates;
    float4 albedo = tex2D(AlbedoSampler,coords);
    float4 normalsGloss = tex2D(NormalsGlossSampler,coords);
    float4 specularGlow = tex2D(SpecularGlowSampler,coords);
    float4 worldSpace = tex2D(WorldSpaceSampler,coords);
    float3 dir = normalize(CameraPosition - worldSpace.xyz);
    float3 shaded =ShadePixelPBS(LightDirection, 
        dir, 
        LightColorStrength.xyz, 
        albedo, 
        normalsGloss, 
        specularGlow);
    
    return float4(shaded, 1.0f);
}