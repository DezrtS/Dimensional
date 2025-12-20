Shader "Skybox/Gradient Procedural Sun Mountains"
{
    Properties
    {
        _TopColor("Top Color", Color) = (0.37, 0.52, 0.73, 1)
        _BottomColor("Bottom Color", Color) = (0.05, 0.13, 0.25, 1)
        _GradientOffset("Gradient Offset", Range(0, 1)) = 0.5
        _Exponent("Blend Exponent", Float) = 1.5

        [Header(Sun Settings)]
        [Toggle] _UseMainLight("Use Scene Light Direction", Float) = 1
        _ManualSunDirection("Manual Sun Direction", Vector) = (0, 0.2, 1, 0)
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
        _MountainMaskStrength("Mountain-to-Gradient Mask", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_lighting
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            half4 _TopColor;
            half4 _BottomColor;
            float _GradientOffset;
            float _Exponent;
            float _UseMainLight;
            float3 _ManualSunDirection;
            half3 _SunColor;
            float _SunBrightness;
            float _SunSize;
            float _SunSoftness;

            sampler2D _MountainTex;
            half4 _MountainTint;
            float _MountainHeight;
            float _MountainBlend;
            sampler2D _MountainMaskTex;
            float _MountainMaskStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.viewDir = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.viewDir);

                //----------------------------------
                // Base gradient background
                //----------------------------------
                float blend = saturate((viewDir.y + _GradientOffset) / (1 + _GradientOffset));
                blend = pow(blend, _Exponent);
                half3 skyColor = lerp(_BottomColor.rgb, _TopColor.rgb, blend);

                //----------------------------------
                // Sun disk
                //----------------------------------
                float3 sunDir = (_UseMainLight > 0.5) ? _WorldSpaceLightPos0.xyz : normalize(_ManualSunDirection);
                float sunAngle = dot(viewDir, sunDir);
                float sunRadius = _SunSize * 0.01;
                float sunFalloff = saturate((sunAngle - (1.0 - sunRadius)) / (sunRadius * _SunSoftness));
                float sunDisk = smoothstep(0, 1, sunFalloff);
                half3 sunColor = _SunColor.rgb * _SunBrightness * sunDisk;

                //----------------------------------
                // Mountains (texture projected around horizon)
                //----------------------------------
                float2 uv;
                uv.x = atan2(viewDir.x, viewDir.z) / (2.0 * UNITY_PI) + 0.5;
                uv.y = saturate(viewDir.y * 0.5 + 0.5);

                half4 mountainTex = tex2D(_MountainTex, uv);
                half4 mountainMaskTex = tex2D(_MountainMaskTex, uv);

                // Soft vertical mask for where mountains appear (bottom area)
                float heightMask = smoothstep(
                    _MountainHeight + _MountainBlend,
                    _MountainHeight - _MountainBlend,
                    uv.y
                );

                // Tint and apply mountain alpha
                half3 mountainColor = _MountainTint.rgb * mountainTex.rgb;
                float mountainAlpha = mountainTex.a * heightMask;

                // Blend mountains into gradient using _MountainMask
                skyColor = lerp(skyColor, mountainColor, mountainAlpha * min(1, mountainMaskTex.rgb * _MountainMaskStrength));

                //----------------------------------
                // Combine sun and sky
                //----------------------------------
                half3 finalColor = skyColor + sunColor;
                return half4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
