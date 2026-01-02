using System;
using Managers;
using Systems.Player;
using TMPro;
using UnityEngine;

namespace User_Interface.Player
{
    public class CollectableCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI counterText;
        
        private Inventory _inventory;

        private void Awake()
        {
            counterText.text = "0";
        }

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Initializing) return;
            if (!PlayerController.Instance.TryGetComponent(out _inventory)) return;
            _inventory.CollectablesChanged += InventoryOnCollectablesChanged;
        }

        private void InventoryOnCollectablesChanged(int previousValue, int newValue)
        {
            counterText.text = newValue.ToString();
        }
    }
}
