Shader "Custom/URP_WobbleMorphGradient"
{
    Properties
    {
        _GradientTex ("Gradient Texture", 2D) = "white" {}
        _CubeSize ("Cube Size", Float) = 1
        _MorphAmplitude ("Morph Amplitude", Range(0,1)) = 0.5
        _MorphFrequency ("Morph Frequency", Float) = 1
        _MorphSpeed ("Morph Speed", Float) = 1

        _WobbleStrength ("Wobble Strength", Float) = 0.1
        _WobbleFrequency ("Wobble Frequency", Float) = 2
        _WobbleSpeed ("Wobble Speed", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_GradientTex);
            SAMPLER(sampler_GradientTex);

            float _CubeSize;
            float _MorphAmplitude;
            float _MorphFrequency;
            float _MorphSpeed;

            float _WobbleStrength;
            float _WobbleFrequency;
            float _WobbleSpeed;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 baseNormalWS : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
            };

            float3 SpherePos(float3 p)
            {
                return normalize(p) * _CubeSize;
            }

            float3 CubePos(float3 p)
            {
                float3 absP = abs(p);
                float maxComp = max(max(absP.x, absP.y), absP.z);
                return p / maxComp * _CubeSize;
            }

            float MorphPhase(float3 worldCenter)
            {
                float wave =
                    sin(worldCenter.x * _MorphFrequency +
                        worldCenter.z * _MorphFrequency +
                        _Time.y * _MorphSpeed);

                return saturate(0.5 + wave * 0.5 * _MorphAmplitude);
            }

            float Wobble(float3 worldPos)
            {
                return sin(worldPos.x * _WobbleFrequency +
                           worldPos.z * _WobbleFrequency +
                           _Time.y * _WobbleSpeed);
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 objectPos = IN.positionOS.xyz;
                float3 worldCenter = TransformObjectToWorld(float3(0,0,0));

                float morph = MorphPhase(worldCenter);

                float3 sphere = SpherePos(objectPos);
                float3 cube   = CubePos(objectPos);
                float3 morphed = lerp(sphere, cube, morph);

                float3 worldPos = TransformObjectToWorld(morphed);

                float3 baseNormalWS = normalize(TransformObjectToWorldNormal(normalize(morphed)));

                float wob = Wobble(worldPos) * _WobbleStrength;
                worldPos += baseNormalWS * wob;

                OUT.positionWS = worldPos;
                OUT.baseNormalWS = baseNormalWS;
                OUT.positionCS = TransformWorldToHClip(worldPos);
                OUT.shadowCoord = TransformWorldToShadowCoord(worldPos);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                Light mainLight = GetMainLight(IN.shadowCoord);

                float3 N = IN.baseNormalWS;

                // --- Analytic wobble gradient ---
                float wave = cos(
                    IN.positionWS.x * _WobbleFrequency +
                    IN.positionWS.z * _WobbleFrequency +
                    _Time.y * _WobbleSpeed
                );

                float dWdx = _WobbleStrength * _WobbleFrequency * wave;
                float dWdz = _WobbleStrength * _WobbleFrequency * wave;

                float3 wobbleGradient = float3(dWdx, 0, dWdz);

                // Adjust normal
                float3 normalWS = normalize(N - wobbleGradient);

                float NdotL = saturate(dot(normalWS, mainLight.direction));

                float2 rampUV = float2(NdotL, 0.5);
                float3 ramp = SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, rampUV).rgb;

                float3 finalColor = ramp * mainLight.color * mainLight.shadowAttenuation;

                return float4(finalColor, 1);
            }

            ENDHLSL
        }

        // ---------- Shadow Caster Pass ----------
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _CubeSize;
            float _MorphAmplitude;
            float _MorphFrequency;
            float _MorphSpeed;

            float _WobbleStrength;
            float _WobbleFrequency;
            float _WobbleSpeed;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float3 SpherePos(float3 p)
            {
                return normalize(p) * _CubeSize;
            }

            float3 CubePos(float3 p)
            {
                float3 absP = abs(p);
                float maxComp = max(max(absP.x, absP.y), absP.z);
                return p / maxComp * _CubeSize;
            }

            float MorphPhase(float3 worldCenter)
            {
                float wave =
                    sin(worldCenter.x * _MorphFrequency +
                        worldCenter.z * _MorphFrequency +
                        _Time.y * _MorphSpeed);

                return saturate(0.5 + wave * 0.5 * _MorphAmplitude);
            }

            float Wobble(float3 worldPos)
            {
                return sin(worldPos.x * _WobbleFrequency +
                           worldPos.z * _WobbleFrequency +
                           _Time.y * _WobbleSpeed);
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 objectPos = IN.positionOS.xyz;
                float3 worldCenter = TransformObjectToWorld(float3(0,0,0));

                float morph = MorphPhase(worldCenter);

                float3 sphere = SpherePos(objectPos);
                float3 cube   = CubePos(objectPos);
                float3 morphed = lerp(sphere, cube, morph);

                float3 worldPos = TransformObjectToWorld(morphed);

                float wob = Wobble(worldPos) * _WobbleStrength;
                worldPos += normalize(morphed) * wob;

                OUT.positionCS = TransformWorldToHClip(worldPos);

                return OUT;
            }

            half4 frag() : SV_Target
            {
                return 0;
            }

            ENDHLSL
        }
    }
}