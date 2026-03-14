using System;
using Managers;
using Scriptables.Save;
using UnityEngine;

namespace Utilities
{
    public class DisableOnBool : MonoBehaviour
    {
        [SerializeField] private BoolVariable boolVariable;
        [SerializeField] private bool disableOnFalse;

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            boolVariable.ValueChanged += BoolVariableOnValueChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            boolVariable.ValueChanged -= BoolVariableOnValueChanged;
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Preparing) return; 
            SetChildren(disableOnFalse == boolVariable.Value);
        }
        
        private void BoolVariableOnValueChanged()
        {
            SetChildren(disableOnFalse == boolVariable.Value);
        }

        private void SetChildren(bool isActive)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }
    }
}
