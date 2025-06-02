using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "MovementControllerDatum", menuName = "Scriptable Objects/Movement/MovementControllerDatum")]
    public class MovementControllerDatum : ScriptableObject
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        
        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
    }
}