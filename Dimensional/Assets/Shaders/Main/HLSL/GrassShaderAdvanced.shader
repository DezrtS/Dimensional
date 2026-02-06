Shader "Custom/ProceduralGrass"
{
    Properties
    {
        _BottomColor ("Bottom Color", Color) = (0.15, 0.6, 0.25, 1)
        _TopColor ("Top Color", Color) = (0.35, 0.9, 0.4, 1)

        _GradientOffset ("Gradient Offset", Range(-1,1)) = 0
        
        _InteractionBendStrength ("Interaction Bend Strength", Float) = 1
        
        _WindTex ("Wind Noise Texture", 2D) = "white" {}
        _WindStrength ("Wind Strength", Float) = 0.3
        _WindAngle ("Wind Angle (Radians)", Float) = 0
        _WindSpeed ("Wind Speed", Float) = 1.0
        _WindScale ("Wind Scale", Float) = 0.1
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

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct GrassVertex
            {
                float3 position;
                float3 normal;
                float  height01;
                float2 uv;
            };

            StructuredBuffer<GrassVertex> _Vertices;
            StructuredBuffer<uint>        _Indices;

            float4 _BottomColor;
            float4 _TopColor;
            float  _GradientOffset;

            TEXTURE2D(_GrassInteractionTex);
            SAMPLER(sampler_GrassInteractionTex);

            float _InteractionBendStrength;

            TEXTURE2D(_WindTex);
            SAMPLER(sampler_WindTex);

            float _WindStrength;
            float _WindAngle;
            float _WindSpeed;
            float _WindScale;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float  height01   : TEXCOORD1;
                float  fogFactor  : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            float SampleWind(float3 worldPos, float2 windDir)
            {
                float2 uv = worldPos.xz * _WindScale;
                uv += windDir * _WindSpeed * _Time.y;

                // Explicit LOD sample in vertex stage
                float noise = SAMPLE_TEXTURE2D_LOD(_WindTex, sampler_WindTex, uv, 0).r;
                return noise * 2.0 - 1.0;
            }

            Varyings vert (Attributes IN)
            {
                Varyings o;

                uint index = _Indices[IN.vertexID];
                GrassVertex v = _Vertices[index];

                float3 worldPos = v.position;
                float3 worldNormal = normalize(v.normal);

                float4 interaction =
                    SAMPLE_TEXTURE2D_LOD(
                        _GrassInteractionTex,
                        sampler_GrassInteractionTex,
                        v.uv,
                        0
                    );

                float bendStrength = interaction.r * _InteractionBendStrength * v.height01;
                float3 bendOffset = float3(0, -bendStrength, 0);
                
                // --- WIND DIRECTION ---
                float2 windDirXZ = float2(cos(_WindAngle), sin(_WindAngle));
                float3 windDirWS = float3(windDirXZ.x, 0, windDirXZ.y);

                float windNoise = SampleWind(worldPos, windDirXZ);

                float bend = windNoise * _WindStrength * v.height01;

                // --- POSITION BEND ---
                worldPos += windDirWS * bend + bendOffset;

                // --- NORMAL BEND ---
                // Tilt normal toward wind direction
                float3 bentNormal = normalize(
                    worldNormal + windDirWS * bend * 0.75 + bendOffset
                );

                float4 clipPos = TransformWorldToHClip(worldPos);

                o.positionCS = clipPos;
                o.normalWS   = bentNormal;
                o.height01   = v.height01;
                o.fogFactor  = ComputeFogFactor(clipPos.z);

                // Needed for shadow sampling
                o.shadowCoord = TransformWorldToShadowCoord(worldPos);

                return o;
            }


            half4 frag (Varyings i) : SV_Target
            {
                float h = saturate(i.height01 + _GradientOffset * 0.5);
                float3 baseColor = lerp(_BottomColor.rgb, _TopColor.rgb, h);

                float3 normal = normalize(i.normalWS);

                Light mainLight = GetMainLight(i.shadowCoord);
                float shadow = lerp(0.35, 1.0, mainLight.shadowAttenuation);
                float NdotL = saturate(dot(normal, mainLight.direction));
                float3 directLight = baseColor * mainLight.color * NdotL * shadow;

                // --- AMBIENT LIGHT ---
                float3 ambient = baseColor * SampleSH(normal);

                // --- COMBINE ---
                float3 color = directLight + ambient;

                // Fog
                color = MixFog(color, i.fogFactor);
                
                return half4(color, 1);
            }

            ENDHLSL
        }
    }
}
