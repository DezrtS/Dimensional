using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Scriptables.Actions.Movement;
using Scriptables.Entities;
using Scriptables.Shapes;
using Systems.Actions.Movement;
using Systems.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Systems.Player
{
    public class PlayerController : Singleton<PlayerController>, IEntity, IMove, IAim
    {
        [SerializeField] private bool setCameraFollowOnStart;
        [Space]
        [SerializeField] private EntityDatum entityDatum;
        [SerializeField] private ShapeDatum[] shapeData;
        [SerializeField] private MovementActionShapesPreset defaultMovementActionShapesPreset;
        [SerializeField] private MovementActionShapesPreset[] movementActionShapesPresets;
        
        private PlayerMovementController _playerMovementController;

        private InputActionMap _inputActionMap;
        private InputAction _moveInputAction;
        private InputAction _lookInputAction;
        
        private IInteractable _interactable;

        public EntityDatum EntityDatum => entityDatum;
        public GameObject GameObject => gameObject;
        public uint Id { get; private set; }
        public bool DebugDisable { get; set; }
        public PlayerLook PlayerLook { get; private set; }

        public List<MovementActionShape> MovementActionShapes { get; private set; }
        private Dictionary<ShapeType, ShapeDatum> ShapeData { get; set; }

        public MovementActionDatum GetMovementActionDatum(MovementActionType movementActionType)
        {
            var movementActionShape = Array.Find(MovementActionShapes.ToArray(), x => x.MovementActionType == movementActionType);
            if (movementActionShape == null) return null;
            var movementActionDictionary = ShapeData[movementActionShape.ShapeType].DefineMovementActions();
            return movementActionDictionary[movementActionType];
        }
        
        public void SetMovementActionShape(MovementActionType movementActionType, ShapeType shapeType)
        {
            for (var i = 0; i < MovementActionShapes.Count; i++)
            {
                if (MovementActionShapes[i].MovementActionType != movementActionType) continue;
                MovementActionShapes[i] = new MovementActionShape(movementActionType, shapeType);
                return;
            }
            
            MovementActionShapes.Add(new MovementActionShape(movementActionType, shapeType));
        }

        public void ResetMovementActions() => _playerMovementController.ResetMovementActions();

        public void SetMovementActionShapesPreset(MovementActionShapesPreset movementActionShapesPreset, bool resetMovementActions = true)
        {
            MovementActionShapes = new List<MovementActionShape>();
            foreach (var movementActionShape in movementActionShapesPreset.MovementActionShapes)
            {
                MovementActionShapes.Add(movementActionShape);
            }
            
            if (resetMovementActions) ResetMovementActions();
        }

        private void Awake()
        {
            Id = EntityManager.GetNextEntityId();
            
            ShapeData = new Dictionary<ShapeType, ShapeDatum>();
            foreach (var shapeDatum in shapeData)
            {
                ShapeData.Add(shapeDatum.ShapeType, shapeDatum);
            }

            SetMovementActionShapesPreset(defaultMovementActionShapesPreset, false);
            
            _playerMovementController = GetComponent<PlayerMovementController>();
            
            PlayerLook = GetComponent<PlayerLook>();
            PlayerLook.Initialize(this);
        }

        private void Start()
        {
            _playerMovementController.Initialize(this);
            
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Player");
            AssignControls();
            
            if (!setCameraFollowOnStart) return;
            CameraManager.Instance.SetFollow(PlayerLook.Root);
            CameraManager.Instance.SetLookAt(null);
        }

        private void AssignControls()
        {
            _moveInputAction ??= _inputActionMap.FindAction("Move");
            _lookInputAction ??= _inputActionMap.FindAction("Look");
            
            var jumpInputAction = _inputActionMap.FindAction("Jump");
            jumpInputAction.performed += OnJump;
            jumpInputAction.canceled += OnJump;
            
            var dashInputAction = _inputActionMap.FindAction("Dash");
            dashInputAction.performed += OnDash;
            dashInputAction.canceled += OnDash;
            
            var crouchInputAction = _inputActionMap.FindAction("Crouch");
            crouchInputAction.performed += OnCrouch;
            crouchInputAction.canceled += OnCrouch;
            
            var airInputAction = _inputActionMap.FindAction("Air");
            airInputAction.performed += OnAir;
            airInputAction.canceled += OnAir;
            
            var leftSpecialInputAction = _inputActionMap.FindAction("Left Special");
            leftSpecialInputAction.performed += OnLeftSpecial;
            leftSpecialInputAction.canceled += OnLeftSpecial;
            
            var rightSpecialInputAction = _inputActionMap.FindAction("Right Special");
            rightSpecialInputAction.performed += OnRightSpecial;
            rightSpecialInputAction.canceled += OnRightSpecial;
            
            var interactInputAction = _inputActionMap.FindAction("Interact");
            interactInputAction.performed += OnInteract;
            
            var openWheelInputAction = _inputActionMap.FindAction("Open Wheel");
            openWheelInputAction.performed += OnOpenWheel;
            
            var switchShapesInputAction = _inputActionMap.FindAction("Switch Shapes");
            switchShapesInputAction.performed += OnSwitchShapes;
            
            _inputActionMap.Enable();
        }
        
        private void UnassignControls()
        {
            var jumpInputAction = _inputActionMap.FindAction("Jump");
            jumpInputAction.performed -= OnJump;
            jumpInputAction.canceled -= OnJump;
            
            var dashInputAction = _inputActionMap.FindAction("Dash");
            dashInputAction.performed -= OnDash;
            dashInputAction.canceled -= OnDash;
            
            var crouchInputAction = _inputActionMap.FindAction("Crouch");
            crouchInputAction.performed -= OnCrouch;
            crouchInputAction.canceled -= OnCrouch;
            
            var airInputAction = _inputActionMap.FindAction("Air");
            airInputAction.performed -= OnAir;
            airInputAction.canceled -= OnAir;
            
            var leftSpecialInputAction = _inputActionMap.FindAction("Left Special");
            leftSpecialInputAction.performed -= OnLeftSpecial;
            leftSpecialInputAction.canceled -= OnLeftSpecial;
            
            var rightSpecialInputAction = _inputActionMap.FindAction("Right Special");
            rightSpecialInputAction.performed -= OnRightSpecial;
            rightSpecialInputAction.canceled -= OnRightSpecial;
            
            var interactInputAction = _inputActionMap.FindAction("Interact");
            interactInputAction.performed -= OnInteract;
            
            var openWheelInputAction = _inputActionMap.FindAction("Open Wheel");
            openWheelInputAction.performed -= OnOpenWheel;
            
            _inputActionMap.Disable();
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
            _playerMovementController.Move(Quaternion.Euler(0, PlayerLook.XRotation, 0));
        }

        private void LateUpdate()
        {
            PlayerLook.Look();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            _interactable = interactable;
            _interactable.View(InteractContext.Construct(gameObject), true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            interactable.View(InteractContext.Construct(gameObject), false);
            if (_interactable != interactable) return;
            _interactable = null;
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) _playerMovementController.StartJumping();
            else if (context.canceled) _playerMovementController.StopJumping();
        }

        private void OnDash(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) _playerMovementController.StartDashing();
            else if (context.canceled) _playerMovementController.StopDashing();
        }

        private void OnCrouch(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) _playerMovementController.StartCrouching();
            else if (context.canceled) _playerMovementController.StopCrouching();
        }

        private void OnAir(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) _playerMovementController.StartAir();
            else if (context.canceled) _playerMovementController.StopAir();
        }

        private void OnLeftSpecial(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) _playerMovementController.StartLeftSpecial();
            else if (context.canceled) _playerMovementController.StopLeftSpecial();
        }

        private void OnRightSpecial(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) _playerMovementController.StartRightSpecial();
            else if (context.canceled) _playerMovementController.StopRightSpecial();
        }
        
        private void OnInteract(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            _interactable?.Interact(InteractContext.Construct(gameObject));
        }

        private static void OnOpenWheel(InputAction.CallbackContext context)
        {
            UIManager.Instance.ActivateActionSelectionWheel();
        }
        
        private void OnSwitchShapes(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            var indexMap = new Dictionary<Vector2, int>()
            {
                { Vector2.left, 0 },
                { Vector2.up, 1 },
                { Vector2.right, 2 },
                { Vector2.down, 3 },
            };

            var index = indexMap[input];
            if (index >= movementActionShapesPresets.Length) return;
            SetMovementActionShapesPreset(movementActionShapesPresets[index]);
        }
    }
}
