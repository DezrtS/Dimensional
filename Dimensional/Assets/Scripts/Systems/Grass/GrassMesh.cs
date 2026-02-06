using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Systems.Grass
{
    public struct GrassPaintCommand
    {
        public Vector2 uv;
        public float radius;
        public float strength;
    }
    
    public class GrassMesh : MonoBehaviour
    {
        private static readonly int GrassInteractionTextureProperty = Shader.PropertyToID("_GrassInteractionTex");
        
        private static readonly int InteractionTextureProperty = Shader.PropertyToID("_InteractionTex");
        private static readonly int PaintCountProperty = Shader.PropertyToID("_PaintCount");
        private static readonly int TextureResolutionProperty = Shader.PropertyToID("_TextureResolution");
        private static readonly int PaintCommandBufferProperty = Shader.PropertyToID("_PaintCommands");

        [SerializeField] private Texture2D maskTexture;
        [SerializeField] private Material overrideGrassMaterial;
        [Space] 
        [SerializeField] private int renderTextureSize = 64;
        [SerializeField] private ComputeShader grassPaintCompute;
        
        private readonly List<GrassInstance> _grassInstances = new List<GrassInstance>();
        
        private readonly List<GrassPaintCommand> _grassPaintCommands = new List<GrassPaintCommand>();
        private ComputeBuffer _paintCommandBuffer;
        
        [SerializeField] private RenderTexture grassInteractionTexture;
        
        public MeshFilter MeshFilter { get; private set; }

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }

        private void Awake()
        {
            MeshFilter = GetComponent<MeshFilter>();

            if (grassInteractionTexture) return;
            grassInteractionTexture = new RenderTexture(renderTextureSize, renderTextureSize, 0, RenderTextureFormat.RGFloat)
            {
                enableRandomWrite = true,
                graphicsFormat = GraphicsFormat.R16G16_SFloat,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            grassInteractionTexture.Create();
            
            Graphics.SetRenderTarget(grassInteractionTexture);
            GL.Clear(false, true, Color.black);
            Graphics.SetRenderTarget(null);
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue == GameState.Initializing) EffectManager.Instance.GrassSystem.AddGrassMesh(this);
        }
        
        public GrassInstance CreateGrassInstance(List<TriangleData> triangleData, GrassChunk grassChunk, ComputeShader grassCompute, Material grassMaterial, GrassSettings grassSettings)
        {
            if (overrideGrassMaterial) grassMaterial = overrideGrassMaterial;
            if (maskTexture)
            {
                var size = MeshFilter.sharedMesh.bounds.extents;
                var scale = transform.lossyScale;
                var grassMask = new MaskData
                {
                    position = MeshFilter.sharedMesh.bounds.center + transform.position,
                    size = new Vector3(size.x * scale.x, size.y * scale.y, size.z * scale.z),
                    rotation = transform.eulerAngles.y * Mathf.Deg2Rad,
                };
                grassChunk.AddGrassMask(grassMask, maskTexture);
            }
            var grassInstance = new GrassInstance(triangleData, grassChunk, grassCompute, grassMaterial, grassSettings);
            _grassInstances.Add(grassInstance);
            grassInstance.MaterialPropertyBlock.SetTexture(GrassInteractionTextureProperty, grassInteractionTexture);
            return grassInstance;
        }

        public void AddGrassPaintCommand(GrassPaintCommand grassPaintCommand)
        {
            _grassPaintCommands.Add(grassPaintCommand);
        }

        private void Update()
        {
            foreach (var grassInstance in _grassInstances)
            {
                grassInstance.Render();
            }
        }

        private void SetupPaintCommandBuffer()
        {
            _paintCommandBuffer?.Release();
            _paintCommandBuffer = new ComputeBuffer(
                _grassPaintCommands.Count,
                sizeof(float) * 4
            );
            _paintCommandBuffer.SetData(_grassPaintCommands);
        }

        private void LateUpdate()
        {
            if (_grassPaintCommands.Count == 0)
                return;
            
            SetupPaintCommandBuffer();
            var kernel = grassPaintCompute.FindKernel("CSMain");
            
            grassPaintCompute.SetTexture(kernel, InteractionTextureProperty, grassInteractionTexture);
            
            var paintCount = _grassPaintCommands.Count;
            grassPaintCompute.SetInt(PaintCountProperty, paintCount);
            grassPaintCompute.SetInt(TextureResolutionProperty, renderTextureSize);
            
            grassPaintCompute.SetBuffer(kernel, PaintCommandBufferProperty, _paintCommandBuffer);

            var groups = Mathf.CeilToInt(renderTextureSize / 8f);
            
            grassPaintCompute.Dispatch(kernel, groups, groups, 1);
            
            _grassPaintCommands.Clear();
        }

        private void OnDestroy()
        {
            foreach (var grassInstance in _grassInstances)
            {
                grassInstance.ReleaseBuffers();
            }
            
            _paintCommandBuffer?.Release();
            grassInteractionTexture.Release();
        }
    }
}
