using System;
using UnityEngine;

namespace Managers
{
    public enum WorldDimensions
    {
        None,
        One,
        Two,
        Three,
    }
    
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private WorldDimensions defaultWorldDimensions;
        
        public WorldDimensions WorldDimensions { get; private set; }

        private void Awake()
        {
            SetWorldDimensions(defaultWorldDimensions);
        }

        public void SetWorldDimensions(WorldDimensions worldDimensions)
        {
            if (worldDimensions == defaultWorldDimensions) return;
            WorldDimensions = worldDimensions;
        }
    }
}
