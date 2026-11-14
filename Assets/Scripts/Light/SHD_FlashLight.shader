Shader "Pustka/FlashLight"
{
    Properties
    {
        _LightColor("Light Color", Color) = (1, 1, 1, 1)
        _Intensity("Intensity", Range(0, 2)) = 0.35
        _OcclusionMap("Occlusion Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }

        Pass
        {
            Blend DstColor One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _LightColor;
                half _Intensity;
            CBUFFER_END
            
            TEXTURE2D(_OcclusionMap);
            SAMPLER(sampler_OcclusionMap);

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 offset = input.uv * 2.0 - 1.0;
                float distanceFromCenter = length(offset);

                // Dla quada, który ustawimy z lokalnym obrotem (90, 0, 0).
                float forward = offset.y / max(distanceFromCenter, 0.0001);
                float cone = smoothstep(0.54, 0.79, forward);

                // Jasność zaczyna spadać po pierwszej ćwiartce zasięgu.
                float falloff = 1.0 - smoothstep(0.25, 1.0, distanceFromCenter);

                float angle = frac(atan2(offset.y, offset.x) / 6.2831853 + 1.0);
                float wallDistance = SAMPLE_TEXTURE2D(
                    _OcclusionMap, sampler_OcclusionMap, float2(angle, 0.5)
                ).r;

                float visibility = step(distanceFromCenter, wallDistance);
                half brightness = _Intensity * cone * falloff * visibility;
                return half4(_LightColor.rgb * brightness, 0);
            }
            ENDHLSL
        }
    }
}