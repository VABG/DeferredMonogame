#ifndef COMMON_PBS
#define COMMON_PBS

float3 ShadePixelPBS(float3 lightDirection, float3 viewDirection, float3 lightColor, float4 albedo, float4 normalGloss, float4 specularGlow)
{
    float lambert = saturate(dot(normalGloss.xyz, -lightDirection));
    float3 reflectionVector = normalize(reflect(lightDirection, normalGloss.xyz));
    float3 specularLight = float3(specularGlow.xyz * pow(saturate(dot(reflectionVector, viewDirection)), normalGloss.a*20.0f))*lambert;
    specularLight = specularLight * lightColor;
    float3 color =  albedo.xyz * lambert;
    return color + specularLight.xyz;
}

#endif