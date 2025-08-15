using System;
using UnityEngine;
using User_Interface.VFXs;
using Utilities;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        public static event Action TransitionFinished;

        [SerializeField] private GameObject controls;
        [SerializeField] private bool transitionOnAwake;
        
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
            }
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
