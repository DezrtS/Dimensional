using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.Cutscenes;
using Scriptables.Quests;
using Systems.Cutscenes;
using Systems.Objectives;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utilities;

namespace Managers
{
    public class QuestManager : Singleton<QuestManager>
    {
        public static event Action<bool> ObjectivesRevealedStateChanged;
        
        [SerializeField] private QuestDatum[] questData;
        
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;
        
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private Material locationMaterial;
        [SerializeField] private float materialChangeDuration;
        
        private ObjectiveLocation _objectiveLocation;
        private InputActionMap _inputActionMap;

        private bool _isToggleDisabled;

        public bool IsHidden { get; private set; }

        protected override void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            base.OnEnable();
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            UnassignControls();
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Initializing) return;
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Objectives");
            AssignControls();
        }

        private void Awake()
        {
            foreach (var questDatum in questData)
            {
                questDatum.AttachQuest(gameObject);
            }
        }

        public void SetObjectivesHidden(bool isHidden)
        {
            _isToggleDisabled = isHidden;
            if (IsHidden) return;
            ObjectivesRevealedStateChanged?.Invoke(!isHidden);
        }

        private void OnToggle(InputAction.CallbackContext context)
        {
            if (_isToggleDisabled) return;
            Toggle();
        }

        private void Toggle()
        {
            IsHidden = !IsHidden;
            ObjectivesRevealedStateChanged?.Invoke(!IsHidden);
        }

        private void AssignControls()
        {
            var toggleInputAction = _inputActionMap.FindAction("Toggle");
            toggleInputAction.performed += OnToggle;
            _inputActionMap.Enable();
        }

        private void UnassignControls()
        {
            var toggleInputAction = _inputActionMap.FindAction("Toggle");
            toggleInputAction.performed -= OnToggle;
            _inputActionMap.Disable();
        }
    }
}