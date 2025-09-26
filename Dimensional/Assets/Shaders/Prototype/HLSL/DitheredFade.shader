Shader "Custom/DitheredFadeScreenSpaceGlobal"
{
    Properties
    {
        _BaseMap("Base Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Fade("Fade", Range(0,1)) = 1.0
        _DitherScale("Dither Cell Size (pixels)", Range(1,32)) = 4
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;   // contains absolute pixel coords in xy
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
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
                float4 col = tex2D(_BaseMap, i.uv) * _Color;

                // true screen-space pixel position
                float2 pixelCoords = i.pos.xy / _DitherScale;

                // snap to integer grid
                int2 bayerCoords = int2(floor(pixelCoords)) & 3;

                float threshold = DitherBayer4(bayerCoords);

                // apply dithered fade
                float alpha = step(threshold, _Fade);
                col.a *= alpha;

                return col;
            }
            ENDHLSL
        }
    }
}
