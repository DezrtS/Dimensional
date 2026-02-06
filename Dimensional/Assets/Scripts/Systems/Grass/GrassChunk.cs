using System.Collections.Generic;
using Managers;
using Systems.Player;
using UnityEngine;

namespace Systems.Grass
{
    public struct MaskData {
        public Vector3 position;
        public Vector3 size;
        public float rotation;
    };
    
    public class GrassChunk : MonoBehaviour
    {
        private static readonly int MaskCountProperty = Shader.PropertyToID("_MaskCount");
        private static readonly int MasksProperty = Shader.PropertyToID("_Masks");

        private const int MaxGrassMasks = 6;

        private ComputeBuffer _maskBuffer;
        
        private Bounds _chunkBounds;
        private GrassSettings _grassSettings;

        private List<MaskData> _grassMasks;
        private List<Texture2D> _grassMaskTextures;

        private Camera _camera;
        private Transform _cameraTransform;
        private Transform _playerTransform;
        
        private List<GrassInstance> _grassInstances;
        private bool _previousIsChunkVisible;
        
        public Bounds ChunkBounds => _chunkBounds;

        public void Initialize(Bounds chunkBounds, GrassSettings grassSettings)
        {
            _chunkBounds = chunkBounds;
            _grassSettings = grassSettings;
            _grassInstances = new List<GrassInstance>();
            
            _grassMasks = new List<MaskData>();
            _grassMaskTextures = new List<Texture2D>();
            
            QueryMasks();
            SetupMaskBuffer();
        }

        public void AddGrassInstance(GrassInstance grassInstance)
        {
            _grassInstances.Add(grassInstance);
        }

        private void Start()
        {
            _camera = CameraManager.Instance.Camera;;
            _cameraTransform = _camera.transform;
            _playerTransform = PlayerController.Instance.transform;
        }

        private void QueryMasks()
        {
            var results = new Collider[10];
            var size = Physics.OverlapBoxNonAlloc(_chunkBounds.center, _chunkBounds.extents, results, Quaternion.identity, _grassSettings.MaskLayer, QueryTriggerInteraction.Collide);
            size = Mathf.Clamp(size, 0, MaxGrassMasks);
            for (var i = 0; i < size; i++)
            {
                var grassMask = results[i].GetComponent<GrassMask>();
                AddGrassMask(
                    new MaskData
                    {
                        position = grassMask.transform.position,
                        size = grassMask.transform.lossyScale * 0.5f,
                        rotation = grassMask.transform.eulerAngles.y * Mathf.Deg2Rad,
                    }, 
                    grassMask.MaskTexture
                );
            }
        }

        public void AddGrassMask(MaskData maskData, Texture2D texture)
        {
            if (_grassMasks.Count >= MaxGrassMasks) return;
            _grassMasks.Add(maskData);
            _grassMaskTextures.Add(texture);
            
            _maskBuffer?.SetData(_grassMasks); 
        }

        private void SetupMaskBuffer()
        {
            _maskBuffer?.Release();
            
            _maskBuffer = new ComputeBuffer(
                MaxGrassMasks, 
                7 * sizeof(float),
                ComputeBufferType.Structured
            );
            _maskBuffer.SetData(_grassMasks);   
        }

        public void AssignMaskBuffer(int kernel, ComputeShader grassCompute)
        {
            var maskCount = Mathf.Min(_grassMasks.Count, MaxGrassMasks);
            grassCompute.SetBuffer(kernel, MasksProperty, _maskBuffer);
            grassCompute.SetInt(MaskCountProperty, maskCount);
            for (var i = 0; i < MaxGrassMasks; i++)
            {
                grassCompute.SetTexture(kernel, $"_MaskTexture{i}", i < maskCount ? _grassMaskTextures[i] : Texture2D.blackTexture);
            }
        }

        private bool IsChunkVisible()
        {
            var cameraPosition = _cameraTransform.position;
            var playerPosition = _playerTransform.position;
            var dist = Vector3.Distance(cameraPosition, _chunkBounds.center);
            if (dist > _grassSettings.MaxGrassDistance)
                return false;
            
            var closestPoint = _chunkBounds.ClosestPoint(playerPosition);
            var toChunk = (closestPoint - cameraPosition).normalized;
            var dot = Vector3.Dot(_cameraTransform.forward, toChunk);
            if (dot < _grassSettings.MinChunkDot)
                return false;
            
            if (dot > _grassSettings.MaxChunkDot)
                return true;
            
            var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
            return GeometryUtility.TestPlanesAABB(planes, _chunkBounds);
        }

        private void Update()
        {
            var isChunkVisible = IsChunkVisible();
            if (isChunkVisible == _previousIsChunkVisible) return;
            
            _previousIsChunkVisible = isChunkVisible;
            foreach (var grassInstance in _grassInstances)
            {
                grassInstance.IsVisible = isChunkVisible;
            }
        }

        private void OnDestroy()
        {
            ReleaseBuffers();
        }

        private void ReleaseBuffers()
        {
            _maskBuffer?.Release();
        }
        
        private void OnDrawGizmosSelected()
        {
            var center = _chunkBounds.center;
            var size = _chunkBounds.size;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
                center,
                size
            );
            
            Gizmos.color = Color.yellow;
            var xHalfSize = size.x * 0.5f;
            var yHalfSize = size.y * 0.5f;
            var zHalfSize = size.z * 0.5f;
            Gizmos.DrawSphere(center + new Vector3(-xHalfSize, yHalfSize, -zHalfSize), 0.5f);
            Gizmos.DrawSphere(center + new Vector3(xHalfSize, yHalfSize, -zHalfSize), 0.5f);
            Gizmos.DrawSphere(center + new Vector3(-xHalfSize, yHalfSize, zHalfSize), 0.5f);
            Gizmos.DrawSphere(center + new Vector3(xHalfSize, yHalfSize, zHalfSize), 0.5f);
            Gizmos.DrawSphere(center + new Vector3(-xHalfSize, -yHalfSize, -zHalfSize), 0.5f);
            Gizmos.DrawSphere(center + new Vector3(xHalfSize, -yHalfSize, -zHalfSize), 0.5f);
            Gizmos.DrawSphere(center + new Vector3(-xHalfSize, -yHalfSize, zHalfSize), 0.5f);
            Gizmos.DrawSphere(center + new Vector3(xHalfSize, -yHalfSize, zHalfSize), 0.5f);
        }
    }
}
