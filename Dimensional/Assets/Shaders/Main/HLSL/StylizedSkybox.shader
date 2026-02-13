Shader "Skybox/StylizedSkybox"
{
    Properties
    {
        _TopColor("Top Color", Color) = (0.37, 0.52, 0.73, 1)
        _BottomColor("Bottom Color", Color) = (0.05, 0.13, 0.25, 1)
        _GradientOffset("Gradient Offset", Range(0, 1)) = 0.5
        _Exponent("Blend Exponent", Float) = 1.5

        [Header(Sun Settings)]
        _SunColor("Sun Color", Color) = (1, 0.8, 0.6, 1)
        _SunBrightness("Sun Brightness", Float) = 10.0
        _SunSize("Sun Size", Range(0.1, 5)) = 1.0
        _SunSoftness("Sun Softness", Range(0.01, 1)) = 0.1

        [Header(Mountain Settings)]
        _MountainTex("Mountain Texture", 2D) = "white" {}
        _MountainTint("Mountain Tint", Color) = (0.2, 0.25, 0.3, 1)
        _MountainHeight("Mountain Height", Range(0, 1)) = 0.3
        _MountainBlend("Mountain Blend Range", Range(0, 1)) = 0.2
        _MountainMaskTex("Mountain Mask Texture", 2D) = "white" {}
        _MountainMaskStrength("Mountain Mask Strength", Float) = 1

        [Header(Cloud Settings)]
        _CloudTex("Cloud Texture", 2D) = "white" {}
        _CloudTint("Cloud Tint", Color) = (1,1,1,1)
        _CloudHeight("Cloud Height", Range(0,1)) = 0.45
        _CloudBlend("Cloud Vertical Blend", Range(0,1)) = 0.15
        _CloudStrength("Cloud Strength", Range(0,1)) = 0.6
        _CloudSpeed("Cloud Scroll Speed", Float) = 0.01
        _CloudSunIntensity("Cloud Sun Edge Intensity", Range(0,5)) = 1.5
        _CloudSunSharpness("Cloud Sun Edge Sharpness", Range(1,10)) = 4
    }

    SubShader
    {
        Tags
        {
            "Queue"="Background"
            "RenderType"="Background"
            "PreviewType"="Skybox"
            "RenderPipeline"="UniversalPipeline"
        }

        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _TopColor;
                half4 _BottomColor;
                float _GradientOffset;
                float _Exponent;

                half3 _SunColor;
                float _SunBrightness;
                float _SunSize;
                float _SunSoftness;

                half4 _MountainTint;
                float _MountainHeight;
                float _MountainBlend;
                float _MountainMaskStrength;

                half4 _CloudTint;
                float _CloudHeight;
                float _CloudBlend;
                float _CloudStrength;
                float _CloudSpeed;
                float _CloudSunIntensity;
                float _CloudSunSharpness;
            CBUFFER_END

            TEXTURE2D(_MountainTex); SAMPLER(sampler_MountainTex);
            TEXTURE2D(_MountainMaskTex); SAMPLER(sampler_MountainMaskTex);
            TEXTURE2D(_CloudTex); SAMPLER(sampler_CloudTex);

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.viewDir = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 viewDir = normalize(i.viewDir);

                //----------------------------------
                // Gradient Sky
                //----------------------------------
                float grad = saturate((viewDir.y + _GradientOffset) / (1.0 + _GradientOffset));
                grad = pow(grad, _Exponent);
                half3 skyColor = lerp(_BottomColor.rgb, _TopColor.rgb, grad);

                //----------------------------------
                // Sun Disk (URP main light)
                //----------------------------------
                float3 sunDir = normalize(_MainLightPosition.xyz);
                float sunAngle = dot(viewDir, sunDir);
                float sunRadius = _SunSize * 0.01;
                float sunMask = smoothstep(
                    1.0 - sunRadius,
                    1.0 - sunRadius + sunRadius * _SunSoftness,
                    sunAngle
                );
                half3 sunColor = _SunColor * _SunBrightness * sunMask;

                //----------------------------------
                // Spherical UVs
                //----------------------------------
                float2 uv;
                uv.x = atan2(viewDir.x, viewDir.z) * INV_TWO_PI + 0.5;
                uv.y = saturate(viewDir.y * 0.5 + 0.5);

                //----------------------------------
                // Mountains
                //----------------------------------
                half4 mountainTex = SAMPLE_TEXTURE2D(_MountainTex, sampler_MountainTex, uv);
                half4 mountainMaskTex = SAMPLE_TEXTURE2D(_MountainMaskTex, sampler_MountainMaskTex, uv);

                float mountainHeightMask = smoothstep(
                    _MountainHeight + _MountainBlend,
                    _MountainHeight - _MountainBlend,
                    uv.y
                );

                float mountainAlpha = mountainTex.a * mountainHeightMask;
                mountainAlpha *= saturate(mountainMaskTex.r * _MountainMaskStrength);

                half3 mountainColor = mountainTex.rgb * _MountainTint.rgb;

                //----------------------------------
                // Clouds (fade into mountains)
                //----------------------------------
                float2 cloudUV = uv;
                cloudUV.x += _Time.y * _CloudSpeed;

                half4 cloudTex = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, cloudUV);

                float cloudHeightMask = smoothstep(
                    _CloudHeight - _CloudBlend,
                    _CloudHeight + _CloudBlend,
                    uv.y
                );

                float mountainFade = 1.0 - mountainAlpha;
                float cloudAlpha = cloudTex.r * cloudHeightMask * mountainFade * _CloudStrength;

                //----------------------------------
                // Sun-lit cloud edges
                //----------------------------------
                float sunCloudDot = saturate(dot(viewDir, sunDir));
                float sunEdge = pow(sunCloudDot, _CloudSunSharpness);
                half3 cloudLighting = _SunColor * sunEdge * _CloudSunIntensity;

                half3 cloudColor = cloudTex.rgb * _CloudTint.rgb + cloudLighting;

                skyColor = lerp(skyColor, cloudColor, cloudAlpha);

                //----------------------------------
                // Apply mountains last (foreground)
                //----------------------------------
                skyColor = lerp(skyColor, mountainColor, mountainAlpha);

                //----------------------------------
                // Final
                //----------------------------------
                return half4(skyColor + sunColor, 1.0);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
