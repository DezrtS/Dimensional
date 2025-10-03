using UnityEngine;
using User_Interface;

namespace Scriptables.User_Interface
{
    [CreateAssetMenu(fileName = "WorldUIAnchorDatum", menuName = "Scriptable Objects/WorldUIAnchorDatum")]
    public abstract class WorldUIAnchorDatum : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        
        [SerializeField] private Vector3 offset;

        [SerializeField] private float minDistance;
        [SerializeField] private float maxDistance;
        [SerializeField] private float minAngle;
        [SerializeField] private float maxAngle;
        [SerializeField] private AnimationCurve sizeCurve;
        
        protected GameObject Prefab => prefab;
        public Vector3 Offset => offset;
        public float MinDistance => minDistance;
        public float MaxDistance => maxDistance;
        public float MinAngle => minAngle;
        public float MaxAngle => maxAngle;
        public AnimationCurve SizeCurve => sizeCurve;

        public abstract WorldUIAnchor SpawnWorldUIAnchor(Transform parent, Transform worldTransform);
    }
}