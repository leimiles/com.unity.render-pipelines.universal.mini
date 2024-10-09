Shader "SoFunny/Mini/MiniOutline"
{

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "True" }
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            Cull Front

            HLSLPROGRAM
            #pragma target 2.0
            #include "MiniInput.hlsl"
            #pragma multi_compile _ ENABLE_VS_SKINNING
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                half3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 normalWS : TEXCOORD2;     // w = viewDir.x
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                v.positionOS.xyz += SafeNormalize(v.normalOS.xyz) * 0.03f;
                VertexPositionInputs vpi = GetVertexPositionInputs(v.positionOS.xyz);
                VertexNormalInputs vni = GetVertexNormalInputs(v.normalOS);
                o.normalWS = half4(vni.normalWS, 1.0);
                o.positionCS = vpi.positionCS;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                return half4(0, 0, 0, 1.0h);
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

            #pragma multi_compile_instancing

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

    Fallback  "Hidden/Universal Render Pipeline/FallbackError"
}