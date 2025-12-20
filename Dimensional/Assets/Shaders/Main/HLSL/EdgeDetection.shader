Shader "Hidden/Edge Detection"
{
    Properties
    {
        // … (we no longer use the old "_OutlineThickness" in pixels)
        //_OutlineThickness ("Outline Thickness (Pixels)", Float) = 1  

        // NEW: thickness in WORLD UNITS (for example, 0.05 world units)
        _OutlineThicknessWorld ("Outline Thickness (World)", Float) = 0.05

        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)

        // These two get overridden every frame by the C# pass
        _TanHalfFOV     ("TanHalfFOV", Float)       = 0.5  // placeholder
        _ScreenHeightPx ("ScreenHeightPx", Float)   = 1080 // placeholder
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"    = "Opaque"
        }

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            Name "EDGE DETECTION OUTLINE"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            

            float _OutlineThicknessWorld;  // world‐space thickness
            float4 _OutlineColor;

            // NEW:
            float _TanHalfFOV;
            float _ScreenHeightPx;

            //----------------------------------------------------------------
            // Roberts Cross kernels (unchanged)
            float RobertsCross(float3 samples[4])
            {
                const float3 d1 = samples[1] - samples[2];
                const float3 d2 = samples[0] - samples[3];
                return sqrt(dot(d1, d1) + dot(d2, d2));
            }

            float RobertsCross(float samples[4])
            {
                const float d1 = samples[1] - samples[2];
                const float d2 = samples[0] - samples[3];
                return sqrt(d1 * d1 + d2 * d2);
            }

            float3 SampleSceneNormalsRemapped(float2 uv)
            {
                return SampleSceneNormals(uv) * 0.5 + 0.5;
            }

            float SampleSceneLuminance(float2 uv)
            {
                float3 col = SampleSceneColor(uv);
                return col.r * 0.3 + col.g * 0.59 + col.b * 0.11;
            }

            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 uv = IN.texcoord;

                // 1) Read the *hardware* depth, then convert to a LINEAR view‐space Z.
                //    Note: UnityLinearEyeDepth returns a positive Z in eye‐space (0 at near plane, increasing as you go “into” the scene).
                float rawDepth   = SampleSceneDepth(uv);
                // new, correct call:
                float viewZ = LinearEyeDepth(rawDepth, UNITY_REVERSED_Z);


                // 2) Compute how many *screen pixels* correspond to 1 world‐unit at that viewZ:
                //    world‐height‐at‐depth = 2 * tan(FOV/2) * viewZ
                //    so worldUnitsPerPixel = (2 * tanHalfFOV * viewZ) / screenHeight
                //    invert → pixelsPerWorldUnit:
                float pxPerWU    = _ScreenHeightPx / (2.0 * _TanHalfFOV * viewZ);

                // 3) desired thickness in screen‐pixels:
                float thicknessPx = _OutlineThicknessWorld * pxPerWU;

                // 4) now divide by ScreenParams to get a UV‐offset “in texture‐space”:
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

                // We want to sample four diagonals around the center at +-half of thicknessPx
                float half_f = floor(thicknessPx * 0.5);
                float half_c = ceil (thicknessPx * 0.5);

                float2 uvs[4];
                uvs[0] = uv + texelSize * float2( half_f,  half_c) * float2(-1,  1); // top‐left
                uvs[1] = uv + texelSize * float2( half_c,  half_c) * float2( 1,  1); // top‐right
                uvs[2] = uv + texelSize * float2( half_f,  half_f) * float2(-1, -1); // bottom‐left
                uvs[3] = uv + texelSize * float2( half_c,  half_f) * float2( 1, -1); // bottom‐right

                // 5) Sample depth, normals, luminance at those 4 offsets:
                float depthSamples[4];
                float3 normalSamples[4];
                float lumSamples[4];

                #pragma unroll
                for (int i = 0; i < 4; i++)
                {
                    depthSamples[i]  = SampleSceneDepth(uvs[i]);
                    normalSamples[i] = SampleSceneNormalsRemapped(uvs[i]);
                    lumSamples[i]    = SampleSceneLuminance(uvs[i]);
                }

                // 6) Run Roberts Cross on each channel
                float edgeDepth     = RobertsCross(depthSamples);
                float edgeNormal    = RobertsCross(normalSamples);
                float edgeLum       = RobertsCross(lumSamples);

                // 7) Threshold each
                float depthThresh     = 1.0 / 200.0;
                float normalThresh    = 1.0 / 4.0;
                float luminanceThresh = 1.0 / 0.5;

                edgeDepth  = edgeDepth  > depthThresh     ? 1 : 0;
                edgeNormal = edgeNormal > normalThresh   ? 1 : 0;
                edgeLum    = edgeLum    > luminanceThresh? 1 : 0;

                float edge = max(edgeDepth, max(edgeNormal, edgeLum));

                // 8) Color it
                return edge * _OutlineColor;
            }
            ENDHLSL
        }
    }
}
