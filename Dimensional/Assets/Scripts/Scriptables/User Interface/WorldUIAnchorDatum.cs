using UnityEngine;
using User_Interface;

namespace Scriptables.User_Interface
{
    [CreateAssetMenu(fileName = "WorldUIAnchorDatum", menuName = "Scriptable Objects/WorldUIAnchorDatum")]
    public class WorldUIAnchorDatum : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector3 offset;
        
        
        [SerializeField] private float range;

        [Header("UI Scaling")]
        [SerializeField] private bool useDistanceScaling;
        [SerializeField] private float minDistance;
        [SerializeField] private float maxDistance;
        
        [SerializeField] private bool useAngleScaling;
        [SerializeField] private float minAngle;
        [SerializeField] private float maxAngle;
        [SerializeField] private AnimationCurve sizeCurve;
        
        protected GameObject Prefab => prefab;
        public Vector3 Offset => offset;
        public float Range => range;
        public bool UseDistanceScaling => useDistanceScaling;
        public float MinDistance => minDistance;
        public float MaxDistance => maxDistance;
        public bool UseAngleScaling => useAngleScaling;
        public float MinAngle => minAngle;
        public float MaxAngle => maxAngle;
        public AnimationCurve SizeCurve => sizeCurve;

        public virtual WorldUIAnchor SpawnWorldUIAnchor(Transform parent, GameObject holderGameObject, Transform worldTransform)
        {
            var worldUIAnchorObject = Instantiate(Prefab, parent);
            var worldUIAnchor = worldUIAnchorObject.GetComponent<WorldUIAnchor>();
            worldUIAnchor.Initialize(this, holderGameObject, worldTransform);
            return worldUIAnchor;
        }
    }
}