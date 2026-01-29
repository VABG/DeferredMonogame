struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 DepthPosition : POSITION1;

};

matrix WorldViewProjection;

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;;
    output.Position = mul(input.Position, WorldViewProjection);
    output.DepthPosition = output.Position;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    return float4(input.DepthPosition.z, 0,0,1);
}