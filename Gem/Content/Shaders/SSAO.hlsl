#include "NormalsEncodeDecode.hlsl"

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

Texture2D Normals: register(t0);
Texture2D Depth : register(t1);

float3 HemisphereSamples[64];
float3 Noise[100];

matrix ViewProjectionMatrix;
float2 Resolution;
float Scale;

SamplerState Sampler : register(s0);

float3 SampleNoise(int noiseResolution, float2 coords)
{
    float2 xy = coords * Resolution / noiseResolution;
    xy = float2(frac(xy.x), frac(xy.y)) * noiseResolution;
    float x = xy.y * noiseResolution + xy.x;
    return Noise[(int)x];
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
    float2 coords = input.TextureCoordinates;
    float depth = Depth.Sample(Sampler, coords).r;
    if (depth <= 0.0001f)
        return float4(1.0f, 1.0f, 1.0f, 1.0f);
    float3 normal = Decode(Normals.Sample(Sampler, coords).xy);
    float3 noise = SampleNoise(10, coords);
    float3 tangent = normalize(noise - normal * dot(normal, noise));
    float3 bitangent = cross(normal, tangent);
    float3x3 tbn = float3x3(tangent, bitangent, normal);
 
    float occlusion = 0;
    for (int i = 0; i < 64; ++i)
    {
        float3 rotatedHemi = mul(HemisphereSamples[i] * (Scale / depth), tbn);
        rotatedHemi.y *= -1; //WHY THIS MAKE IT BETTER?
        float2 sampleLoc = coords + (rotatedHemi.xy / Resolution ); 
        float depthAtSample = Depth.Sample(Sampler, sampleLoc).r;
        
        if (depthAtSample == 0 || depthAtSample > depth)
            continue;
        float occluded = depth - 0.02f <= depthAtSample ? 0.0f : 1.0f;
        float intensity = smoothstep(0.0, 1.0, Scale / abs(depth - depthAtSample));
        occluded *= intensity;
        occlusion += occluded;
    }
    float res = 1.0f - (occlusion / 64.0f);
    return float4(res, res, res, res);
}
