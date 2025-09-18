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

        public void AddCollectables(int amount)
        {
            if (Collectables >= collectableCapacity) return;
            var newCollectables = Mathf.Min(Collectables + amount, collectableCapacity);
            CollectablesChanged?.Invoke(Collectables, newCollectables);
            Collectables = newCollectables;
        }

        public void AddKeys(int amount)
        {
            var newKeys = Keys + amount;
            KeysChanged?.Invoke(Keys, newKeys);
            Keys = newKeys;
        }
    }
}