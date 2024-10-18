Shader "SoFunny/Utils/MaterialID"
{
    Properties
    {
        [HideInInspector][PerRendererData]_ColorID ("ColorID", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "True" }

        Pass
        {
            Name "MID"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ ENABLE_VS_SKINNING

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct attributes
            {
                float3 positionOS : POSITION;
                half3 normalOS : NORMAL;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _ColorID;
            CBUFFER_END

            struct varyings
            {
                float4 positionCS : SV_POSITION;
                half3 normalWS : TEXCOORD0;
                half3 viewDirWS : TEXCOORD1;
                //half3 vertexSH : TEXCOORD2;

            };

            varyings vert(attributes input)
            {
                varyings o = (varyings)0;
                VertexPositionInputs vpi = GetVertexPositionInputs(input.positionOS);
                o.positionCS = vpi.positionCS;
                VertexNormalInputs vni = GetVertexNormalInputs(input.normalOS);
                o.normalWS = vni.normalWS;
                o.viewDirWS = GetWorldSpaceNormalizeViewDir(vpi.positionWS);
                //o.vertexSH = SampleSH(vni.normalWS);
                return o;
            }

            half4 frag(varyings i) : SV_Target
            {
                //_ColorID.rgb *= i.vertexSH.rgb;
                return _ColorID;
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
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma multi_compile_instancing

            #pragma multi_compile _ ENABLE_VS_SKINNING

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half frag(Varyings input) : SV_TARGET
            {
                return input.positionCS.z;
            }

            ENDHLSL
        }
    }
}
