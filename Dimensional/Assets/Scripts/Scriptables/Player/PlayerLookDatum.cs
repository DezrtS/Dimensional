using UnityEngine;

namespace Scriptables.Player
{
    [CreateAssetMenu(fileName = "PlayerLookDatum", menuName = "Scriptable Objects/Player/PlayerLookDatum")]
    public class PlayerLookDatum : ScriptableObject
    {
        [Header("2D")] 
        [SerializeField] private float positionSensitivity;
        [SerializeField] private float range;
        [SerializeField] private float resetDelay;
        [Space]
        [Header("3D")]
        [SerializeField] private bool invertX;
        [SerializeField] private bool invertY;
        [SerializeField] private float xSensitivity;
        [SerializeField] private float ySensitivity;
        [SerializeField] private float maxYAngle;
        
        public bool InvertX => invertX;
        public bool InvertY => invertY;
        
        public float PositionSensitivity => positionSensitivity;
        public float Range => range;
        public float ResetDelay => resetDelay;
        
        public float XSensitivity => xSensitivity;
        public float YSensitivity => ySensitivity;
        public float MaxYAngle => maxYAngle;
    }
}
