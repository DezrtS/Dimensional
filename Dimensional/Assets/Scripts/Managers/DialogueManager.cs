using System;
using Scriptables.Dialogue;
using Systems.Dialogue;
using TMPro;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public delegate void DialogueSequenceEventHandler(DialogueSequenceDatum dialogueSequence);
        public static event DialogueSequenceEventHandler SequenceStarted;
        public static event DialogueSequenceEventHandler SequenceInterrupted;
        public static event DialogueSequenceEventHandler SequenceFinished;
        
        [Header("Dialogue Settings")]
        [SerializeField] private bool startDialogueOnAwake;
        [SerializeField] private DialogueSequenceDatum currentDialogueSequence;
        [Space(10)]
        [SerializeField] private TMP_Text dialogueTextComponent;

        private int _currentDialogueLineIndex;
        private bool _isDialogueActive;
        
        private DialogueEffectHandler _dialogueEffectHandler;
        private TypewriterEffect _typewriterEffect;
        
        private void Awake()
        {
            _dialogueEffectHandler = new DialogueEffectHandler();
            
            _typewriterEffect = GetComponent<TypewriterEffect>();
            _typewriterEffect.Initialize(dialogueTextComponent);
            _typewriterEffect.CharacterRevealed += TypewriterEffectOnCharacterRevealed;

            if (!startDialogueOnAwake) return;
            StartDialogueSequence(currentDialogueSequence);
        }

        private void TypewriterEffectOnCharacterRevealed(char character, int index)
        {
            _dialogueEffectHandler.OnCharacterRevealed(character, index);
        }

        public void ChangeTypewriterSpeed(float speed)
        {
            _typewriterEffect.SetMultiplier(speed);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) AdvanceDialogueSequence();
        }

        public void StartDialogueSequence(DialogueSequenceDatum dialogueSequence)
        {
            currentDialogueSequence = dialogueSequence;
            _isDialogueActive = true;
            _currentDialogueLineIndex = 0;
            DisplayDialogueLine(dialogueSequence);
        }

        public void AdvanceDialogueSequence()
        {
            if (!_isDialogueActive)
            {
                if (currentDialogueSequence) StartDialogueSequence(currentDialogueSequence);
                return;
            }
            
            _currentDialogueLineIndex++;
            if (currentDialogueSequence.DialogueLineData.Length <= _currentDialogueLineIndex)
            {
                StopDialogueSequence();
            }
            else
            {
                DisplayDialogueLine(currentDialogueSequence);
            }
        }

        public void StopDialogueSequence()
        {
            currentDialogueSequence = null;
            _isDialogueActive = false;
            _currentDialogueLineIndex = 0;
            dialogueTextComponent.text = string.Empty;
        }

        private void DisplayDialogueLine(DialogueSequenceDatum dialogueSequence) => DisplayDialogueLine(dialogueSequence.DialogueLineData[_currentDialogueLineIndex]);

        private void DisplayDialogueLine(DialogueLineDatum dialogueLineDatum)
        {
            var processData = ProcessDialogueLine(dialogueLineDatum);
            dialogueTextComponent.text = processData.FormattedText;
            _typewriterEffect.StartTyping(processData.BareText);
        }

        private void SkipDialogueLine()
        {
            
        }

        private DialogueEffectHandler.ProcessData ProcessDialogueLine(DialogueLineDatum dialogueLineDatum)
        {
            return _dialogueEffectHandler.ProcessTags(dialogueLineDatum.Text);
        }
    }
}
