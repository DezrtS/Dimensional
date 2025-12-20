Shader "Unlit/DistanceFogGradient"
{
    Properties
    {
        _TopColor("Top Color", Color) = (0.37, 0.52, 0.73, 1)
        _BottomColor("Bottom Color", Color) = (0.05, 0.13, 0.25, 1)
        _StartDistance("Fog Start Distance", Float) = 10.0
        _EndDistance("Fog End Distance", Float) = 100.0
        _Exponent("Blend Exponent", Float) = 1.0

        [Header(Sun Settings)]
        [Toggle] _UseMainLight("Use Scene Light Direction", Float) = 1
        _ManualSunDirection("Manual Sun Direction", Vector) = (0, 0.2, 1, 0)
        _SunColor("Sun Color", Color) = (1, 0.8, 0.6, 1)
        _SunBrightness("Sun Brightness", Float) = 1.0
        _SunSize("Sun Size", Range(0.1, 5)) = 1.0
        _SunSoftness("Sun Softness", Range(0.01, 1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            half4 _TopColor;
            half4 _BottomColor;
            float _StartDistance;
            float _EndDistance;
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
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Distance from camera
                float dist = distance(_WorldSpaceCameraPos, i.worldPos);

                // Distance-based fog factor
                float fogFactor = saturate((dist - _StartDistance) / (_EndDistance - _StartDistance));
                fogFactor = pow(fogFactor, _Exponent);

                // Fog gradient between bottom/top colors
                half3 fogColor = lerp(_BottomColor.rgb, _TopColor.rgb, fogFactor);

                // Sun effect
                float3 sunDir = (_UseMainLight > 0.5) ? _WorldSpaceLightPos0.xyz : normalize(_ManualSunDirection);
                float sunAngle = dot(normalize(i.viewDir), sunDir);
                float sunRadius = _SunSize * 0.01;
                float sunFalloff = saturate((sunAngle - (1.0 - sunRadius)) / (sunRadius * _SunSoftness));
                float sunDisk = smoothstep(0, 1, sunFalloff);
                half3 sunColor = _SunColor * _SunBrightness * sunDisk;

                half3 finalColor = fogColor + sunColor;

                return half4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}
