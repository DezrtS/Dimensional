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
            if (newValue == GameState.Preparing) gameObject.SetActive(disableOnFalse == boolVariable.Value);
        }
        
        private void BoolVariableOnValueChanged()
        {
            gameObject.SetActive(disableOnFalse == boolVariable.Value);
        }
    }
}
