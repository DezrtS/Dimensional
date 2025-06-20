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

    public enum PowerLevel
    {
        Low,
        Medium,
        High,
    }

    public enum GroundedCheckType
    {
        Ray,
        Sphere,
        Box,
    }
    
    public class GameManager : Singleton<GameManager>
    {
        public static event DimensionsChangedEventHandler WorldDimensionsChanged;
        [SerializeField] private Dimensions defaultWorldDimensions;
        [SerializeField] private GameObject controls;
        
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

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                controls.SetActive(!controls.activeSelf);
            }
        }

        public void SetWorldDimensions(Dimensions worldDimensions)
        {
            if (worldDimensions == WorldDimensions) return;
            WorldDimensionsChanged?.Invoke(WorldDimensions, worldDimensions);
            WorldDimensions = worldDimensions;
        }

        public static float Derivative(AnimationCurve curve, float time)
        {
            const float delta = 0.001f;
            var currentHeight = curve.Evaluate(time);
            var nextHeight = curve.Evaluate(time + delta);
            var slope = (nextHeight - currentHeight) / delta;
        
            return slope;
        }
    }
}
