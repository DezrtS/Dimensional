using System;
using System.Collections.Generic;
using Managers;
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
        
        public int Collectables { get; private set; }
        public int Keys { get; private set; }

        private void OnEnable()
        {
            SaveManager.Saving += SaveManagerOnSaving;
            SaveManager.Loaded += SaveManagerOnLoaded;
        }

        private void OnDisable()
        {
            SaveManager.Saving -= SaveManagerOnSaving;
            SaveManager.Loaded -= SaveManagerOnLoaded;
        }

        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Collectable)) return;
            saveData.collectableData.collectables = Collectables;
        }
        
        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Collectable)) return;
            AddCollectables(saveData.collectableData.collectables);
        }

        public void AddCollectables(int amount)
        {
            if (Collectables >= collectableCapacity) return;
            var newCollectables = Mathf.Min(Collectables + amount, collectableCapacity);
            CollectablesChanged?.Invoke(Collectables, newCollectables);
            Collectables = newCollectables;
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