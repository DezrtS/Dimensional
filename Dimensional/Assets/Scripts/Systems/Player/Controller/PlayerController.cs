using System;
using Interfaces;
using Scriptables.Entities;
using Scriptables.Projectiles;
using Systems.Movement;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Player
{
    public class PlayerController : MonoBehaviour, IEntity, IMove, IAim
    {
        [SerializeField] private EntityDatum entityDatum;
        [SerializeField] private TextMeshProUGUI typeText;
        
        [SerializeField] private ProjectileDatum projectileDatum;
        [SerializeField] private float fireSpeed;
        
        private PlayerMovementController _playerMovementController;
        private PlayerLook _playerLook;
        
        private PlayerInputSystem_Actions _playerInputSystemActions;
        private InputAction _moveInputAction;
        private InputAction _lookInputAction;

        public EntityDatum EntityDatum => entityDatum;

        private void Awake()
        {
            _playerMovementController = GetComponent<PlayerMovementController>();
            _playerMovementController.Initialize(this);
            //_playerMovementController.ShapeTypeChanged += PlayerMovementControllerOnShapeTypeChanged;
            
            _playerLook = GetComponent<PlayerLook>();
            _playerLook.Initialize(this);
            
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
            jumpInputAction.canceled += OnJump;
            jumpInputAction.Enable();
            
            var crouchInputAction = _playerInputSystemActions.Player.Crouch;
            crouchInputAction.performed += OnCrouch;
            crouchInputAction.canceled += OnCrouch;
            crouchInputAction.Enable();
            
            var actionInputAction = _playerInputSystemActions.Player.Action;
            actionInputAction.performed += OnPrimaryAction;
            actionInputAction.canceled += OnPrimaryAction;
            actionInputAction.Enable();
            
            var attackInputAction = _playerInputSystemActions.Player.Attack;
            attackInputAction.performed += OnAttack;
            attackInputAction.Enable();
            
            var switchInputAction = _playerInputSystemActions.Player.Switch;
            switchInputAction.performed += OnSwitch;
            switchInputAction.Enable();
        }
        
        Vector3 IMove.GetInput()
        {
            var input = _moveInputAction.ReadValue<Vector2>();
            return new Vector3(input.x, 0, input.y);
        }

        Vector3 IAim.GetInput()
        {
            return _lookInputAction.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            _playerMovementController.Move(Quaternion.Euler(0, _playerLook.XRotation, 0));
        }

        private void LateUpdate()
        {
            _playerLook.Look();
        }

        private void OnAim(InputAction.CallbackContext context)
        {
            
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed) _playerMovementController.Jump();
            else if (context.canceled) _playerMovementController.StopJumping();
        }
        
        private void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed) _playerMovementController.Crouch();
            else if (context.canceled) _playerMovementController.StopCrouching();
        }
        
        private void OnSwitch(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            if (input.x != 0)
            {
                //_playerMovementController.ChangeShape(input.x > 0 ? PlayerMovementController.ShapeType.Boomerang : PlayerMovementController.ShapeType.Spring);
            }
            else
            {
                //_playerMovementController.ChangeShape(input.y > 0 ? PlayerMovementController.ShapeType.Sphere : PlayerMovementController.ShapeType.Heavy);
            }
        }

        private void OnPrimaryAction(InputAction.CallbackContext context)
        {
            if (context.performed) _playerMovementController.Boomerang();
            else if (context.canceled) _playerMovementController.StopBoomerang();
            
            //var projectile = projectileDatum.Spawn();
            //var position = transform.position;
            //var velocity = _playerMovementController.ForceController.GetVelocity();
            //projectile.Fire(FireContext.Construct(position, position + velocity, fireSpeed, true));
        }
        
        private void OnAttack(InputAction.CallbackContext context)
        {
            _playerMovementController.Attack();
        }

        //private void PlayerMovementControllerOnShapeTypeChanged(PlayerMovementController.ShapeType oldValue, PlayerMovementController.ShapeType newValue)
        //{
        //    typeText.text = newValue.ToString();
        //}
    }
}
