using System;
using Interfaces;
using Scriptables.Entities;
using Systems.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Player
{
    public class PlayerController : MonoBehaviour, IEntity, IMove, IAim
    {
        [SerializeField] private EntityDatum entityDatum;
        
        private PlayerMovementController _playerMovementController;
        private PlayerInputSystem_Actions _playerInputSystemActions;
        private InputAction _moveInputAction;
        private InputAction _lookInputAction;

        public EntityDatum EntityDatum => entityDatum;

        private void Awake()
        {
            _playerMovementController = GetComponent<PlayerMovementController>();
            _playerMovementController.Initialize(this);
            AssignControls();
        }

        private void AssignControls()
        {
            _playerInputSystemActions ??= new PlayerInputSystem_Actions();
            
            _moveInputAction ??= _playerInputSystemActions.Player.Move;
            _moveInputAction.Enable();
            
            _lookInputAction ??= _playerInputSystemActions.Player.Look;
            _lookInputAction.performed += OnAim;
            _lookInputAction.Enable();
            
            var jumpInputAction = _playerInputSystemActions.Player.Jump;
            jumpInputAction.performed += OnJump;
            jumpInputAction.Enable();
        }
        
        Vector3 IMove.GetInput()
        {
            var input = _moveInputAction.ReadValue<Vector2>();
            return new Vector3(input.x, 0, input.y);
        }

        Vector3 IAim.GetInput()
        {
            return _lookInputAction.ReadValue<Vector3>();
        }

        private void FixedUpdate()
        {
            _playerMovementController.Move();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            
        }

        private void OnAim(InputAction.CallbackContext context)
        {
            
        }
        
        private void OnBend(InputAction.CallbackContext context)
        {
            
        }

        private void OnPrimaryAction(InputAction.CallbackContext context)
        {
            
        }
    }
}
