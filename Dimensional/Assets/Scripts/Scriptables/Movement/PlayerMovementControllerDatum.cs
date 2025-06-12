using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "PlayerMovementControllerDatum", menuName = "Scriptable Objects/Movement/PlayerMovementControllerDatum")]
    public class PlayerMovementControllerDatum : MovementControllerDatum
    {
        [SerializeField] private float jumpPower;
        
        [SerializeField] private float jumpHeight;
        [SerializeField] private AnimationCurve jumpCurve;
        [SerializeField] private AnimationCurve fallCurve;
        
        public float JumpPower => jumpPower;
    }
}
