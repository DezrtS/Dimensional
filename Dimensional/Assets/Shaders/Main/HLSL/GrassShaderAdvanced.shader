Shader "Custom/ProceduralGrass"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.35, 0.9, 0.4, 1)
        _BottomColor ("Bottom Color", Color) = (0.15, 0.6, 0.25, 1)
        _GradientOffset ("Gradient Offset", Range(-1,1)) = 0
        
        _BladeHeight ("Blade Height", Float) = 1
        _BladeWidth ("Blade Width", Float) = 1
        _WidthTaper ("Width Taper", Float) = 1
        _BladeSegments ("Blade Segments", Int) = 2
        _BladeBendFactor ("Blade Bend Factor", Range(0, 1)) = 0.5
        
        _InteractionBendStrength ("Interaction Bend Strength", Float) = 1
        
        _WindTex ("Wind Noise Texture", 2D) = "white" {}
        _WindStrength ("Wind Strength", Float) = 0.3
        _WindAngle ("Wind Angle (Radians)", Float) = 0
        _WindSpeed ("Wind Speed", Float) = 1.0
        _WindScale ("Wind Scale", Float) = 0.1
       
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        
        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            #pragma multi_compile_fog
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            struct GrassBlade
            {
                float3 position;
                float3 normal;
                float2 uv;
                float height;
                float rotation;
            };

            StructuredBuffer<GrassBlade> _GrassBlades;

            float4 _TopColor;
            float4 _BottomColor;
            float  _GradientOffset;

            float _BladeHeight;
            float _BladeWidth;
            float _WidthTaper;
            int _BladeSegments;
            float _BladeBendFactor;

            TEXTURE2D(_GrassInteractionTex);
            SAMPLER(sampler_GrassInteractionTex);

            float _InteractionBendStrength;
            
            TEXTURE2D(_WindTex);
            SAMPLER(sampler_WindTex);

            float _WindStrength;
            float _WindAngle;
            float _WindSpeed;
            float _WindScale;

            float3 _Offset;

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 normal   : TEXCOORD0;
                float height01    : TEXCOORD1;
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


            Varyings vert (uint vertexID : SV_VertexID)
            {
                Varyings o;

                uint trisPerBlade = 2 * _BladeSegments - 1;
                uint vertsPerBlade = trisPerBlade * 3;

                uint bladeID = vertexID / vertsPerBlade;
                uint localVertex = vertexID % vertsPerBlade;

                GrassBlade grassBlade = _GrassBlades[bladeID];

                uint triID = localVertex / 3;
                uint corner = localVertex % 3;

                float3 up = normalize(grassBlade.normal);

                float3 right = normalize(cross(up, float3(0,0,1)));
                if (length(right) < 0.01)
                    right = float3(1,0,0);
                
                float s = sin(grassBlade.rotation);
                float c = cos(grassBlade.rotation);

                float3 rotatedRight = right * c + cross(up, right) * s;

                float3 forward = normalize(cross(up, rotatedRight));
                float halfWidth = _BladeWidth * 0.5;

                float3 position = grassBlade.position + _Offset;
                float height = grassBlade.height * _BladeHeight;

                uint segPair = triID / 2;

                float t0 = (float)segPair / _BladeSegments;
                float t1 = (float)(segPair + 1) / _BladeSegments;
                float height01;
                
                if (triID < trisPerBlade - 1)
                {
                    float3 left0  = position + up * (height * t0) - rotatedRight * halfWidth;
                    float3 right0 = position + up * (height * t0) + rotatedRight * halfWidth;
                    float3 left1  = position + up * (height * t1) - rotatedRight * halfWidth;
                    float3 right1 = position + up * (height * t1) + rotatedRight * halfWidth;

                    if (triID % 2 == 0)
                    {
                        height01 = t1;
                        if (corner == 0)
                        {
                            position = left0;
                            height01 = t0;
                        }
                        if (corner == 1) position = left1;
                        if (corner == 2) position = right1;
                    }
                    else
                    {
                        height01 = t0;
                        if (corner == 0) position = left0;
                        if (corner == 1)
                        {
                            position = right1;
                            height01 = t1;
                        }
                        if (corner == 2) position = right0;
                    }
                }
                else
                {
                    float3 left0  = position + up * (height * t0) - rotatedRight * halfWidth;
                                        float3 up0  = position + up * (height * t1);
                    float3 right0 = position + up * (height * t0) + rotatedRight * halfWidth;

                    height01 = t0;
                    if (corner == 0) position = left0;
                    if (corner == 1)
                    {
                        position = up0;
                        height01 = t1;
                    }
                    if (corner == 2) position = right0;
                }
                
                float4 interaction = SAMPLE_TEXTURE2D_LOD(_GrassInteractionTex, sampler_GrassInteractionTex, grassBlade.uv, 0);
                float bendStrength = interaction.r * _InteractionBendStrength * height01;
                
                float curve = pow(height01, 2.0) * _BladeBendFactor;
                
                
                float3 forwardBend = forward * curve * height + float3(0, -bendStrength, 0);
                
                // --- WIND DIRECTION ---
                float2 windDirXZ = float2(cos(_WindAngle), sin(_WindAngle));
                float3 windDirWS = float3(windDirXZ.x, 0, windDirXZ.y);

                float windNoise = SampleWind(position, windDirXZ);

                float bend = windNoise * _WindStrength * height01;

                // --- POSITION BEND ---
                position += windDirWS * bend + forwardBend;

                // --- NORMAL BEND ---
                // Tilt normal toward wind direction
                float3 bentNormal = normalize(
                    up + windDirWS * bend * 0.75 + forwardBend
                );

                float4 clipPosition = TransformWorldToHClip(float4(position, 1));
                o.vertex = clipPosition;
                o.normal = bentNormal;
                o.height01 = height01;
                o.fogFactor = ComputeFogFactor(clipPosition.z);
                o.shadowCoord = TransformWorldToShadowCoord(position);
                return o;
            }


            half4 frag (Varyings i) : SV_Target
            {
                float h = saturate(i.height01 + _GradientOffset * 0.5);
                float3 baseColor = lerp(_BottomColor.rgb, _TopColor.rgb, h);

                float3 normal = normalize(i.normal);
                
                Light mainLight = GetMainLight(i.shadowCoord);
                float shadow = lerp(0.7, 1.0, mainLight.shadowAttenuation);
                float NdotL = saturate(dot(normal, mainLight.direction));
                float3 directLight = baseColor * mainLight.color * NdotL * shadow;

                // --- AMBIENT LIGHT ---
                float3 ambient = baseColor * SampleSH(normal);

                // --- COMBINE ---
                float3 color = directLight + ambient;

                // Fog
                color = MixFog(color, i.fogFactor);

                //return shadow;
                
                return half4(color, 1);
            }

            ENDHLSL
        }
    }
}
