using System;
using Scriptables.Interactables;
using Systems.Player;
using UnityEngine;
using User_Interface.Visual_Effects;
using Utilities;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        public static event Action TransitionFinished;

        [SerializeField] private GameObject controls;
        [SerializeField] private bool transitionOnAwake;

        [SerializeField] private Transform interactableIconTransform;
        
        private MaskReveal _maskReveal;

        private void Awake()
        {
            _maskReveal = GetComponent<MaskReveal>();
            _maskReveal.Finished += MaskRevealOnFinished;
            
            if (transitionOnAwake) Transition(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                controls.SetActive(!controls.activeSelf);
                if (controls.activeSelf)
                {
                    PlayerController.Instance.DebugDisable = true;
                    CameraManager.Instance.UnlockAndShowCursor();
                }
                else
                {
                    PlayerController.Instance.DebugDisable = false;
                    CameraManager.Instance.LockAndHideCursor();
                }
            }
        }

        public void SpawnInteractableIcon(InteractableIconDatum interactableIconDatum, Transform interactableTransform)
        {
            interactableIconDatum.Spawn(interactableIconTransform, interactableTransform);
        }

        public void Transition(bool reverse)
        {
            _maskReveal.Transition(true, reverse);
        }

        private static void MaskRevealOnFinished()
        {
            TransitionFinished?.Invoke();
        }
    }
}
