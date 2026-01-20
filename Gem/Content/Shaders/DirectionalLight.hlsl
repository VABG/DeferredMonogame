#include "PBRShared.hlsl"

Texture2D Albedo : register(t0);
Texture2D NormalsGloss: register(t1);
Texture2D SpecularGlow: register(t2);
Texture2D WorldSpace: register(t3);
TextureCube<float4> CubeMap : register(t4);
float CubeMapLevelCount; 

float3 LightDirection;
float4 LightColorStrength;
float3 CameraPosition;

//TODO: Shadow map(s) somehow
SamplerState Sampler : register(s0);

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

float3 Environment(float4 albedo, float4 normalsGloss, float4 specularGlow, float3 viewDir)
{
    float3 reflVec = reflect(viewDir, normalsGloss.xyz);
    float metallic = 0.95f;
    float3 irradiance = CubeMap.SampleLevel(Sampler, -normalsGloss.xzy, CubeMapLevelCount).xyz;
    float3 h = normalize((reflVec + viewDir) * .5f);
    float f = Fresnel(specularGlow,viewDir,  h);
    float kd = (float3(1.0f, 1.0f, 1.0f) - f) * (1.0f - metallic);
    float3 diffuseIBL = kd * albedo.xyz * irradiance;
    float3 specularIrradiance = CubeMap.SampleLevel(Sampler, -reflVec, normalsGloss.a * f * CubeMapLevelCount).xyz;
    return (diffuseIBL + specularIrradiance * f * specularGlow.xyz) * .5f;
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
    float2 coords = input.TextureCoordinates;
    float4 albedo = Albedo.Sample(Sampler,coords);
    float4 normalsGloss = NormalsGloss.Sample(Sampler,coords);
    float4 specularGlow = SpecularGlow.Sample(Sampler,coords);
    float4 worldSpace = WorldSpace.Sample(Sampler,coords);
    float3 dir = normalize(CameraPosition - worldSpace.xyz);
    float3 shaded =ShadePixelPBS(-LightDirection, 
        dir, 
        LightColorStrength.xyz * 2.0f, 
        albedo, 
        normalsGloss, 
        specularGlow);
    
    float3 environment =Environment(albedo, normalsGloss, specularGlow, dir) * .5f;
   
    return float4(shaded + environment, 1.0f) ;
}

