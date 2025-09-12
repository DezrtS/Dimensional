using UnityEngine;

namespace Systems.Player
{
    public class Inventory : MonoBehaviour
    {
        public delegate void CollectableEventHandler(int previousValue, int newValue);
        public event CollectableEventHandler CollectablesChanged;
        
        [SerializeField] private int capacity;
        public int Collectables { get; private set; }

        public void AddCollectables(int amount)
        {
            var newCollectables = Collectables + amount;
            CollectablesChanged?.Invoke(Collectables, newCollectables);
            Collectables = newCollectables;
        }
    }
}