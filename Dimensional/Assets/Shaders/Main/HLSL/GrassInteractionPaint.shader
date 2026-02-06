Shader "Custom/GrassInteractionPaint"
{
    SubShader
    {
        
        
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
        }

        ZWrite Off
        ZTest Always
        Cull Off
        Blend One One   // ADDITIVE accumulation

        Pass
        {
            Name "GrassInteractionPaint"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

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

            // Fullscreen triangle
            Varyings Vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            TEXTURE2D(_PrevTex);
            SAMPLER(sampler_PrevTex);

            float2 _HitUV;
            float2 _Direction;   // normalized XZ direction
            float  _Radius;
            float  _Strength;

            half4 Frag (Varyings i) : SV_Target
            {
                float2 delta = i.uv - _HitUV;
                float dist = length(delta);

                // Soft circular falloff
                float falloff = saturate(1.0 - dist / _Radius);
                falloff = falloff * falloff; // smoother edge

                float bend = falloff * _Strength;

                // Encode direction + strength
                float2 dir = _Direction * bend;

                return half4(dir.x, dir.y, bend, 0);
            }
            ENDHLSL
        }
    }
}
