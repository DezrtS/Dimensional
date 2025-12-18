using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Scriptables.Actions.Movement;
using Scriptables.Entities;
using Scriptables.Player;
using Scriptables.Shapes;
using Systems.Actions.Movement;
using Systems.Entities;
using Systems.Entities.Behaviours;
using Systems.Interactables;
using Systems.Movement;
using Systems.Visual_Effects;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Systems.Player
{
    public class PlayerController : Singleton<PlayerController>, IEntity, IMove, IAim
    {
        public event Action SwitchedWheel;
        
        [SerializeField] private bool setCameraFollowOnStart;
        [SerializeField] private bool setCameraLookOnStart;
        [Space]
        [SerializeField] private EntityDatum entityDatum;
        [SerializeField] private ShapeDatum[] shapeData;
        [SerializeField] private MovementActionShapesPreset defaultMovementActionShapesPreset;
        [SerializeField] private MovementActionShapesPreset[] movementActionShapesPresets;
        [Space] 
        [SerializeField] private ResetPlayerDatum deathResetPlayerDatum;
        [Space]
        [SerializeField] private MeshRenderer[] eyeMeshRenderers;
        [SerializeField] private Material defaultEyeMaterial;
        [SerializeField] private Material dizzyEyeMaterial;
        [SerializeField] private EffectPlayer dizzyStars;

        private Health _health;
        private StunBehaviourComponent _stunBehaviour;

        private InputActionMap _inputActionMap;
        private InputAction _moveInputAction;
        private InputAction _lookInputAction;
        
        private readonly List<Interactable> _interactables = new List<Interactable>();
        private Interactable _interactable;

        private bool _isResetting;

        public EntityDatum EntityDatum => entityDatum;
        public GameObject GameObject => gameObject;
        public uint Id { get; private set; }
        public bool DebugDisable { get; set; }
        public PlayerLook PlayerLook { get; private set; }
        public PlayerMovementController PlayerMovementController { get; private set; }

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

        public void ResetMovementActions() => PlayerMovementController.ResetMovementActions();

        public void SetMovementActionShapesPreset(MovementActionShapesPreset movementActionShapesPreset, bool resetMovementActions = true)
        {
            MovementActionShapes = new List<MovementActionShape>();
            foreach (var movementActionShape in movementActionShapesPreset.MovementActionShapes)
            {
                MovementActionShapes.Add(movementActionShape);
            }
            
            if (resetMovementActions) ResetMovementActions();
        }

        protected override void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            SaveManager.Saving += SaveManagerOnSaving;
            SaveManager.Loaded += SaveManagerOnLoaded;
            base.OnEnable();
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
            
            PlayerMovementController = GetComponent<PlayerMovementController>();
            _health = GetComponent<Health>();
            _health.HealthStateChanged += HealthOnHealthStateChanged;
            _stunBehaviour = GetComponent<StunBehaviourComponent>();
            _stunBehaviour.Stunned += StunBehaviourOnStunned;
            _stunBehaviour.Landed += StunBehaviourOnLanded;
            _stunBehaviour.Recovered += StunBehaviourOnRecovered;
            
            PlayerLook = GetComponent<PlayerLook>();
            PlayerLook.Initialize(this);
        }

        private void Start()
        {
            PlayerMovementController.Initialize(this);
            
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Player");
            AssignControls();
            
            if (setCameraFollowOnStart) CameraManager.Instance.SetFollow(PlayerLook.Root);
            if (setCameraLookOnStart) CameraManager.Instance.SetLookAt(transform);
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
            
            _inputActionMap.Disable();
        }

        private void OnDisable()
        {
            UnassignControls();
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            SaveManager.Saving -= SaveManagerOnSaving;
            SaveManager.Loaded -= SaveManagerOnLoaded;
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
            PlayerMovementController.Move(Quaternion.Euler(0, PlayerLook.XRotation, 0));
            if (_interactables.Count > 0) CheckInteractables();
        }

        private void LateUpdate()
        {
            PlayerLook.Look();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Interactable interactable)) return;
            _interactables.Add(interactable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out Interactable interactable)) return;
            _interactables.Remove(interactable);
            interactable.UnHover(InteractContext.Construct(gameObject));
            if (_interactable == interactable) _interactable = null;
        }

        private void CheckInteractables()
        {
            var interactContext = InteractContext.Construct(gameObject);
            if (_interactables.Count == 1)
            {
                _interactable = _interactables[0];
                _interactable.Hover(interactContext);
                return;
            }
            
            var closestAngleDifference = _interactable ? Vector3.Angle(PlayerLook.Root.forward, _interactable.transform.position - transform.position) : float.MaxValue;
            var closestInteractable = _interactable;
            foreach (var interactable in _interactables)
            {
                var angleDifference = Vector3.Angle(PlayerLook.Root.forward, interactable.transform.position - transform.position);
                if (angleDifference > closestAngleDifference) continue;
                closestAngleDifference = angleDifference;
                closestInteractable = interactable;
            }
            
            if (_interactable == closestInteractable) return;
            if (_interactable) _interactable.UnHover(interactContext);
            _interactable = closestInteractable;
            _interactable.Hover(interactContext);
        }

        public void SetDizzyEyes(bool dizzyEyes)
        {
            foreach (var eyeMeshRenderer in eyeMeshRenderers)
            {
                eyeMeshRenderer.material = dizzyEyes ? dizzyEyeMaterial : defaultEyeMaterial;
            }
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Preparing) return;
            
            var checkpoint = CheckpointManager.Instance.GetLastCheckpoint();
            if (!checkpoint) return;
            PlayerMovementController.ForceController.Teleport(checkpoint.SpawnPosition);
        }
        
        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Player)) return;
            saveData.playerData.health = _health.CurrentHealth;
        }
        
        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Player)) return;
            _health.SetHealth(saveData.playerData.health);
        }
        
        private void HealthOnHealthStateChanged(Health health, bool isDead)
        {
            if (!isDead) return;
            _health.Revive();
            ResetPlayer(deathResetPlayerDatum, Vector3.zero);
        }
        
        private void StunBehaviourOnStunned()
        {
            SetDizzyEyes(true);
            PlayerMovementController.CancelAllActions();
            PlayerMovementController.IsDisabled = true;
            DebugDisable = true;
        }
        
        private void StunBehaviourOnLanded()
        {
            dizzyStars.Play(false);
        }
        
        private void StunBehaviourOnRecovered()
        {
            SetDizzyEyes(false);
            PlayerMovementController.IsDisabled = false;
            DebugDisable = false;
        }

        public void ResetPlayer(ResetPlayerDatum resetPlayerDatum, Vector3 defaultResetPosition)
        {
            if (_isResetting) return;
            StartCoroutine(ResetRoutine(resetPlayerDatum, defaultResetPosition));
        }

        private void RespawnPlayer(Vector3 defaultRespawnPosition)
        {
            var checkpoint = CheckpointManager.Instance.GetLastCheckpoint();
            var spawnPosition = checkpoint ? checkpoint.SpawnPosition : defaultRespawnPosition;
            PlayerMovementController.ForceController.Teleport(spawnPosition);
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) PlayerMovementController.StartJumping();
            else if (context.canceled) PlayerMovementController.StopJumping();
        }

        private void OnDash(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) PlayerMovementController.StartDashing();
            else if (context.canceled) PlayerMovementController.StopDashing();
        }

        private void OnCrouch(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) PlayerMovementController.StartCrouching();
            else if (context.canceled) PlayerMovementController.StopCrouching();
        }

        private void OnAir(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) PlayerMovementController.StartAir();
            else if (context.canceled) PlayerMovementController.StopAir();
        }

        private void OnLeftSpecial(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) PlayerMovementController.StartLeftSpecial();
            else if (context.canceled) PlayerMovementController.StopLeftSpecial();
        }

        private void OnRightSpecial(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (context.performed) PlayerMovementController.StartRightSpecial();
            else if (context.canceled) PlayerMovementController.StopRightSpecial();
        }
        
        private void OnInteract(InputAction.CallbackContext context)
        {
            if (DebugDisable) return;
            if (_interactable) _interactable.Interact(InteractContext.Construct(gameObject));
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

        private IEnumerator ResetRoutine(ResetPlayerDatum resetPlayerDatum, Vector3 defaultResetPosition)
        {
            _isResetting = true;
            
            var cameraManager = CameraManager.Instance;
            if (!resetPlayerDatum.FollowPlayer) cameraManager.SetFollow(null);
            if (resetPlayerDatum.LookAtPlayer) cameraManager.SetLookAt(transform);
            yield return new WaitForSeconds(resetPlayerDatum.ResetDelay);
            if (resetPlayerDatum.UseTransition)
            {
                UIManager.Instance.Transition(false, true, resetPlayerDatum.TransitionDuration);
                yield return new WaitForSeconds(resetPlayerDatum.TransitionDuration);
                UIManager.Instance.Transition(false, false);   
            }
            
            switch (resetPlayerDatum.ResetResponseType)
            {
                case ResetResponseType.None:
                    break;
                case ResetResponseType.Kill:
                    _health.Die();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            cameraManager.SetFollow(PlayerLook.Root);
            cameraManager.SetLookAt(null);
            RespawnPlayer(defaultResetPosition);
            _isResetting = false;
        }
    }
}
