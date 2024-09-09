Shader "SoFunny/Mini/MiniLit"
{
    Properties
    {
        // override properties, this shader don't support alphaclip
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_BaseMap ("Base Map", 2D) = "white" { }
        [NoScaleOffset]_NormalMap ("Normal Map", 2D) = "bump" { }
        [NoScaleOffset]_MAREMap ("Non-Metallic AO Roughness EmissiveMask Map", 2D) = "white" { }
        [HDR]_EmissionColor ("Emission Color", Color) = (0, 0, 0, 0)
        _ST ("Scale And Offset", Vector) = (1, 1, 0, 0)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "True" }
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "MiniInput.hlsl"
            #include "MiniLighting.hlsl"

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE  // this is the only shadow we need
            #pragma multi_compile_fragment _SHADOWS_SOFT
            #pragma multi_compile _ ENABLE_VS_SKINNING

            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                half3 normalOS : NORMAL;
                half4 tangentOS : TANGENT;
                half2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 uv0uv1 : TEXCOORD0;
                half4 normalWS : TEXCOORD2;     // w = viewDir.x
                half4 tangentWS : TEXCOORD3;    // w = viewDir.y
                half4 bitangentWS : TEXCOORD4;  // w = viewDir.z
                half4 sh_tangentSign : TEXCOORD5;
                float3 positionWS : TEXCOORD6;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };



            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);        SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MAREMap);        SAMPLER(sampler_MAREMap);

            // only what we need for mini surface
            struct MiniSurfaceData
            {
                half3 albedo;
                half3 normalTS;
                half4 metalic_occlusion_roughness_emissionMask;
            };

            inline void InitializeMiniSurfaceData(half2 uv, out MiniSurfaceData outMiniSurfaceData)
            {
                outMiniSurfaceData = (MiniSurfaceData)0;
                outMiniSurfaceData.albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb * _BaseColor.rgb;
                outMiniSurfaceData.normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv));
                outMiniSurfaceData.metalic_occlusion_roughness_emissionMask = SAMPLE_TEXTURE2D(_MAREMap, sampler_MAREMap, uv);
            }

            void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
            {
                inputData = (InputData)0;
                inputData.positionWS = input.positionWS;   // no need for now
                half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
                half sign = input.sh_tangentSign.w;
                input.bitangentWS.xyz = sign * (cross(input.normalWS.xyz, input.tangentWS.xyz));
                inputData.tangentToWorld = half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz);
                inputData.normalWS = TransformTangentToWorld(normalTS, inputData.tangentToWorld);
                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                inputData.viewDirectionWS = SafeNormalize(viewDirWS);     // do it in vertex stage

                inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);      // just because we only need shadow cascade situation

                //inputData.fogCoord = 0; //    no need for now
                //inputData.vertexLighting = 0    // no need for now
                inputData.bakedGI = SampleSHPixel(input.sh_tangentSign.xyz, inputData.normalWS);
                //inputData.normalizedScreenSpaceUV = 0;  // no need for now
                //inputData.shadowMask = 0;    // no need for now

            }

            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                VertexPositionInputs vpi = GetVertexPositionInputs(v.positionOS.xyz);
                VertexNormalInputs vni = GetVertexNormalInputs(v.normalOS, v.tangentOS);
                //o.uv = TRANSFORM_TEX(v.texcoord, _BaseMap);
                o.positionWS = vpi.positionWS;
                o.uv0uv1.xy = v.texcoord.xy * _ST.xy + _ST.zw;
                half3 viewDirWS = _WorldSpaceCameraPos - vpi.positionWS;        // always perspective solution
                o.normalWS = half4(vni.normalWS, viewDirWS.x);
                o.tangentWS = half4(vni.tangentWS, viewDirWS.y);
                o.bitangentWS = half4(vni.bitangentWS, viewDirWS.z);
                o.sh_tangentSign.xyz = SampleSHVertex(o.normalWS.xyz);
                half sign = v.tangentOS.w * unity_WorldTransformParams.w;       // dont' use it on ray-tracing
                o.sh_tangentSign.w = sign;
                o.positionCS = vpi.positionCS;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                MiniSurfaceData miniSurfaceData;
                InitializeMiniSurfaceData(i.uv0uv1.xy, miniSurfaceData);

                InputData inputData;
                InitializeInputData(i, miniSurfaceData.normalTS, inputData);


                //half ndotv = max(dot(inputData.normalWS, inputData.viewDirectionWS), 0.0);    // I need to fix this
                half ndotv = 0.5;

                Light light = GetMainLight(inputData.shadowCoord, inputData.positionWS, half4(1, 1, 1, 1));
                light.color *= light.shadowAttenuation;


                half3 diffuse;
                half3 specular;
                MiniLightingGeneral(
                    inputData.normalWS,
                    light.direction,
                    inputData.viewDirectionWS,
                    light.color * miniSurfaceData.metalic_occlusion_roughness_emissionMask.g + miniSurfaceData.metalic_occlusion_roughness_emissionMask.a * _EmissionColor,
                    1.0h - miniSurfaceData.metalic_occlusion_roughness_emissionMask.r, // because of white texture input by default
                    miniSurfaceData.metalic_occlusion_roughness_emissionMask.b,
                    ndotv,
                    diffuse,
                    specular);
                half3 finalColor = (diffuse.rgb + inputData.bakedGI) * miniSurfaceData.albedo + specular.rgb;
                //return half4(miniSurfaceData.metalic_occlusion_roughness_emissionMask.g, miniSurfaceData.metalic_occlusion_roughness_emissionMask.g, miniSurfaceData.metalic_occlusion_roughness_emissionMask.g, 1);
                return half4(finalColor, 1.0h);
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            //#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            #include "MiniInput.hlsl"
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma multi_compile _ ENABLE_VS_SKINNING

            half3 _LightDirection;
            float3 _LightPosition;
            float4 _ShadowBias; // x: depth bias, y: normal bias

            struct Attributes
            {
                float4 positionOS : POSITION;
                half3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float3 ApplyShadowBias(float3 positionWS, half3 normalWS, half3 lightDirection)
            {
                half invNdotL = 1.0 - saturate(dot(lightDirection, normalWS));
                float scale = invNdotL * _ShadowBias.y;

                // normal bias is negative since we want to apply an inset normal offset
                positionWS = lightDirection * _ShadowBias.xxx + positionWS;
                positionWS = normalWS * scale.xxx + positionWS;
                return positionWS;
            }

            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                half3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    half3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                    half3 lightDirectionWS = _LightDirection;
                #endif

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                return positionCS;
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }

            half4 frag(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On   // must output z-value
            ColorMask R // one channel output

            Cull Back

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            //#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            #pragma multi_compile _ ENABLE_VS_SKINNING
            #include "MiniInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };


            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                VertexPositionInputs vpi = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vpi.positionCS;

                #if UNITY_REVERSED_Z
                    output.positionCS.z = min(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    output.positionCS.z = max(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                return output;
            }

            half4 frag(Varyings input) : SV_TARGET
            {
                return 0;
            }

            ENDHLSL
        }
    }
}