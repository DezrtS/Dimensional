using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    
    public enum CheckType
    {
        Ray,
        Sphere,
        Box,
    }
    
    public class GameManager : Singleton<GameManager>
    {
        public static event DimensionsChangedEventHandler WorldDimensionsChanged;
        
        [SerializeField] private Dimensions defaultWorldDimensions;
        [SerializeField] private InputActionAsset defaultInputActionAsset;
        
        public Dimensions WorldDimensions { get; private set; }
        public InputActionAsset InputActionAsset => defaultInputActionAsset;

        private void Start()
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

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Time.timeScale = Mathf.Min(Time.timeScale + 0.1f, 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Time.timeScale = Mathf.Max(Time.timeScale - 0.1f, 0.1f);
            }
        }

        public static void SetTimeScale(float timeScale = 1)
        {
            var clampedTimeScale = Mathf.Clamp(timeScale, 0.1f, 1f);
            Time.timeScale = clampedTimeScale;
            Time.fixedDeltaTime = 0.02f * clampedTimeScale;
        }

        public void SetWorldDimensions(Dimensions worldDimensions)
        {
            if (worldDimensions == WorldDimensions) return;
            WorldDimensionsChanged?.Invoke(WorldDimensions, worldDimensions);
            WorldDimensions = worldDimensions;
        }

        public void ResetInputActionMapToDefault() => SwitchInputActionMaps("Player");

        public void SwitchInputActionMaps(string newInputActionMap)
        {
            InputActionAsset.Disable();
            InputActionAsset.FindActionMap(newInputActionMap).Enable();
        }

        public static float Derivative(AnimationCurve curve, float time)
        {
            const float delta = 0.001f;
            var currentHeight = curve.Evaluate(time);
            var nextHeight = curve.Evaluate(time + delta);
            var slope = (nextHeight - currentHeight) / delta;
        
            return slope;
        }

        public static bool CheckLayerMask(LayerMask layerMask, GameObject gameObject)
        {
            return ((layerMask.value & (1 << gameObject.layer)) != 0);
        }
        
        public static List<RaycastHit> CheckCast(Vector3 worldPosition, CheckType checkType, Vector3 offset, Vector3 direction, Vector3 size, float distance, LayerMask layerMask)
        {
            var position = worldPosition + offset;
            var hits = new List<RaycastHit>();
            var results = new RaycastHit[10];
            var count = 0;

            switch (checkType)
            {
                case CheckType.Ray:
                    if (Physics.Raycast(position, direction, out var hit, distance, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        hits.Add(hit);
                    }
                    break;
                case CheckType.Sphere:
                    count = Physics.SphereCastNonAlloc(position, size.magnitude, direction, results, distance, layerMask, QueryTriggerInteraction.Ignore);
                    break;
                case CheckType.Box:
                    count = Physics.BoxCastNonAlloc(position, size / 2f,  direction, results, Quaternion.identity, distance, layerMask, QueryTriggerInteraction.Ignore);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(checkType), checkType, null);
            }
            
            for (var i = 0; i < count; i++)
            {
                hits.Add(results[i]);
            }
            
            return hits;
        }
    }
}
