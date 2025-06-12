using System;
using System.Collections;
using Scriptables.Dialogue;
using TMPro;
using UnityEngine;

namespace Systems.Dialogue
{
    public class DialogueHandler : MonoBehaviour
    {
        public delegate void DialogueSequenceEventHandler(DialogueSequenceDatum dialogueSequence);
        public event DialogueSequenceEventHandler SequenceStarted;
        public event DialogueSequenceEventHandler SequenceFinished;

        [SerializeField] private bool startDialogueOnAwake;
        [SerializeField] private DialogueSequenceDatum defaultDialogueSequence;

        [SerializeField] private TextMeshProUGUI dialogueText;
        
        private TypewriterEffect _typewriter;

        private bool _isDialogueActive;
        private int _dialogueLineIndex;

        private bool _isAnimating;
        private float _animationMultiplier;
        
        public DialogueSequenceDatum DialogueSequenceDatum { get; private set; }

        private void Awake()
        {
            _typewriter = new TypewriterEffect(dialogueText, 5, 5);
            
            if (!startDialogueOnAwake) return;
            StartDialogueSequence(defaultDialogueSequence);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) AdvanceDialogueSequence();
        }

        public void StartDialogueSequence(DialogueSequenceDatum dialogueSequenceDatum)
        {
            DialogueSequenceDatum = dialogueSequenceDatum;
            _isDialogueActive = true;
            _dialogueLineIndex = 0;
            _animationMultiplier = 1;
            DisplayDialogueLine(DialogueSequenceDatum);
        }

        public void AdvanceDialogueSequence()
        {
            if (!_isDialogueActive)
            {
                StartDialogueSequence(defaultDialogueSequence);
                return;
            }

            if (_isAnimating)
            {
                SkipDialogueLine();
                return;
            }
            
            _dialogueLineIndex++;
            if (DialogueSequenceDatum.DialogueLineData.Length <= _dialogueLineIndex)
            {
                StopDialogueSequence();
            }
            else
            {
                DisplayDialogueLine(DialogueSequenceDatum);
            }
        }

        public void StopDialogueSequence()
        {
            DialogueSequenceDatum = null;
            _isDialogueActive = false;
            _dialogueLineIndex = 0;
            dialogueText.text = string.Empty;
        }
        
        private void DisplayDialogueLine(DialogueSequenceDatum dialogueSequence) => DisplayDialogueLine(dialogueSequence.DialogueLineData[_dialogueLineIndex]);

        private void DisplayDialogueLine(DialogueLineDatum dialogueLineDatum)
        {
            //_typewriter.StartTyping(dialogueLineDatum.Text);
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.text = dialogueLineDatum.Text;
            _animationMultiplier = 1;
            StartCoroutine(AnimateText(dialogueText));
        }

        private void SkipDialogueLine()
        {
            _animationMultiplier = 5;
        }

        private IEnumerator AnimateText(TextMeshProUGUI textMeshProUGUI)
        {
            _isAnimating = true;
            var totalLength = textMeshProUGUI.text.Length;
            for (var i = 0; i < totalLength; i++)
            {
                textMeshProUGUI.maxVisibleCharacters = i;
                yield return new WaitForSeconds(0.1f / _animationMultiplier);
            }
            _isAnimating = false;
        }
    }
}
