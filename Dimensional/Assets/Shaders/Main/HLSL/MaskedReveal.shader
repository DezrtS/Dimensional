Shader "Custom/MaskedReveal"
{
    Properties
    {
        _Color ("Background Color", Color) = (1, 1, 1, 1)
        _MainTex ("Background Texture", 2D) = "white" {}
        _MainTexColor ("Background Texture Color", Color) = (1, 1, 1, 1)
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Vector) = (0, 0, 0, 0)
        _Progress ("Reveal Progress", Range(0, 1)) = 0
        _Transparency ("Transparency", Range(0, 1)) = 0
        _Rotation ("Rotation Angle", Float) = 0
        _InvertMask ("Invert Mask", Float) = 0

        // --- UI Masking ---
        _Stencil ("Stencil ID", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 screenUV : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            float4 _Color;
            float4 _MainTexColor;
            float2 _ScrollSpeed;
            float _Progress;
            float _Transparency;
            float _Rotation;
            float _InvertMask;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenUV = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 center = float2(0.5, 0.5);
                float2 d = i.screenUV - center;

                d.x *= aspect;

                float s, c;
                sincos(_Rotation, s, c);
                float2 rotated = float2(
                    d.x * c - d.y * s,
                    d.x * s + d.y * c
                );

                float scale = min(aspect, 1.0);
                rotated /= scale * 2.0;

                float2 maskUV = rotated + center;

                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, maskUV).r;
                if (_InvertMask > 0.5) mask = 1.0 - mask;

                float reveal = step(mask, _Progress);

                float2 scrollUV = i.uv + _Time.y * _ScrollSpeed;
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, scrollUV) * _MainTexColor;

                half4 bgColor = _Color;
                half4 finalColor;
                finalColor.rgb = lerp(bgColor.rgb, texColor.rgb, texColor.a);
                finalColor.a = lerp(bgColor.a, texColor.a, texColor.a);

                finalColor.a *= reveal * _Transparency;
                return finalColor;
            }
            ENDHLSL
        }
    }
}
