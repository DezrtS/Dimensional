Shader "Custom/DitheredDiffuse"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1,1,1,1)

        _DitherScale ("Dither Scale", Float) = 1
        _DistanceFromCamera ("Dither Fade", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma multi_compile_fog

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float4 screenPos   : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4;
                float  fogFactor  : TEXCOORD5;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;
            float _DitherScale;
            float _DistanceFromCamera;

            Varyings vert (Attributes v)
            {
                Varyings o;

                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(o.positionWS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.positionHCS);
                o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);
                o.fogFactor = ComputeFogFactor(o.positionHCS.z);
                
                return o;
            }

            // 4x4 Bayer matrix
            float Dither4x4(uint2 pixelPos)
            {
                uint x = pixelPos.x % 4;
                uint y = pixelPos.y % 4;

                int index = x + y * 4;

                const float dither[16] =
                {
                    0.0,  8.0,  2.0, 10.0,
                    12.0, 4.0, 14.0, 6.0,
                    3.0, 11.0, 1.0,  9.0,
                    15.0,7.0, 13.0, 5.0
                };

                return dither[index] / 16.0;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // --- DITHER ---
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                int2 pixelPos = uint2(screenUV * _ScreenParams.xy / _DitherScale);

                float ditherValue = Dither4x4(pixelPos);
                clip(_DistanceFromCamera - ditherValue);

                // --- TEXTURE ---
                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                float3 albedo = tex.rgb * _BaseColor.rgb;

                float3 normalWS = normalize(i.normalWS);

                // Shadow-aware light
                Light mainLight = GetMainLight(i.shadowCoord);

                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 lighting = mainLight.color * NdotL * mainLight.shadowAttenuation;

                // Ambient
                float3 ambient = SampleSH(normalWS);

                float3 finalColor = albedo * (lighting + ambient);

                finalColor = MixFog(finalColor, i.fogFactor);

                return float4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}