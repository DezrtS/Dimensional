Shader "Custom/ProceduralGrass"
{
    Properties
    {
        _BottomColor ("Bottom Color", Color) = (0.15, 0.6, 0.25, 1)
        _TopColor ("Top Color", Color) = (0.35, 0.9, 0.4, 1)

        _GradientOffset ("Gradient Offset", Range(-1,1)) = 0
        _BladeSegments ("Blade Segments", Int) = 4
        
        _CurveFactor ("Curve Amount", Float) = 0.2
        _BladeWidth ("Blade Width", Float) = 1
        _WidthTaper ("Width Taper", Range(0,2)) = 1.2
        
        _WindTex ("Wind Noise Texture", 2D) = "white" {}
        _WindStrength ("Wind Strength", Float) = 0.3
        _WindScroll ("Wind Scroll (X,Z)", Vector) = (0.5, 0, 0.5, 0)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct GrassBlade
            {
                float3 position;
                float3 normal;
                float height;
                float rotation;
            };

            StructuredBuffer<GrassBlade> _Blades;

            float4 _BottomColor;
            float4 _TopColor;
            float _GradientOffset;
            int _BladeSegments;
            float _CurveFactor;
            float _BladeWidth;
            float _WidthTaper;
            TEXTURE2D(_WindTex);
            SAMPLER(sampler_WindTex);

            float _WindStrength;
            float4 _WindScroll;
            
            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float height01 : TEXCOORD0;
                float fogFactor : TEXCOORD1;
            };

            float SampleWind(float3 worldPos)
            {
                float2 uv = worldPos.xz * 0.1; // scale of wind
                uv += _WindScroll.xz * _Time.y;

                float noise = SAMPLE_TEXTURE2D(_WindTex, sampler_WindTex, uv).r;
                return (noise * 2.0 - 1.0); // remap to -1..1
            }

            Varyings vert (Attributes IN)
            {
                Varyings o;

                uint quadSegments = _BladeSegments;
                uint vertsPerBlade = quadSegments * 6 + 3;

                uint bladeIndex = IN.vertexID / vertsPerBlade;
                uint localID = IN.vertexID % vertsPerBlade;

                GrassBlade blade = _Blades[bladeIndex];

                float3 up = normalize(blade.normal);

                float3 right = normalize(cross(up, float3(0,1,0)));
                if (length(right) < 0.01)
                    right = float3(1,0,0);

                float3 forward = normalize(cross(right, up));

                float s = sin(blade.rotation);
                float c = cos(blade.rotation);

                float3 rotatedRight   =  right * c + forward * s;
                float3 rotatedForward = -right * s + forward * c;

                right = rotatedRight;
                forward = rotatedForward;

                float3 worldPos;
                if (localID < quadSegments * 6)
                {
                    uint triID = localID / 3;
                    uint vertInTri = localID % 3;

                    uint segment = triID / 2;
                    bool upperTri = (triID % 2) == 1;

                    float t0 = (float)segment / quadSegments;
                    float t1 = (float)(segment + 1) / quadSegments;

                    float width0 = _BladeWidth * pow(1.0 - t0, _WidthTaper);
                    float width1 = _BladeWidth * pow(1.0 - t1, _WidthTaper);

                    float wind = SampleWind(blade.position);

                    float bend0 = t0 * t0 * (_CurveFactor + wind * _WindStrength);
                    float bend1 = t1 * t1 * (_CurveFactor + wind * _WindStrength);


                    float3 bentUp0 = up * cos(bend0) + forward * sin(bend0);
                    float3 bentUp1 = up * cos(bend1) + forward * sin(bend1);

                    float3 base0 = blade.position + bentUp0 * (blade.height * t0);
                    float3 base1 = blade.position + bentUp1 * (blade.height * t1);

                    float3 v[6] =
                    {
                        base0 - right * width0 * 0.5,
                        base1 - right * width1 * 0.5,
                        base1 + right * width1 * 0.5,

                        base0 - right * width0 * 0.5,
                        base1 + right * width1 * 0.5,
                        base0 + right * width0 * 0.5
                    };

                    worldPos = v[vertInTri + (upperTri ? 3 : 0)];
                    o.height01 = t0;
                }
                else
                {
                    uint tipVert = localID - quadSegments * 6;

                    float wind = SampleWind(blade.position);
                    float bendTip = (_CurveFactor + wind * _WindStrength);

                    float3 bentUpTip = up * cos(bendTip) + forward * sin(bendTip);

                    float3 tipBase = blade.position + bentUpTip * blade.height;
                    float tipWidth = _BladeWidth * pow(1.0 - 1.0, _WidthTaper) * 0.5;

                    float3 tipVerts[3] =
                    {
                        tipBase - right * tipWidth,
                        tipBase + right * tipWidth,
                        tipBase + up * (blade.height / _BladeSegments)
                    };

                    worldPos = tipVerts[tipVert];
                    o.height01 = 1.0;
                }
                
                o.pos = TransformWorldToHClip(worldPos);
                o.fogFactor = ComputeFogFactor(o.pos.z);

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float h = saturate(i.height01 + _GradientOffset * 0.5);
                half4 col = lerp(_BottomColor, _TopColor, h);
                col.rgb = MixFog(col.rgb, i.fogFactor);
                return col;
            }

            ENDHLSL
        }
    }
}
