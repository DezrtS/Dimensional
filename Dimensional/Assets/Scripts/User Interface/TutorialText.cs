using System.Collections;
using Febucci.UI;
using UnityEngine;

namespace User_Interface
{
    public class TutorialText : MonoBehaviour
    {
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TypewriterByCharacter typewriterByCharacter;
        
        private bool _isShown;

        public void ShowText(string text, float duration, bool hasDuration = true)
        {
            if (_isShown) return;
            _isShown = true;
            textAnimator.SetText(text, true);
            typewriterByCharacter.StartShowingText();
            if (hasDuration) StartCoroutine(HideAreaRoutine(duration));
        }

        public void HideText()
        {
            if (!_isShown) return;
            _isShown = false;
            typewriterByCharacter.StartDisappearingText();
        }

        private IEnumerator HideAreaRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            HideText();
        }
    }
}
