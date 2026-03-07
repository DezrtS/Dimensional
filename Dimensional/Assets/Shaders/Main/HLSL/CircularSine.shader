Shader "Custom/URP/CircularSine"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)

        _Speed ("Speed", Float) = 1
        _Amplitude ("Amplitude", Float) = 0.5
        _Frequency ("Frequency", Float) = 10
        _Clamp ("Clamp", Vector) = (1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Pass
        {
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
            };

            float4 _Color;
            float _Speed;
            float _Amplitude;
            float _Frequency;
            float2 _Clamp;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Center UVs
                float2 centered = uv - 0.5;

                // Distance from center
                float dist = length(centered);

                // Circular sine wave
                float wave = sin(dist * _Frequency - _Time.y * _Speed);

                // Apply amplitude
                wave *= _Amplitude;

                // Convert from -1..1 to 0..1
                wave = wave * 0.5 + 0.5;

                // Clamp control
                wave = min(wave, _Clamp.y);
                wave = max(wave, _Clamp.x);

                return _Color * wave;
            }

            ENDHLSL
        }
    }
}