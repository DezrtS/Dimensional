using System;
using UnityEngine;
using Utilities;

namespace Managers
{
    public delegate void DimensionsChangedEventHandler(Dimensions oldValue, Dimensions newValue);
    public enum Dimensions
    {
        Two,
        Three,
    }
    
    public class GameManager : Singleton<GameManager>
    {
        public static event DimensionsChangedEventHandler WorldDimensionsChanged;
        [SerializeField] private Dimensions defaultWorldDimensions;
        
        public Dimensions WorldDimensions { get; private set; }

        private void Awake()
        {
            SetWorldDimensions(defaultWorldDimensions);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetWorldDimensions(Dimensions.Two);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetWorldDimensions(Dimensions.Three);
            }
        }

        public void SetWorldDimensions(Dimensions worldDimensions)
        {
            if (worldDimensions == WorldDimensions) return;
            WorldDimensionsChanged?.Invoke(WorldDimensions, worldDimensions);
            WorldDimensions = worldDimensions;
        }
    }
}
