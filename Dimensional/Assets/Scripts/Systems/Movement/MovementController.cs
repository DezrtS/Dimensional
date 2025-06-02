using Interfaces;
using Scriptables.Movement;
using UnityEngine;

namespace Systems.Movement
{
    public abstract class MovementController : MonoBehaviour
    {
        [SerializeField] private MovementControllerDatum movementControllerDatum;
        
        protected bool IsDisabled { get; set; }
        private IMove _move;
        
        protected ForceController ForceController { get; private set; }
        
        public MovementControllerDatum MovementControllerDatum => movementControllerDatum;

        private void Awake()
        {
            ForceController = GetComponent<ForceController>();
        }

        public void Initialize(IMove move)
        {
            _move = move;
        }

        public void Move()
        {
            if (IsDisabled) return;
            Move(_move?.GetInput() ?? Vector3.zero);
        }
        
        public void Move(Quaternion rotation)
        {
            if (IsDisabled) return;
            Move(TransformInput(_move?.GetInput() ?? Vector3.zero, rotation));
        }

        protected abstract void Move(Vector3 input);

        private static Vector3 TransformInput(Vector3 input, Quaternion rotation)
        {
            return rotation * input;
        }
    }
}