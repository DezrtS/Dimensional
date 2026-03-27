using System;
using System.Linq;
using Managers;
using Scriptables.Save;
using UnityEngine;

namespace Utilities
{
    public class ChangeOnBoolVariables : MonoBehaviour
    {
        public BoolVariableInstance[] boolVariableInstances;
        private bool _wasDisabled;

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            foreach (var boolVariableInstance in boolVariableInstances)
            {
                boolVariableInstance.boolVariable.ValueChanged += BoolVariableOnValueChanged;
            }

            if (!_wasDisabled) return;
            _wasDisabled = false;
            SetChildren(IsActive());
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            foreach (var boolVariableInstance in boolVariableInstances)
            {
                boolVariableInstance.boolVariable.ValueChanged -= BoolVariableOnValueChanged;
            }
            _wasDisabled = true;
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Preparing) return; 
            SetChildren(IsActive());
        }

        private void BoolVariableOnValueChanged()
        {
            SetChildren(IsActive());
        }

        private void SetChildren(bool isActive)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }

        private bool IsActive()
        {
            return boolVariableInstances.Length == 0 || boolVariableInstances.All(boolVariableInstance => boolVariableInstance.IsEnabled());
        }
    }
}
