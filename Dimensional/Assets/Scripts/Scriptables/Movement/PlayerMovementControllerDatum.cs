using UnityEngine;

namespace Scriptables.Movement
{
    [CreateAssetMenu(fileName = "PlayerMovementControllerDatum", menuName = "Scriptable Objects/Movement/PlayerMovementControllerDatum")]
    public class PlayerMovementControllerDatum : MovementControllerDatum
    {
        [SerializeField] private float jumpPower;
        
        public float JumpPower => jumpPower;
    }
}
