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

Texture2D MotionVectors: register(t0);
Texture2D GatherImage : register(t1);
//Texture2D WorldSpace: register(t1);
//float2 Resolution;
float VelocityScale;
int MaxSamples = 16;

SamplerState Sampler : register(s0);


float4 MainPS(VertexShaderOutput input) : SV_Target
{
    //float texelSize = 1.0f / Resolution;
    float2 coords = input.TextureCoordinates;
    //float4 depth = WorldSpace.Sample(Sampler, coords).w;
    float2 velocity = MotionVectors.Sample(Sampler, coords).xy;
    //float speed = length(velocity / texelSize);
    //int nSamples = clamp(int(speed), 1, 16);
    float4 color = GatherImage.Sample(Sampler, coords);
    if (color.a < 0.0)
        return color;
    
    for (int i = 1; i < 16; ++i) {
        float2 offset = velocity * (float(i) / float(16 - 1) - 0.5);
        color += GatherImage.Sample(Sampler, coords + offset);
    }
    return color / float(16);
}
