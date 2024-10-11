Shader "SoFunny/Mini/QuadShadows"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex ("Particle Texture", 2D) = "white" { }
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent-10" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane"
        }
        
        Pass
        {
            Name "QuadShadows"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Lighting Off ZWrite Off

            Blend SrcAlpha OneMinusSrcAlpha, one OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _TintColor;
            CBUFFER_END

            struct appdata_t
            {
                float4 vertex: POSITION;
                float2 texcoord: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex: SV_POSITION;
                float2 texcoord: TEXCOORD0;
                float3 positionWS: TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                VertexPositionInputs vpi = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vpi.positionCS;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.positionWS = vpi.positionWS;
                return o;
            }

            half4 frag(v2f i): SV_Target
            {
                half4 col = _TintColor * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                return col;
            }
            ENDHLSL
        }
    }

    Fallback  "Hidden/Universal Render Pipeline/FallbackError"
}