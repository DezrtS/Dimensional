using System;
using System.Collections;
using Febucci.UI;
using UnityEngine;

namespace User_Interface
{
    public class TutorialText : MonoBehaviour
    {
        private static readonly int IsShownHash = Animator.StringToHash("IsShown");
        
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TypewriterByCharacter typewriterByCharacter;
        
        private bool _isShown;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void ShowText(string text, float duration, bool hasDuration = true)
        {
            if (_isShown) return;
            _isShown = true;
            if (_animator) _animator.SetBool(IsShownHash, _isShown);
            textAnimator.SetText(text, true);
            typewriterByCharacter.StartShowingText();
            if (hasDuration) StartCoroutine(HideAreaRoutine(duration));
        }

        public void HideText()
        {
            if (!_isShown) return;
            _isShown = false;
            if (_animator) _animator.SetBool(IsShownHash, _isShown);
            typewriterByCharacter.StartDisappearingText();
        }

        private IEnumerator HideAreaRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            HideText();
        }
    }
}
