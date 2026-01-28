using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Rendering;

namespace Systems.Grass
{
    public class GrassChunk : MonoBehaviour
    {
        private static readonly int TrianglesProperty = Shader.PropertyToID("_Triangles");
        private static readonly int BladesProperty = Shader.PropertyToID("_Blades");
        private static readonly int TriangleCountProperty = Shader.PropertyToID("_TriangleCount");
        private static readonly int BladeHeightProperty = Shader.PropertyToID("_BladeHeight");
        private static readonly int BladesPerSquareUnitProperty = Shader.PropertyToID("_BladesPerSquareUnit");
        private static readonly int BladeSegmentsProperty = Shader.PropertyToID("_BladeSegments");

        private const int MaxBlades = 250000;
        
        private ComputeBuffer _triangleBuffer;
        private Bounds _chunkBounds;
        private Material _grassMaterial;
        
        private ComputeShader _grassCompute;
        private ComputeBuffer _bladeBuffer;
        private ComputeBuffer _argsBuffer;
        
        private GrassSettings _grassSettings;
        
        private MaterialPropertyBlock _materialPropertyBlock;

        private Camera _camera;
        private Transform _cameraTransform;

        public void Initialize(List<TriangleData> triangleData, Bounds chunkBounds, ComputeShader grassCompute, Material grassMaterial, GrassSettings grassSettings)
        {
            _triangleBuffer = new ComputeBuffer(triangleData.Count, sizeof(float) * 13);
            _triangleBuffer.SetData(triangleData);
            _chunkBounds = chunkBounds;
            _grassCompute = grassCompute;
            _grassMaterial = grassMaterial;
            _grassSettings = grassSettings;
            
            SetupBuffers();
            Dispatch();
        }

        public void UpdateGrassSettings(GrassSettings grassSettings)
        {
            _grassSettings = grassSettings;
            SetupBuffers();
            Dispatch();
        }

        private void Start()
        {
            _camera = CameraManager.Instance.Camera;;
            _cameraTransform = _camera.transform;
        }

        private void SetupBuffers()
        {
            _bladeBuffer?.Release();
            _bladeBuffer = new ComputeBuffer(MaxBlades, sizeof(float) * 8, ComputeBufferType.Append);
            _bladeBuffer.SetCounterValue(0);

            _argsBuffer?.Release();
            _argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            _argsBuffer.SetData(new uint[] { 0, 1, 0, 0, 0 });
        }

        private void Dispatch()
        {
            var kernel = _grassCompute.FindKernel("CSMain");

            _grassCompute.SetBuffer(kernel, TrianglesProperty, _triangleBuffer);
            _grassCompute.SetBuffer(kernel, BladesProperty, _bladeBuffer);
            _grassCompute.SetInt(TriangleCountProperty, _triangleBuffer.count);
            _grassCompute.SetInt(BladesPerSquareUnitProperty, _grassSettings.BladesPerSquareUnit);
            _grassCompute.SetFloat(BladeHeightProperty, _grassSettings.BladeHeight);
            
            var groups = Mathf.Max(1, Mathf.CeilToInt(_triangleBuffer.count / 64f));
            _grassCompute.Dispatch(kernel, groups, 1, 1);
            
            ComputeBuffer.CopyCount(_bladeBuffer, _argsBuffer, 0);

            var args = new uint[5];
            _argsBuffer.GetData(args);

            var bladeCount = args[0];
            args[0] = bladeCount * (_grassSettings.BladeSegments * 6 + 3);
            args[1] = 1;

            _argsBuffer.SetData(args);
        }


        private void Update()
        {
            var cameraPosition = _cameraTransform.position;
            var dist = Vector3.Distance(cameraPosition, _chunkBounds.center);
            if (dist > _grassSettings.MaxGrassDistance)
                return;
            
            var closestPoint = _chunkBounds.ClosestPoint(cameraPosition);
            var toChunk = (closestPoint - cameraPosition).normalized;
            var dot = Vector3.Dot(_cameraTransform.forward, toChunk);
            if (dot < _grassSettings.MinChunkDot)
                return;
            
            var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
            if (!GeometryUtility.TestPlanesAABB(planes, _chunkBounds))
                return;
            
            _materialPropertyBlock ??= new MaterialPropertyBlock();
            _materialPropertyBlock.SetBuffer(BladesProperty, _bladeBuffer);
            _materialPropertyBlock.SetInt(BladeSegmentsProperty, (int)_grassSettings.BladeSegments);

            Graphics.DrawProceduralIndirect(
                _grassMaterial,
                _chunkBounds,
                MeshTopology.Triangles,
                _argsBuffer,
                0,
                null,
                _materialPropertyBlock,
                ShadowCastingMode.Off
            );
        }

        private void OnDestroy()
        {
            ReleaseBuffers();
        }

        private void ReleaseBuffers()
        {
            _triangleBuffer?.Release();
            _bladeBuffer?.Release();
            _argsBuffer?.Release();
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
