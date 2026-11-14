Shader "Pustka/WallLamp"
{
    Properties
    {
        _LightColor("Light Color", Color) = (1, 0.72, 0.38, 1)
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
                float falloff = saturate(1.0 - distanceFromCenter);

                float angle = frac(atan2(offset.y, offset.x) / 6.2831853 + 1.0);
                float wallDistance = SAMPLE_TEXTURE2D(
                    _OcclusionMap, sampler_OcclusionMap, float2(angle, 0.5)
                ).r;

                float visibility = step(distanceFromCenter, wallDistance);
                half brightness = _Intensity * falloff * falloff * visibility;
                return half4(_LightColor.rgb * brightness, 0);
            }
            ENDHLSL
        }
    }
}