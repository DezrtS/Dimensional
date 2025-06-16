using System.Collections;
using Managers;
using TMPro;
using UnityEngine;

namespace Systems.Dialogue
{
    public class TypewriterEffect
    {
        public delegate void TypewriterEventHandler();

        public event TypewriterEventHandler Finished;
        
        private readonly TextMeshProUGUI _textComponent;
        private readonly float _charactersPerSecond;
        private readonly float _punctuationDelayMultiplier;
        
        private Coroutine _typewriterCoroutine;

        public TypewriterEffect(TextMeshProUGUI textComponent, float charactersPerSecond, float punctuationDelayMultiplier)
        {
            _textComponent = textComponent;
            _charactersPerSecond = charactersPerSecond;
            _punctuationDelayMultiplier = punctuationDelayMultiplier;
        }

        public void StartTyping(string text)
        {
            if (_typewriterCoroutine != null) 
                CoroutineManager.Instance.StopCoroutine(_typewriterCoroutine);
            
            _textComponent.text = text;
            _textComponent.ForceMeshUpdate(); // Critical for accurate character count
            
            _typewriterCoroutine =  CoroutineManager.Instance.StartCoroutine(TypeText());
        }

        private IEnumerator TypeText()
        {
            _textComponent.maxVisibleCharacters = 0;
            var totalCharacters = _textComponent.textInfo.characterCount;
            var visibleCount = 1;

            while (visibleCount < totalCharacters)
            {
                visibleCount++;
                _textComponent.maxVisibleCharacters = visibleCount;
                if (_textComponent.textInfo.characterInfo[visibleCount - 1].isVisible) yield return GetWaitTime(visibleCount - 1);
            }
            
            Finished?.Invoke();
        }

        private WaitForSeconds GetWaitTime(int charIndex)
        {
            var currentChar = _textComponent.text[charIndex];
            var delay = 1f / _charactersPerSecond;
            
            // Add extra pause for punctuation
            if (char.IsPunctuation(currentChar)) delay *= _punctuationDelayMultiplier;
            
            return new WaitForSeconds(delay);
        }

        public void Skip()
        {
            if (_typewriterCoroutine == null) return;
            CoroutineManager.Instance.StopCoroutine(_typewriterCoroutine);
            _textComponent.maxVisibleCharacters = _textComponent.textInfo.characterCount;
            
            Finished?.Invoke();
        }
    }
}
