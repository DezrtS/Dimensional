Shader "Skybox/Gradient Procedural Sun"
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
                
                // Vertical gradient based on view direction (Y-axis)
                float blend = saturate((viewDir.y + _GradientOffset) / (1 + _GradientOffset));
                blend = pow(blend, _Exponent);
                half3 skyColor = lerp(_BottomColor.rgb, _TopColor.rgb, blend);
                
                // Get sun direction (automatically or manual)
                float3 sunDir;
                if (_UseMainLight > 0.5)
                {
                    // Use scene's main directional light direction
                    sunDir = _WorldSpaceLightPos0.xyz;
                }
                else
                {
                    // Use manual direction
                    sunDir = normalize(_ManualSunDirection);
                }
                
                // Calculate sun disk
                float sunAngle = dot(viewDir, sunDir);
                
                // Calculate sun size with soft edges
                float sunRadius = _SunSize * 0.01; // Scale size for practical range
                float sunFalloff = saturate((sunAngle - (1.0 - sunRadius)) / (sunRadius * _SunSoftness));
                float sunDisk = smoothstep(0, 1, sunFalloff);
                
                // Apply brightness and color
                half3 sunColor = _SunColor.rgb * _SunBrightness * sunDisk;
                
                // Combine sky and sun (additive blending)
                half3 finalColor = skyColor + sunColor;
                return half4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}