using System;
using Debugging;
using Scriptables.Selection_Wheels;
using Scriptables.User_Interface;
using Systems.Actions.Movement;
using TMPro;
using UnityEngine;
using User_Interface;
using User_Interface.Selection_Wheels;
using User_Interface.Visual_Effects;
using Utilities;
using Action = System.Action;

namespace Managers
{
    public enum UserInterfaceType
    {
        None,
        Pause,
        Quests,
        TravelMap,
    }
    
    public class UIManager : Singleton<UIManager>
    {
        public static event Action TransitionFinished;

        [SerializeField] private GameObject controls;
        [Space]
        [SerializeField] private bool showAreaTitle = true;
        [SerializeField] private AreaTitle areaTitle;
        [SerializeField] private string areaName = "Sphero";
        [SerializeField] private float areaTitleDuration = 5;
        [Space]
        [SerializeField] private GameObject travelMap;
        [Space]
        [SerializeField] private Transform interactableIconTransform;
        [Space]
        [SerializeField] private TextMeshProUGUI tutorialText;
        [Space]
        [SerializeField] private GameObject fade;
        [SerializeField] private GameObject actionSelectionWheelTransform;
        [SerializeField] private GameObject shapeSelectionWheelTransform;
        [SerializeField] private SelectionWheelDatum actionSelectionWheelDatum;
        
        private MaskReveal _maskReveal;
        private SelectionWheel _actionSelectionWheel;
        private SelectionWheel _shapeSelectionWheel;
        
        private PlayerUIV1 _playerUI;
        
        public MovementActionType SelectedMovementActionType { get; set; }

        private void Awake()
        {
            _maskReveal = GetComponent<MaskReveal>();
            _maskReveal.Finished += MaskRevealOnFinished;
            
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void Start()
        {
            DeactivateUI(false);
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Playing) return;
            if (!showAreaTitle) return;
            areaTitle.ShowArea(areaName, areaTitleDuration);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                controls.SetActive(!controls.activeSelf);
            }
        }

        public WorldUIAnchor SpawnWorldUIAnchor(WorldUIAnchorDatum worldUIAnchorDatum, GameObject holderGameObject, Transform worldTransform)
        {
            return worldUIAnchorDatum.SpawnWorldUIAnchor(interactableIconTransform, holderGameObject, worldTransform);
        }

        public void Transition(bool invert, bool reverse, float duration = -1)
        {
            _maskReveal.Transition(invert, reverse, duration);
        }

        public void ActivateUI(UserInterfaceType userInterfaceType)
        {
            //CameraManager.Instance.UnlockAndShowCursor();
            GameManager.Instance.SwitchInputActionMaps("UI");
            switch (userInterfaceType)
            {
                case UserInterfaceType.None:
                    break;
                case UserInterfaceType.Pause:
                    break;
                case UserInterfaceType.Quests:
                    break;
                case UserInterfaceType.TravelMap:
                    travelMap.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(userInterfaceType), userInterfaceType, null);
            }
        }

        public void DeactivateUI(bool resetInput = true)
        {
            if (resetInput) GameManager.Instance.ResetInputActionMapToDefault();
            travelMap.SetActive(false);
        }

        public GameObject GetShapeSelectionWheelGameObject(bool useOtherMethod)
        {
            return useOtherMethod ? _playerUI.ShapeSelectionWheelTransform : shapeSelectionWheelTransform;
        }
        
        public void EnableFade() => fade.SetActive(true);
        public void DisableFade() => fade.SetActive(false);

        private static void MaskRevealOnFinished()
        {
            TransitionFinished?.Invoke();
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }
    }
}
