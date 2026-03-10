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
        private static readonly int OffsetProperty = Shader.PropertyToID("_Offset");
        
        private const int MaxPaintCommands = 16;

        [SerializeField] private Texture2D maskTexture;
        [SerializeField] private Material overrideGrassMaterial;
        [Space] 
        [SerializeField] private int renderTextureSize = 64;
        [SerializeField] private ComputeShader grassPaintCompute;
        
        private readonly List<GrassInstance> _grassInstances = new List<GrassInstance>();
        
        private readonly List<GrassPaintCommand> _grassPaintCommands = new List<GrassPaintCommand>();
        private ComputeBuffer _paintCommandBuffer;

        private Vector3 _startPosition;
        private int _kernel;
        private int _groups;
        
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
            _startPosition = transform.position;
            MeshFilter = GetComponent<MeshFilter>();
            
            if (!grassInteractionTexture)
            {
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
            else
            {
                renderTextureSize = grassInteractionTexture.width;
            }
            
            _kernel = grassPaintCompute.FindKernel("CSMain");
            _groups = Mathf.CeilToInt(renderTextureSize / 8f);

            _paintCommandBuffer = new ComputeBuffer(MaxPaintCommands, sizeof(float) * 4);

            grassPaintCompute.SetTexture(_kernel, InteractionTextureProperty, grassInteractionTexture);
            grassPaintCompute.SetBuffer(_kernel, PaintCommandBufferProperty, _paintCommandBuffer);
            grassPaintCompute.SetInt(TextureResolutionProperty, renderTextureSize);
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue == GameState.Initializing) EffectManager.Instance.GrassSystem.AddGrassMesh(this);
        }
        
        public GrassInstance CreateGrassInstance(List<TriangleData> triangleData, GrassChunk grassChunk, ComputeShader grassCompute, Material grassMaterial, GrassSettings grassSettings)
        {
            if (overrideGrassMaterial) grassMaterial = overrideGrassMaterial;
            if (maskTexture) grassChunk.SetUVGrassMask(maskTexture);
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
            var offset = transform.position - _startPosition;
            foreach (var grassInstance in _grassInstances)
            {
                grassInstance.MaterialPropertyBlock.SetVector(OffsetProperty, offset);
                grassInstance.Render();
            }
        }

        private void LateUpdate()
        {
            var paintCount = _grassPaintCommands.Count;
            if (paintCount == 0)
                return;

            paintCount = Mathf.Min(paintCount, MaxPaintCommands);
            if (_grassPaintCommands.Count > MaxPaintCommands) _grassPaintCommands.RemoveRange(MaxPaintCommands, paintCount - MaxPaintCommands);

            grassPaintCompute.SetInt(PaintCountProperty, paintCount);
            try
            {
                _paintCommandBuffer.SetData(_grassPaintCommands);
            }
            catch (Exception ex)
            {
                _grassPaintCommands.Clear();
            }
            grassPaintCompute.SetBuffer(_kernel, PaintCommandBufferProperty, _paintCommandBuffer);
            
            grassPaintCompute.SetTexture(_kernel, InteractionTextureProperty, grassInteractionTexture);
            grassPaintCompute.SetInt(TextureResolutionProperty, renderTextureSize);

            grassPaintCompute.Dispatch(_kernel, _groups, _groups, 1);

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
