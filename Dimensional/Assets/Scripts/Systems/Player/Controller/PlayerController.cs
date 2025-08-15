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
        
        
        private PlayerMovementController _playerMovementController;
        private PlayerLook _playerLook;
        
        private PlayerInputSystem_Actions _playerInputSystemActions;
        private InputAction _moveInputAction;
        private InputAction _lookInputAction;
        
        private IInteractable _interactable;

        public EntityDatum EntityDatum => entityDatum;
        public GameObject GameObject => gameObject;

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
            
            var glideInputAction = _playerInputSystemActions.Player.Glide;
            glideInputAction.performed += OnGlide;
            glideInputAction.canceled += OnGlide;
            glideInputAction.Enable();
            
            var grappleInputAction = _playerInputSystemActions.Player.Grapple;
            grappleInputAction.performed += OnGrapple;
            grappleInputAction.canceled += OnGrapple;
            grappleInputAction.Enable();
            
            var boomerangInputAction = _playerInputSystemActions.Player.Boomerang;
            boomerangInputAction.performed += OnBoomerang;
            boomerangInputAction.canceled += OnBoomerang;
            boomerangInputAction.Enable();
            
            var crouchInputAction = _playerInputSystemActions.Player.Crouch;
            crouchInputAction.performed += OnCrouch;
            crouchInputAction.canceled += OnCrouch;
            crouchInputAction.Enable();
            
            var attackInputAction = _playerInputSystemActions.Player.Attack;
            attackInputAction.performed += OnAttack;
            attackInputAction.Enable();
            
            var interactInputAction = _playerInputSystemActions.Player.Interact;
            interactInputAction.performed += OnInteract;
            interactInputAction.Enable();
        }
        
        private void UnassignControls()
        {
            _moveInputAction.Disable();
            
            _lookInputAction.performed -= OnAim;
            _lookInputAction.Disable();
            
            var jumpInputAction = _playerInputSystemActions.Player.Jump;
            jumpInputAction.performed -= OnJump;
            jumpInputAction.canceled -= OnJump;
            jumpInputAction.Disable();
            
            var glideInputAction = _playerInputSystemActions.Player.Glide;
            glideInputAction.performed -= OnGlide;
            glideInputAction.canceled -= OnGlide;
            glideInputAction.Disable();
            
            var grappleInputAction = _playerInputSystemActions.Player.Grapple;
            grappleInputAction.performed -= OnGrapple;
            grappleInputAction.canceled -= OnGrapple;
            grappleInputAction.Disable();
            
            var boomerangInputAction = _playerInputSystemActions.Player.Boomerang;
            boomerangInputAction.performed -= OnBoomerang;
            boomerangInputAction.canceled -= OnBoomerang;
            boomerangInputAction.Disable();
            
            var crouchInputAction = _playerInputSystemActions.Player.Crouch;
            crouchInputAction.performed -= OnCrouch;
            crouchInputAction.canceled -= OnCrouch;
            crouchInputAction.Disable();
            
            var attackInputAction = _playerInputSystemActions.Player.Attack;
            attackInputAction.performed -= OnAttack;
            attackInputAction.Disable();
            
            var interactInputAction = _playerInputSystemActions.Player.Interact;
            interactInputAction.performed -= OnInteract;
            interactInputAction.Disable();
        }

        private void OnDisable()
        {
            UnassignControls();
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

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            _interactable = interactable;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            if (_interactable != interactable) return;
            _interactable = null;
        }

        private void OnAim(InputAction.CallbackContext context)
        {
            
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed) _playerMovementController.Jump();
            else if (context.canceled) _playerMovementController.CutJump();
        }

        private void OnGlide(InputAction.CallbackContext context)
        {
            if (context.performed) _playerMovementController.Glide();
            else if (context.canceled) _playerMovementController.StopGliding();
        }

        private void OnBoomerang(InputAction.CallbackContext context)
        {
            if (context.performed) _playerMovementController.Boomerang();
            else if (context.canceled) _playerMovementController.StopBoomeranging();
        }
        
        private void OnGrapple(InputAction.CallbackContext context)
        {
            if (context.performed) _playerMovementController.Grapple();
            else if (context.canceled) _playerMovementController.StopGrappling();
        }
        
        private void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed) _playerMovementController.Crouch();
            else if (context.canceled) _playerMovementController.StopCrouching();
        }
        
        private void OnAttack(InputAction.CallbackContext context)
        {
            _playerMovementController.Attack();
        }
        
        private void OnInteract(InputAction.CallbackContext context)
        {
            _interactable?.Interact(InteractContext.Construct(gameObject));
        }
    }
}
