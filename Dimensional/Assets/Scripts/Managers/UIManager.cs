using System;
using Debugging;
using Scriptables.Selection_Wheels;
using Scriptables.User_Interface;
using Systems.Actions.Movement;
using UnityEngine;
using User_Interface;
using User_Interface.Selection_Wheels;
using User_Interface.Visual_Effects;
using Utilities;
using Action = System.Action;

namespace Managers
{
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
        [SerializeField] private Transform interactableIconTransform;
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
