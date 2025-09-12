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
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenUV : TEXCOORD1;
            };

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTexColor;
            sampler2D _MaskTex;
            float2 _ScrollSpeed;
            float _Progress;
            float _Transparency;
            float _Rotation;
            float _InvertMask;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenUV = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate aspect ratio correction
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 center = float2(0.5, 0.5);
                float2 d = i.screenUV - center;
                
                // Apply aspect correction
                d.x *= aspect;
                
                // Apply rotation
                float s, c;
                sincos(_Rotation, s, c);
                float2 rotated = float2(
                    d.x * c - d.y * s,
                    d.x * s + d.y * c
                );
                
                // Scale to maintain square ratio
                float scale = min(aspect, 1.0);
                rotated /= scale * 2.0;
                
                // Create final UV for mask
                float2 maskUV = rotated + center;
                
                // Sample mask texture
                float mask = tex2D(_MaskTex, maskUV).r;
                
                // Apply inversion toggle
                if (_InvertMask) mask = 1.0 - mask;
                
                // Calculate reveal factor (1 = visible, 0 = hidden)
                float reveal = step(mask, _Progress);
                
                // Scroll background texture
                float2 scrollUV = i.uv + _Time.y * _ScrollSpeed;
                fixed4 texColor = tex2D(_MainTex, scrollUV) * _MainTexColor;
                
                // Layer background color behind texture
                fixed4 bgColor = _Color;
                fixed4 finalColor;
                finalColor.rgb = texColor.rgb * texColor.a + bgColor.rgb * (1 - texColor.a);
                finalColor.a = texColor.a + bgColor.a * (1 - texColor.a);
                
                // Apply reveal effect to alpha
                finalColor.a *= reveal * _Transparency;
                
                return finalColor;
            }
            ENDCG
        }
    }
}