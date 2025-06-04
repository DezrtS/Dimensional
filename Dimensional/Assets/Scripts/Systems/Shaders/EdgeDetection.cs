using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Systems.Shaders
{
    public class EdgeDetection : ScriptableRendererFeature
    {
        //--------------- unchanged ---------------
        private class EdgeDetectionPass : ScriptableRenderPass
        {
            private Material material;

            // old: private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
            // old: private static readonly int OutlineColorProperty    = Shader.PropertyToID("_OutlineColor");
            // We add two more uniform IDs:
            private static readonly int OutlineThicknessWorldProperty = Shader.PropertyToID("_OutlineThicknessWorld");
            private static readonly int OutlineColorProperty          = Shader.PropertyToID("_OutlineColor");
            private static readonly int TanHalfFOVProperty             = Shader.PropertyToID("_TanHalfFOV");
            private static readonly int ScreenHeightProperty           = Shader.PropertyToID("_ScreenHeightPx");

            public EdgeDetectionPass()
            {
                profilingSampler = new ProfilingSampler(nameof(EdgeDetectionPass));
            }

            // We now pass in "outlineThicknessWorld" instead of "outlineThickness"
            public void Setup(ref EdgeDetectionSettings settings, ref Material edgeDetectionMaterial, Camera cam)
            {
                material = edgeDetectionMaterial;
                renderPassEvent = settings.renderPassEvent;

                // 1) Set the world‐space thickness (user sets this in Inspector, e.g. 0.05)
                material.SetFloat(OutlineThicknessWorldProperty, settings.outlineThicknessWorld);

                // 2) Set the outline color as before
                material.SetColor(OutlineColorProperty, settings.outlineColor);

                // 3) Compute tan(FOV/2) & screen‐height, send to shader
                float fovRad       = cam.fieldOfView * Mathf.Deg2Rad;
                float tanHalfFOV   = Mathf.Tan(fovRad * 0.5f);
                float screenHeight = (float)cam.pixelHeight;

                material.SetFloat(TanHalfFOVProperty, tanHalfFOV);
                material.SetFloat(ScreenHeightProperty, screenHeight);
            }

            private class PassData { }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();

                using var builder = renderGraph.AddRasterRenderPass<PassData>("Edge Detection", out _);

                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                builder.UseAllGlobalTextures(true);
                builder.AllowPassCulling(false);
                builder.SetRenderFunc((PassData _, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, Vector2.one, material, 0);
                });
            }
        }

        [Serializable]
        public class EdgeDetectionSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

            [Tooltip("Outline thickness in world-units (e.g. 0.05 means 0.05 world-units)")]
            public float outlineThicknessWorld = 0.05f;

            public Color outlineColor = Color.black;
        }

        [SerializeField] private EdgeDetectionSettings settings;
        private Material edgeDetectionMaterial;
        private EdgeDetectionPass edgeDetectionPass;

        public override void Create()
        {
            edgeDetectionPass ??= new EdgeDetectionPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview
                || renderingData.cameraData.cameraType == CameraType.Reflection
                || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
                return;

            if (edgeDetectionMaterial == null)
            {
                edgeDetectionMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Edge Detection"));
                if (edgeDetectionMaterial == null)
                {
                    Debug.LogWarning("Not all required materials could be created. Edge Detection will not render.");
                    return;
                }
            }

            // We need linear depth & normals & color
            edgeDetectionPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
            edgeDetectionPass.requiresIntermediateTexture = true;

            // Pass in the camera so the pass can compute TanHalfFOV and ScreenHeight
            edgeDetectionPass.Setup(ref settings, ref edgeDetectionMaterial, renderingData.cameraData.camera);
            renderer.EnqueuePass(edgeDetectionPass);
        }

        protected override void Dispose(bool disposing)
        {
            edgeDetectionPass = null;
            CoreUtils.Destroy(edgeDetectionMaterial);
        }
    }
}
