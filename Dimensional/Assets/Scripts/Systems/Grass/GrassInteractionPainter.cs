using System;
using UnityEngine;

namespace Systems.Grass
{
    [Serializable]
    public struct GrassEffectData
    {
        [SerializeField] private float interactionRadius;
        [SerializeField] private float interactionStrength;
        
        [SerializeField] private LayerMask interactionLayerMask;
        
        [SerializeField] private float checkDistance;
        [SerializeField] private bool scaleDistance;
        [SerializeField] private Vector2 scaleRange;
        
        public float InteractionRadius => interactionRadius;
        public float InteractionStrength => interactionStrength;
        public LayerMask InteractionLayerMask => interactionLayerMask;
        public float CheckDistance => checkDistance;
        public bool ScaleDistance => scaleDistance;
        public Vector2 ScaleRange => scaleRange;
    }
    
    public class GrassInteractionPainter : MonoBehaviour
    {
        [SerializeField] private GrassEffectData grassEffectData;
        
        [SerializeField] private float interactionRadius;
        [SerializeField] private float interactionStrength;
        
        [SerializeField] private LayerMask interactionLayerMask;
        
        [SerializeField] private float checkDistance;
        [SerializeField] private bool scaleDistance;
        [SerializeField] private Vector2 scaleRange;
        
        private Collider _previousHitCollider;
        private GrassMesh _previousGrassMesh;

        private void FixedUpdate()
        {
            if (!Physics.Raycast(transform.position, Vector3.down, out var hit, checkDistance, interactionLayerMask)) return;
            if (hit.collider != _previousHitCollider)
            {
                _previousHitCollider = hit.collider;
                _previousGrassMesh = _previousHitCollider.GetComponent<GrassMesh>();
            }

            if (!_previousGrassMesh) return;
            
            var meshBounds = _previousGrassMesh.MeshFilter.sharedMesh.bounds;
            var worldToUV = 1f / (Mathf.Max(meshBounds.extents.x, meshBounds.extents.z) * Mathf.Max(_previousGrassMesh.transform.lossyScale.x, _previousGrassMesh.transform.lossyScale.z));
            var uvRadius = interactionRadius * worldToUV;
            
            var strength = interactionStrength;

            if (scaleDistance)
            {
                var distanceRatio = 1 - (hit.distance - scaleRange.x) / scaleRange.y;
                strength = Mathf.Clamp01(strength * distanceRatio);
            }
            
            var uv = ComputeHitUV(hit, _previousGrassMesh.MeshFilter.sharedMesh);
            _previousGrassMesh.AddGrassPaintCommand(new GrassPaintCommand {radius = uvRadius, strength = strength, uv = uv});
        }

        public static Vector2 ComputeHitUV(RaycastHit hit, Mesh mesh)
        {
            var triangleIndex = hit.triangleIndex * 3;
            var triangles = mesh.triangles;
            var uvs = mesh.uv;

            var i0 = triangles[triangleIndex + 0];
            var i1 = triangles[triangleIndex + 1];
            var i2 = triangles[triangleIndex + 2];

            var barycentricCoordinate = hit.barycentricCoordinate;

            return
                uvs[i0] * barycentricCoordinate.x +
                uvs[i1] * barycentricCoordinate.y +
                uvs[i2] * barycentricCoordinate.z;
        }
    }
}
