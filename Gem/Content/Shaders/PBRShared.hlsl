#ifndef COMMON_PBS
#define COMMON_PBS

const float pi = 3.141592;

float GetMaxOrMinimumValue(float value)
{
    return max(value, 0.000001);
}

float Fresnel(float3 reflectivity, float3 view, float3 halfVector)
{
    return reflectivity + (float3(1.0f, 1.0f, 1.0f) - reflectivity) * pow(1.0f - saturate(dot(view, halfVector)), 5.0f);
}

float3 HalfVector(float3 view, float3 lightDirection)
{
    return (view + lightDirection) * .5f;
}

float GeometryShadowing(float roughness, float3 normal, float3 lightDirection)
{
    float numerator = saturate(dot(normal, lightDirection));
    float k = roughness * .5f;
    float denominator = saturate(dot(normal, lightDirection)) * (1.0f - k) + k;
    denominator = GetMaxOrMinimumValue(denominator);
    return numerator / denominator;
}

float SmithModel(float roughness, float3 normal, float3 view, float3 lightDirection)
{
    return GeometryShadowing(roughness, normal, view) * GeometryShadowing(roughness, normal, lightDirection);
}

float LightDistribution(float roughness, float3 normal, float3 halfVector)
{
    float numerator = pow(roughness, 2.0f);
    float nDotH = saturate(dot(normal, halfVector));
    float denominator = pi * pow(pow(nDotH, 2.0f) * (pow(roughness, 2.0f) - 1.0f) + 1.0f, 2.0f);
    denominator = GetMaxOrMinimumValue(denominator);
    return numerator / denominator;
}

float3 ShadePixelPBS(float3 lightDirection, float3 viewDirection, float3 lightColor, 
    float4 albedo, float4 normalGloss, float4 specularGlow)
{
    float metallic = 0.95f;
    float3 h = normalize((lightDirection + viewDirection) * .5f);
    float3 ks = Fresnel(specularGlow.xyz * normalGloss.w, viewDirection, h);
    float kd = (float3(1.0f, 1.0f, 1.0f) - ks) * (1.0f - metallic);
    
    float3 lambert = albedo.xyz / pi;
    
    float r = normalGloss.w;
    float3 n = normalGloss.xyz;
    
    float lambertShading = dot(lightDirection, n);
    
    float cookTorranceN = LightDistribution(r, n, h) * SmithModel(r, n, viewDirection, lightDirection);
    float cookTorranceD = 4.0f * saturate(dot(viewDirection, n)) * lambertShading;
    cookTorranceD = GetMaxOrMinimumValue(cookTorranceD);
    float cookTorrance = cookTorranceN / cookTorranceD;
    float3 cookTorranceColor = specularGlow.xyz * cookTorrance;
    
    float3 BRDF = kd * lambert + cookTorranceColor;
    
    //TODO: Return glow too
    return BRDF * lightColor * lambertShading;
}

#endif
