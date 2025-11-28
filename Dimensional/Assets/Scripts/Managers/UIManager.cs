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
        [SerializeField] private bool transitionOnAwake;
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
            
            if (transitionOnAwake) Transition(true, false);
            
            //_playerUI = GetComponent<PlayerUIV1>();
        }

        private void Start()
        {
            if (!showAreaTitle) return;
            areaTitle.ShowArea(areaName, areaTitleDuration);
            
            //if (!actionSelectionWheelDatum) return;
            //_actionSelectionWheel = actionSelectionWheelDatum.AttachSelectionWheel(actionSelectionWheelTransform);
            //_actionSelectionWheel.GenerateSelectionWheel();
            //_actionSelectionWheel.Cancelled += SelectionWheelOnCancelled;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                controls.SetActive(!controls.activeSelf);
            }
        }

        public WorldUIAnchor SpawnWorldUIAnchor(WorldUIAnchorDatum worldUIAnchorDatum, Transform worldTransform)
        {
            return worldUIAnchorDatum.SpawnWorldUIAnchor(interactableIconTransform, worldTransform);
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

        public void OpenActionShapeSelection()
        {
            return;
            _actionSelectionWheel.Show();
            _actionSelectionWheel.Activate();   
            
            GameManager.Instance.SwitchInputActionMaps("Selection Wheel");
            GameManager.SetTimeScale(0.25f);
            EnableFade();
        }

        public void CloseActionShapeSelection()
        {
            return;
            _playerUI.CloseActionShapeSelection();
            _actionSelectionWheel.Hide();
            _actionSelectionWheel.Deactivate();   
            
            GameManager.Instance.ResetInputActionMapToDefault();
            GameManager.SetTimeScale();
            DisableFade();
        }

        private void SelectionWheelOnCancelled(SelectionWheel selectionWheel)
        {
            CloseActionShapeSelection();
        }

        private static void MaskRevealOnFinished()
        {
            TransitionFinished?.Invoke();
        }
    }
}
