using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Managers
{
    public enum GameState
    {
        None,
        Initializing, 
        Loading,
        Preparing,
        Starting,
        Playing,
    }

    [Serializable]
    public class GameStateEvents
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private EventDatum[] eventData;
        
        public GameState GameState => gameState;
        public EventDatum[] EventData => eventData;
    }
    
    public enum Dimensions
    {
        Two,
        Three,
    }
    
    public enum CheckType
    {
        Ray,
        Sphere,
        Box,
    }
    
    public class GameManager : Singleton<GameManager>
    {
        public delegate void GameStateChangedHandler(GameState oldValue, GameState newValue);
        public static event GameStateChangedHandler GameStateChanged;
        public delegate void DimensionsChangedEventHandler(Dimensions oldValue, Dimensions newValue);
        public static event DimensionsChangedEventHandler WorldDimensionsChanged;
        
        [SerializeField] private Dimensions defaultWorldDimensions;
        [SerializeField] private InputActionAsset defaultInputActionAsset;
        [Space]
        [SerializeField] private GameStateEvents[] gameStateEvents;

        [Space] [SerializeField] private GameObject saveManagerPrefab;
        [SerializeField] private bool loadSceneData;
        [SerializeField] private List<DataType> loadOnLoading;

        private bool _isFirstLoad;
        
        public GameState GameState { get; private set; }
        public Dimensions WorldDimensions { get; private set; }
        public InputActionAsset InputActionAsset => defaultInputActionAsset;

        private void Start()
        {
            SetWorldDimensions(defaultWorldDimensions);
            ChangeGameState(GameState.Initializing);
            //StartCoroutine(GameStateRoutine());
        }

        private IEnumerator GameStateRoutine()
        {
            ChangeGameState(GameState.Initializing);
            yield return null;
            var isFirstLoad = !SaveManager.Instance;
            if (isFirstLoad) Instantiate(saveManagerPrefab);
            ChangeGameState(GameState.Loading);
            yield return null;
            if (isFirstLoad && loadSceneData) SaveManager.Instance.RequestLoad(new List<DataType>() { DataType.Scene, DataType.World });
            SaveManager.Instance.RequestLoad(loadOnLoading); 
            ChangeGameState(GameState.Preparing);
            yield return null;
            ChangeGameState(GameState.Starting);
            UIManager.Instance.Transition(false, false, 2f);
            yield return new WaitForSeconds(2f);
            ChangeGameState(GameState.Playing);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SceneManager.ReloadScene();
            }
            /*
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetWorldDimensions(Dimensions.Two);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetWorldDimensions(Dimensions.Three);
            }
            */

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

        public void ChangeGameState(GameState newState)
        {
            GameStateChanged?.Invoke(GameState, newState);
            GameState = newState;

            switch (newState)
            {
                case GameState.None:
                    break;
                case GameState.Initializing:
                    _isFirstLoad = !SaveManager.Instance;
                    if (_isFirstLoad) Instantiate(saveManagerPrefab);
                    break;
                case GameState.Loading:
                    if (_isFirstLoad && loadSceneData) SaveManager.Instance.RequestLoad(new List<DataType>() { DataType.Scene, DataType.World });
                    SaveManager.Instance.RequestLoad(loadOnLoading); 
                    break;
                case GameState.Preparing:
                    break;
                case GameState.Starting:
                    break;
                case GameState.Playing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
            
            foreach (var gameStateEvent in gameStateEvents)
            {
                if (gameStateEvent.GameState == newState)
                {
                    EventManager.SendEvents(gameStateEvent.EventData);
                }
            }
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
            InputActionAsset.FindActionMap("Player").Disable();
            InputActionAsset.FindActionMap("Selection Wheel").Disable();
            InputActionAsset.FindActionMap("UI").Disable();
            InputActionAsset.FindActionMap("Dialogue").Disable();
            InputActionAsset.FindActionMap(newInputActionMap).Enable();
        }

        public void TriggerHitStop(float duration)
        {
            StartCoroutine(HitStopRoutine(duration));
        }

        private IEnumerator HitStopRoutine(float duration)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1;
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
