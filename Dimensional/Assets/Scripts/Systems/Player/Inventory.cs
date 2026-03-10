using System;
using System.Collections.Generic;
using Managers;
using Scriptables.Save;
using Systems.Events;
using Systems.Events.Busses;
using UnityEngine;

namespace Systems.Player
{
    public class Inventory : MonoBehaviour
    {
        public delegate void InventoryEventHandler(int previousValue, int newValue);
        public event InventoryEventHandler CollectablesChanged;
        public event InventoryEventHandler KeysChanged;
        
        [SerializeField] private int collectableCapacity;
        [SerializeField] private IntVariable collectablesSaveData;
        
        public int Collectables { get; private set; }
        public int Keys { get; private set; }

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
            if (newValue == GameState.Preparing)
            {
                AddCollectables(collectablesSaveData.Value);
            }
        }

        public void AddCollectables(int amount)
        {
            if (Collectables >= collectableCapacity) return;
            var newCollectables = Mathf.Min(Collectables + amount, collectableCapacity);
            CollectablesChanged?.Invoke(Collectables, newCollectables);
            Collectables = newCollectables;
            collectablesSaveData.Value = newCollectables;
            EventManager.SendEvent(new CounterEvent { CounterId = "Collectables", CounterValue = Collectables });
        }

        public void AddKeys(int amount)
        {
            var newKeys = Keys + amount;
            KeysChanged?.Invoke(Keys, newKeys);
            Keys = newKeys;
        }
    }
}