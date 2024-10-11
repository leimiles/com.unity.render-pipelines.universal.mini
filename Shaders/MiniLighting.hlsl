#ifndef MINI_LIGHTING_INCLUDED
#define MINI_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#define invPI   0.3183h
#define INV_FG  0.0625h
#define MEDIUMP_FLT_MAX 65504.0h
#define saturateMediump(x) min(x, MEDIUMP_FLT_MAX)

half3 MiniLightingDiffuse(Light light, half3 normal)
{
    half ndotl = max(dot(normal, light.direction), 0.0h);
    
    half3 diffuse;
    // diffuse
    #if defined(LIGHTMAP_ON)
    diffuse = 0;
    #else
    diffuse = half3(ndotl, ndotl, ndotl) * light.color * light.distanceAttenuation * light.shadowAttenuation;
    #endif

    return diffuse;
}

half3 MiniLightingSpecular(Light light, half3 normal, half3 viewDir, half fZero, half roughness, half ndotv)
{
    half3 halfVec = normalize(light.direction + viewDir);
    
    half ndotl = max(dot(normal, light.direction), 0.0h);
    half ndoth = max(dot(normal, halfVec), 0.0h);
    half hdotv = max(dot(viewDir, halfVec), 0.0h);

    // specular
    half alpha = roughness * roughness;

    half alpha2 = alpha * alpha;
    half sum = ((ndoth * ndoth) * (alpha2 - 1.0h) + 1.0h);
    half denom = PI * sum * sum;
    half D = alpha2 / denom;

    // Compute Fresnel function via Schlick's approximation.
    half fresnel = fZero + (1.0h - fZero) * pow(2.0h, (-5.55473h * hdotv - 6.98316h) * hdotv);
    half k = alpha * 0.5h;

    half G_V = ndotv / (ndotv * (1.0h - k) + k);
    half G_L = ndotl / (ndotl * (1.0h - k) + k);
    half G = (G_V * G_L);

    //half specular = (D * fresnel * G) / (4.0h * ndotv);
    half specular = (D * fresnel * G) / (32.0h * ndotv); // correction in gamma space

    specular = saturate(specular);
    specular = half3(specular, specular, specular) * light.color;
    
    return specular;
}

half3 MiniLightingGeneral(InputData inputData, MiniSurfaceData miniSurfaceData, half fZero, half roughness, half ndotv)
{
    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, half4(1, 1, 1, 1));

    // mainlight
    half3 outDiffuse = MiniLightingDiffuse(mainLight, inputData.normalWS);
    half3 outSpecular = MiniLightingSpecular(mainLight, inputData.normalWS, inputData.viewDirectionWS, fZero, roughness, ndotv);

    // additionalLight
    #if defined(_ADDITIONAL_LIGHTS)
        uint pixelLightCount = GetAdditionalLightsCount();
        LIGHT_LOOP_BEGIN(pixelLightCount)
        Light additionalLight = GetAdditionalLight(lightIndex, inputData.positionWS, half4(1, 1, 1, 1));
        outDiffuse += MiniLightingDiffuse(additionalLight, inputData.normalWS);
        outSpecular += MiniLightingSpecular(additionalLight, inputData.normalWS, inputData.viewDirectionWS, fZero, roughness, ndotv);
        LIGHT_LOOP_END
    #endif

    return half3((outDiffuse.rgb + inputData.bakedGI) * miniSurfaceData.albedo + outSpecular);
}

#endif
