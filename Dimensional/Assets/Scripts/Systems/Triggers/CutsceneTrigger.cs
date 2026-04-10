using System;
using System.Collections.Generic;
using Managers;
using Scriptables.Cutscenes;
using Scriptables.Save;
using Systems.Cutscenes;
using UnityEngine;
using Utilities;

namespace Systems.Triggers
{
    public class CutsceneTrigger : MonoBehaviour
    {
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;

        [SerializeField] private bool hasBoolVariable;
        [SerializeField] private BoolVariableInstance boolVariableInstance;
        
        private bool _isTriggered;
        private bool _isCompleted;

        private void OnEnable()
        {
            if (hasBoolVariable) GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void OnDisable()
        {
            if (hasBoolVariable) GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Starting) return;
            if (!boolVariableInstance.IsEnabled()) return;
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
            _isTriggered = true;
        }

        private void Awake()
        {
            cutscene.Initialize(cutsceneDatum);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered || _isCompleted || !other.CompareTag("Player")) return;
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
            _isTriggered = true;
        }
    }
}
