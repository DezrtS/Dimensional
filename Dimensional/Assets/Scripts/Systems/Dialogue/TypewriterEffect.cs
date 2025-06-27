using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;

namespace Systems.Dialogue
{
    public class TypewriterEffect : MonoBehaviour
    {
        public delegate void TypewriterEventHandler();
        public event TypewriterEventHandler Finished;
        public delegate void CharacterEventHandler(char character, int index);
        public event CharacterEventHandler CharacterRevealed;
        
        [SerializeField] private float defaultCharactersPerSecond = 20;
        [SerializeField] private float defaultInterpunctuationDelay = 0.5f;

        private float _typingMultiplier = 1;
        
        private TMP_Text _textComponent;
        private bool _isTyping;
        private string _strippedText;
        private int _currentVisibleCharacterIndex;
        
        private Coroutine _coroutine;

        public void Initialize(TMP_Text textComponent)
        {
            _textComponent = textComponent;
        }

        public void SetMultiplier(float multiplier)
        {
            _typingMultiplier = multiplier;
        }

        public void StartTyping(string strippedText)
        {
            if (_isTyping) StopTyping();
            
            _isTyping = true;
            _strippedText = strippedText;
            _currentVisibleCharacterIndex = 0;
            _textComponent.maxVisibleCharacters = 0;
            _coroutine = StartCoroutine(Typewriter());
        }

        private void StopTyping()
        {
            if (!_isTyping) return;
            
            _isTyping = false;
            StopCoroutine(_coroutine);
        }

        public void Skip()
        {
            
        }

        private IEnumerator Typewriter()
        {
            foreach (var c in _strippedText)
            {
                _textComponent.maxVisibleCharacters = _currentVisibleCharacterIndex + 1;
                CharacterRevealed?.Invoke(c, _currentVisibleCharacterIndex - 1);

                if (char.IsPunctuation(c))
                {
                    yield return new WaitForSeconds(defaultInterpunctuationDelay);
                }
                else
                {
                    yield return new WaitForSeconds(1 / (defaultCharactersPerSecond * _typingMultiplier));
                }
                
                _currentVisibleCharacterIndex++;
            }
            
            _isTyping = false;
        }
    }
}
