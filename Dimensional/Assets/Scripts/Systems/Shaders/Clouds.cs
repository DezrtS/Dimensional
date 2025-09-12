using System;
using Managers;
using UnityEngine;
using UnityEngine.Rendering;

namespace Systems.Shaders
{
    public class Clouds : MonoBehaviour
    {
        [SerializeField] private int horizontalStackSize = 20;
        [SerializeField] private float cloudHeight;
        [SerializeField] private Mesh quadMesh;
        [SerializeField] private Material cloudMaterial;

        [SerializeField] private int renderingLayer = 0; // Layer index (0-31)
        [SerializeField] private uint renderingLayerMask = 1; // Bitmask for rendering layers (1 << layerIndex)

        private float _offset;
        private Camera _camera;
        private Matrix4x4[] _matrices;
        private MaterialPropertyBlock _props;
        private RenderParams _renderParams;
        private bool _initialized;
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private Vector3 _lastScale;
        private int _lastStackSize;

        private void Awake()
        {
            _props = new MaterialPropertyBlock();
            _matrices = new Matrix4x4[horizontalStackSize];
            
            // Setup RenderParams
            _renderParams = new RenderParams(cloudMaterial)
            {
                layer = renderingLayer,
                renderingLayerMask = renderingLayerMask,
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = false,
                worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000f) // Large bounds to always render
            };

            var instance = CameraManager.Instance;
            if (instance)
            {
                CameraManagerOnInitialized(instance);
            }
            else
            {
                CameraManager.Initialized += CameraManagerOnInitialized;
            }
        }

        private void OnDestroy()
        {
            CameraManager.Initialized -= CameraManagerOnInitialized;
        }

        private void CameraManagerOnInitialized(CameraManager instance)
        {
            _camera = instance.Camera;
            _initialized = true;
        }

        private void OnValidate()
        {
            // Update layer mask when properties change in editor
            if (renderingLayer >= 0 && renderingLayer < 32)
            {
                renderingLayerMask = (uint)(1 << renderingLayer);
            }
        }

        private void Update()
        {
            if (!_initialized || _camera == null) return;

            // Update render params if changed
            if (_renderParams.layer != renderingLayer || 
                _renderParams.renderingLayerMask != renderingLayerMask)
            {
                _renderParams.layer = renderingLayer;
                _renderParams.renderingLayerMask = renderingLayerMask;
            }

            // Recalculate matrices if needed
            if (transform.hasChanged || _lastStackSize != horizontalStackSize)
            {
                RecalculateMatrices();
                transform.hasChanged = false;
                _lastStackSize = horizontalStackSize;
            }

            // Update material properties
            _offset = cloudHeight / horizontalStackSize / 2f;
            _props.SetFloat("_midYValue", transform.position.y);
            _props.SetFloat("_cloudHeight", cloudHeight);
            _props.SetVector("_cameraPosition", _camera.transform.position);

            // Assign property block to render params
            _renderParams.matProps = _props;

            // Render all instances
            Graphics.RenderMeshInstanced(
                _renderParams,
                quadMesh,
                0,
                _matrices,
                horizontalStackSize
            );
        }

        private void RecalculateMatrices()
        {
            // Resize array if stack size changed
            if (_matrices.Length != horizontalStackSize)
            {
                _matrices = new Matrix4x4[horizontalStackSize];
            }

            Vector3 startPosition = transform.position + Vector3.up * (cloudHeight / 2f);
            
            for (int i = 0; i < horizontalStackSize; i++)
            {
                Vector3 position = startPosition - Vector3.up * (cloudHeight * i / horizontalStackSize);
                _matrices[i] = Matrix4x4.TRS(
                    position,
                    transform.rotation,
                    transform.localScale
                );
            }
        }
    }
}