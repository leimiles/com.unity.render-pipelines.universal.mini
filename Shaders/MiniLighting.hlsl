#ifndef MINI_LIGHTING_INCLUDED
#define MINI_LIGHTING_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#define invPI   0.3183h
#define INV_FG  0.0625h
#define MEDIUMP_FLT_MAX 65504.0h
#define saturateMediump(x) min(x, MEDIUMP_FLT_MAX)



void MiniLightingGeneral(half3 normal, half3 lightDir, half3 viewDir, half3 lightColor, half fZero, half roughness, half ndotv, out half3 outDiffuse, out half3 outSpecular)
{
    // diffuse
    half3 halfVec = normalize(lightDir + viewDir);

    half ndotl = max(dot(normal, lightDir), 0.0h);
    #if defined(LIGHTMAP_ON)
        outDiffuse = 0;
    #else
        outDiffuse = half3(ndotl, ndotl, ndotl) * lightColor;
    #endif

    // specular
    half ndoth = max(dot(normal, halfVec), 0.0h);
    half hdotv = max(dot(viewDir, halfVec), 0.0h);

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
    half specular = (D * fresnel * G) / (32.0h * ndotv);        // correction in gamma space

    specular = saturate(specular);
    outSpecular = half3(specular, specular, specular) * lightColor;
}

half3 MiniAdditionalLighting(Light additionalLight, InputData inputData)
{
    half3 halfVec = normalize(additionalLight.direction + inputData.viewDirectionWS);
    half ndotl = max(dot(inputData.normalWS, additionalLight.direction), 0.0h);
    return half3(ndotl, ndotl, ndotl) * additionalLight.color * additionalLight.distanceAttenuation * additionalLight.shadowAttenuation;
}

void MiniLightingGeneralWithAdditionalLight(InputData inputData, MiniSurfaceData miniSurfaceData, Light light, half ndotv, out half3 outDiffuse, out half3 outSpecular)
{
    // diffuse
    half3 halfVec = normalize(light.direction + inputData.viewDirectionWS);

    half ndotl = max(dot(inputData.normalWS, light.direction), 0.0h);
    #if defined(LIGHTMAP_ON)
        outDiffuse = 0;
    #else
        outDiffuse = half3(ndotl, ndotl, ndotl) * light.color * light.shadowAttenuation;
        #if defined(_ADDITIONAL_LIGHTS)
            uint pixelLightCount = GetAdditionalLightsCount();
            LIGHT_LOOP_BEGIN(pixelLightCount)
            Light additionalLight = GetAdditionalLight(lightIndex, inputData.positionWS, half4(1, 1, 1, 1));
            outDiffuse += MiniAdditionalLighting(additionalLight, inputData);
            // turn off rendering sepcular for now
            LIGHT_LOOP_END
        #endif
    #endif


    // specular
    half ndoth = max(dot(inputData.normalWS, halfVec), 0.0h);
    half hdotv = max(dot(inputData.viewDirectionWS, halfVec), 0.0h);

    half alpha = miniSurfaceData.metalic_occlusion_roughness_emissionMask.b * miniSurfaceData.metalic_occlusion_roughness_emissionMask.b;

    half alpha2 = alpha * alpha;
    half sum = ((ndoth * ndoth) * (alpha2 - 1.0h) + 1.0h);
    half denom = PI * sum * sum;
    half D = alpha2 / denom;

    half fZero = 1.0h - miniSurfaceData.metalic_occlusion_roughness_emissionMask.r;
    // Compute Fresnel function via Schlick's approximation.
    half fresnel = fZero + (1.0h - fZero) * pow(2.0h, (-5.55473h * hdotv - 6.98316h) * hdotv);
    half k = alpha * 0.5h;

    half G_V = ndotv / (ndotv * (1.0h - k) + k);
    half G_L = ndotl / (ndotl * (1.0h - k) + k);
    half G = (G_V * G_L);

    //half specular = (D * fresnel * G) / (4.0h * ndotv);
    half specular = (D * fresnel * G) / (32.0h * ndotv);        // correction in gamma space

    specular = saturate(specular);
    outSpecular = half3(specular, specular, specular) * light.color;
}




#endif