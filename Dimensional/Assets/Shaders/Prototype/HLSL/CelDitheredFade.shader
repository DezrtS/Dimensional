Shader "Custom/CelShadedDitherFade"
{
    Properties
    {
        _BaseMap("Base Texture", 2D) = "white" {}
        _Color("Base Color", Color) = (1,1,1,1)

        // Dither fade
        _Fade("Fade", Range(0,1)) = 1.0
        _DitherScale("Dither Cell Size (pixels)", Range(1,32)) = 4

        // Cel shading controls
        _MinBrightness("Min Brightness", Range(-1,1)) = 0.2
        _MaxBrightness("Max Brightness", Range(0,1)) = 1.0
        _Shades("Number of Shades", Range(2,8)) = 3
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float4 _Color;

            float _Fade;
            float _DitherScale;

            float _MinBrightness;
            float _MaxBrightness;
            int _Shades;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // 4x4 Bayer dither matrix
            float DitherBayer4(int2 p)
            {
                const float bayer[16] = {
                     0,  8,  2, 10,
                    12,  4, 14,  6,
                     3, 11,  1,  9,
                    15,  7, 13,  5
                };
                return (bayer[p.y * 4 + p.x] + 0.5) / 16.0;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Base color
                float4 baseCol = tex2D(_BaseMap, i.uv) * _Color;

                // === Cel Shading ===
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = dot(normalize(i.normal), lightDir);
                NdotL = saturate(NdotL * 0.5 + 0.5); // remap -1..1 -> 0..1

                // Remap to min/max brightness
                float lit = lerp(_MinBrightness, _MaxBrightness, NdotL);

                // Quantize into discrete steps
                float stepSize = 1.0 / (_Shades - 1);
                lit = round(lit / stepSize) * stepSize;

                baseCol.rgb *= lit;


                // === Screen-space dithering ===
                float2 screenPos = i.pos.xy; // pixel coords
                int2 pixelCoords = int2(floor(screenPos / _DitherScale));
                int2 bayerCoords = pixelCoords & 3;
                float threshold = DitherBayer4(bayerCoords);

                float alpha = step(threshold, _Fade);
                baseCol.a *= alpha;

                return baseCol;
            }
            ENDHLSL
        }
    }
}
