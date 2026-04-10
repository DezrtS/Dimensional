using System;
using Debugging;
using Scriptables.Selection_Wheels;
using Scriptables.User_Interface;
using Systems.Actions.Movement;
using Systems.Events;
using Systems.Events.Busses;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] private bool canPause = true;
        [SerializeField] private bool enableUIControls;
        [Space]
        [SerializeField] private bool showAreaTitle = true;
        [SerializeField] private AreaTitle areaTitle;
        [SerializeField] private string areaName = "Sphero";
        [SerializeField] private float areaTitleDuration = 5;
        [Space]
        [SerializeField] private GameObject travelMap;
        [SerializeField] private GameObject pauseMenu;
        [Space]
        [SerializeField] private Transform interactableIconTransform;
        [Space]
        [SerializeField] private TutorialText tutorialText;
        [SerializeField] private TutorialText objectivesText;
        [SerializeField] private TutorialText bossText;
        [Space]
        [SerializeField] private GameObject fade;
        [SerializeField] private GameObject actionSelectionWheelTransform;
        [SerializeField] private GameObject shapeSelectionWheelTransform;
        [SerializeField] private SelectionWheelDatum actionSelectionWheelDatum;
        
        private InputActionMap _inputActionMap;
        
        private MaskReveal _maskReveal;
        private SelectionWheel _actionSelectionWheel;
        private SelectionWheel _shapeSelectionWheel;
        
        private PlayerUIV1 _playerUI;
        private Animator _animator;
        private bool _isPaused;
        
        public MovementActionType SelectedMovementActionType { get; set; }
        public AreaTitle AreaTitle => areaTitle;

        private void Awake()
        {
            _maskReveal = GetComponent<MaskReveal>();
            _maskReveal.Finished += MaskRevealOnFinished;
            
            _animator = GetComponent<Animator>();
            
            UIEventBus.EventFired += UIEventBusOnEventFired;
            
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void UIEventBusOnEventFired(GameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case DisplayTextEvent displayTextEvent:
                    switch (displayTextEvent.DisplayType)
                    {
                        case DisplayType.Tutorial:
                            tutorialText.ShowText(displayTextEvent.Text, displayTextEvent.DisplayDuration, displayTextEvent.HasDisplayDuration);
                            break;
                        case DisplayType.Area:
                            areaTitle.ShowArea(displayTextEvent.Text, displayTextEvent.DisplayDuration, displayTextEvent.HasDisplayDuration);
                            break;
                        case DisplayType.Boss:
                            bossText.ShowText(displayTextEvent.Text, displayTextEvent.DisplayDuration, displayTextEvent.HasDisplayDuration);
                            break;
                        case DisplayType.Objective:
                            objectivesText.ShowText(displayTextEvent.Text, displayTextEvent.DisplayDuration, displayTextEvent.HasDisplayDuration);
                            break;
                    }
                    break;
                case HideTextEvent hideTextEvent:
                    switch (hideTextEvent.DisplayType)
                    {
                        case DisplayType.Tutorial:
                            tutorialText.HideText();
                            break;
                        case DisplayType.Area:
                            areaTitle.HideArea();
                            break;
                        case DisplayType.Boss:
                            bossText.HideText();
                            break;
                        case DisplayType.Objective:
                            objectivesText.HideText();
                            break;
                    }

                    break;
            }
        }

        private void Start()
        {
            DeactivateUI(false);
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.Initializing:
                    _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Pause");
                    if (canPause) AssignControls();
                    if (enableUIControls) GameManager.Instance.SwitchInputActionMaps("UI");
                    break;
                case GameState.Playing:
                    if (!showAreaTitle) return;
                    areaTitle.ShowArea(areaName, areaTitleDuration);
                    break;
            }
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
                    pauseMenu.SetActive(true);
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

        private void OnPause(InputAction.CallbackContext context)
        {
            Pause();
        }

        public void Pause()
        {
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                ActivateUI(UserInterfaceType.Pause);
                CameraManager.Instance.UnlockAndShowCursor();
                Time.timeScale = 0;
            }
            else
            {
                CameraManager.Instance.LockAndHideCursor();
                DeactivateUI();
                Time.timeScale = 1;
            }
        }

        public void ExitToMainMenu()
        {
            SceneManager.Instance.LoadSceneWithTransition("MainMenuTest");
        }

        public void DeactivateUI(bool resetInput = true)
        {
            if (resetInput) GameManager.Instance.ResetInputActionMapToDefault();
            travelMap.SetActive(false);
            pauseMenu.SetActive(false);
        }

        public void SetUIHidden(bool isHidden)
        {
            _animator.SetBool("Hide", isHidden);
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
            UIEventBus.EventFired -= UIEventBusOnEventFired;
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            UnassignControls();
        }
        
        private void AssignControls()
        {
            var pauseInputAction = _inputActionMap.FindAction("Pause");
            pauseInputAction.performed += OnPause;
            _inputActionMap.Enable();
        }

        private void UnassignControls()
        {
            var pauseInputAction = _inputActionMap.FindAction("Pause");
            pauseInputAction.performed -= OnPause;
            _inputActionMap.Disable();
        }
    }
}
