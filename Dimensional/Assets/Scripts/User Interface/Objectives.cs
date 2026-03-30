using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Scriptables.Objectives;
using Scriptables.Save;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace User_Interface
{
    public class Objectives : MonoBehaviour
    {
        public delegate void LocateObjectiveHandler(ObjectiveDatum objectiveDatum);
        public static event LocateObjectiveHandler ObjectiveLocated;
        
        private static readonly int IsRevealedHash = Animator.StringToHash("IsRevealed");
        private static readonly int IsLocatingHash = Animator.StringToHash("IsLocating");

        [SerializeField] private StringListVariable unlockedObjectives;
        [SerializeField] private StringListVariable completedObjectives;
        [SerializeField] private ObjectiveDatum[] objectiveData;
        [Space]
        [SerializeField] private bool debugUnlockAll;
        [SerializeField] private float timeToLocate;
        [Space]
        [SerializeField] private TextMeshProUGUI objectiveType;
        [SerializeField] private TextMeshProUGUI objectiveTitle;
        [SerializeField] private TextMeshProUGUI objectiveInfo;

        private InputActionMap _inputActionMap;
        private Animator _animator;

        private bool _isRevealed;
        private bool _canSwitch = true;
        private bool _isLocating;

        private Dictionary<string, ObjectiveDatum> _objectiveDictionary;
        private List<ObjectiveDatum> _objectives;
        
        private int _currentObjectiveIndex;
        private float _locateTimer;
        
        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            unlockedObjectives.ValueChanged += UnlockedObjectivesOnValueChanged;
            completedObjectives.ValueChanged += CompletedObjectivesOnValueChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            unlockedObjectives.ValueChanged -= UnlockedObjectivesOnValueChanged;
            completedObjectives.ValueChanged -= CompletedObjectivesOnValueChanged;
            UnassignControls();
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Preparing) return;
            DefineObjectives();
        }
        
        private void UnlockedObjectivesOnValueChanged()
        {
            DefineObjectives();
        }
        
        private void CompletedObjectivesOnValueChanged()
        {
            DefineObjectives();
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _objectiveDictionary = new Dictionary<string, ObjectiveDatum>();
            foreach (var objective in objectiveData) _objectiveDictionary.Add(objective.ObjectiveId, objective);
        }
        
        private void Start()
        {
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Objectives");
            AssignControls();
        }
        
        private void FixedUpdate()
        {
            if (!_isLocating) return;

            _locateTimer -= Time.fixedDeltaTime;
            if (_locateTimer <= 0f)
            {
                Locate();
            }
        }

        private void StartLocating()
        {
            if (_isLocating) return;
            _isLocating = true;
            _locateTimer = timeToLocate;
            _animator.SetBool(IsLocatingHash, _isLocating);
        }

        private void StopLocating()
        {
            if (!_isLocating) return;
            _isLocating = false;
            _locateTimer = 0;
            _animator.SetBool(IsLocatingHash, _isLocating);
        }

        public void Locate()
        {
            StopLocating();
            var objectiveDatum = _objectives[_currentObjectiveIndex];
            ObjectiveLocated?.Invoke(objectiveDatum);
        }
        
        public void EnableCanSwitch() => _canSwitch = true;
        
        public void DisableCanSwitch() => _canSwitch = false;

        private void DefineObjectives()
        {
            _objectives = new List<ObjectiveDatum>();
            if (debugUnlockAll)
            {
                foreach (var objective in objectiveData)
                {
                    _objectives.Add(objective);
                }

                return;
            }
            
            foreach (var objectiveId in unlockedObjectives.Value.list.Where(objectiveId => !completedObjectives.Value.list.Contains(objectiveId)))
            {
                _objectives.Add(_objectiveDictionary[objectiveId]);
            }
        }

        private void UpdateUI()
        {
            if (_objectives.Count <= 0)
            {
                objectiveType.text = "No Objectives";
                objectiveTitle.text = string.Empty;
                objectiveInfo.text = string.Empty;
                return;
            }
            
            var objectiveDatum = _objectives[_currentObjectiveIndex];
            objectiveType.text = "Active Objective";
            objectiveTitle.text = objectiveDatum.ObjectiveName;
            objectiveInfo.text = objectiveDatum.ObjectiveDescription;
        }

        private void SwitchObjectives(int direction)
        {
            var objectiveCount = _objectives.Count;
            _currentObjectiveIndex = (_currentObjectiveIndex + direction + objectiveCount) % objectiveCount;
            _animator.SetTrigger(direction > 0 ? "Right" : "Left");
        }

        private void OnOpen(InputAction.CallbackContext context)
        {
            if (!_canSwitch || !context.performed) return;
            _isRevealed = !_isRevealed;
            _animator.SetBool(IsRevealedHash, _isRevealed);
        }

        private void OnSwitch(InputAction.CallbackContext context)
        {
            if (!_isRevealed || !_canSwitch) return;
            
            var input = (int)context.ReadValue<float>();
            SwitchObjectives(input);
        }

        private void OnLocate(InputAction.CallbackContext context)
        {
            if (!_isRevealed || !_canSwitch) return;

            if (context.performed) StartLocating();
            else if (context.canceled) StopLocating();
        }
        
        private void AssignControls()
        {
            var openInputAction = _inputActionMap.FindAction("Open");
            openInputAction.performed += OnOpen;
            
            var switchInputAction = _inputActionMap.FindAction("Switch");
            switchInputAction.performed += OnSwitch;
            
            var locateInputAction = _inputActionMap.FindAction("Locate");
            locateInputAction.performed += OnLocate;
            locateInputAction.canceled += OnLocate;
        }

        private void UnassignControls()
        {
            var openInputAction = _inputActionMap.FindAction("Open");
            openInputAction.performed -= OnOpen;
            
            var switchInputAction = _inputActionMap.FindAction("Switch");
            switchInputAction.performed -= OnSwitch;
            
            var locateInputAction = _inputActionMap.FindAction("Locate");
            locateInputAction.performed -= OnLocate;
            locateInputAction.canceled -= OnLocate;
        }
    }
}
