using System;
using System.Collections;
using Febucci.UI;
using UnityEngine;

namespace User_Interface
{
    public class AreaTitle : MonoBehaviour
    {
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TypewriterByCharacter typewriterByCharacter;
        
        private Animator _animator;
        private bool _isShown;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void ShowArea(string areaName, float duration)
        {
            if (_isShown) return;
            _isShown = true;
            textAnimator.SetText(areaName, true);
            typewriterByCharacter.StartShowingText();
            _animator.SetTrigger("Show");
            StartCoroutine(HideAreaRoutine(duration));
        }

        public void HideArea()
        {
            if (!_isShown) return;
            _isShown = false;
            typewriterByCharacter.StartDisappearingText();
            _animator.SetTrigger("Hide");
        }

        private IEnumerator HideAreaRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            HideArea();
        }
    }
}
