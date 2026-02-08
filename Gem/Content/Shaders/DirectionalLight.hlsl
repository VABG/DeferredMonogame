#include "PBRShared.hlsl"
#include "NormalsEncodeDecode.hlsl"

Texture2D Albedo : register(t0);
Texture2D Normals: register(t1);
Texture2D SpecularGloss: register(t2);
Texture2D Depth: register(t3);
Texture2D AO : register(t4);
TextureCube CubeMap : register(t5);
float CubeMapLevelCount;

float3 LightDirection;
float4 LightColorStrength;
matrix InverseViewTransform;

float HalfTanFov;
float AspectRatio;
float FarPlaneDistance;
float NearPlaneDistance;

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

float3 F_SchlickRoughness(float3 V, float3 H, float3 F0, float roughness)
{
    float VdotH = max(dot(V, H), 0);
    float roughNessInv = 1.0f - roughness;
    return F0 + (max(float3(roughNessInv,roughNessInv,roughNessInv), F0) - F0) * pow(1.0 - VdotH, 5.0);
}

float3 DepthToPosition(float depth, float2 coords)
{
    float3 pos;
    coords -= 0.5f;
    coords *= 2.0f;
    pos.z = depth;//, NearPlaneDistance, FarPlaneDistance);
    pos.x = -coords.x * HalfTanFov * AspectRatio * pos.z;
    pos.y = coords.y * HalfTanFov * pos.z;
    return pos;
}

float3 Environment(float4 albedo, float3 normal, float gloss, float3 specular, float3 viewDir)
{
    //float metallic = 0.95f;
    float3 irradiance = CubeMap.SampleLevel(Sampler, normal, CubeMapLevelCount-3).xyz;
    
    float3 reflVec = reflect(viewDir, normal);
    float3 h = normalize((normal.xyz + reflVec) * .5f);
    float3 f = F_SchlickRoughness(-reflVec, h, specular.xyz, 1.0f - gloss);
    float kd = (float3(1.0f, 1.0f, 1.0f) - f); // * (1.0f - metallic);
    float3 diffuseIBL = kd * albedo.xyz * irradiance;
    float3 specularIrradiance = CubeMap.SampleLevel(Sampler, reflVec, gloss * (CubeMapLevelCount)).xyz * f;
    return (diffuseIBL + specularIrradiance);
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
    float2 coords = input.TextureCoordinates;
    float4 albedo = Albedo.Sample(Sampler, coords);
    float3x3 inv = InverseViewTransform;
    float3 worldNormals = mul(Decode(Normals.Sample(Sampler, coords).xy), inv);
    float4 specularGloss = SpecularGloss.Sample(Sampler, coords);
    float depth = Depth.Sample(Sampler, coords);
    float ao = AO.Sample(Sampler, coords).a;
    float3 pos = DepthToPosition(depth, coords);
    float3 dir = mul(normalize(pos), inv);
    float3 shaded = ShadePixelPBS(-LightDirection,
                                  dir,
                                  LightColorStrength.xyz * 2.0f,
                                  albedo,
                                  worldNormals,
                                  specularGloss.xyz, 
                                  specularGloss.w);

    float3 environment = Environment(albedo, 
        worldNormals, 
        specularGloss.w, 
        specularGloss.xyz, 
         -dir);
    return float4( shaded + (environment* 0.25f * ao), albedo.a);
}
