using System;
using Scriptables.Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Managers
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public delegate void DialogueSequenceEventHandler(DialogueSequenceDatum dialogueSequenceDatum);
        public static event DialogueSequenceEventHandler SequenceStarted;
        public static event DialogueSequenceEventHandler SequenceFinished;
        
        public delegate void DialogueSpeakerEventHandler(DialogueLineDatum dialogueLineDatum);
        public static event DialogueSpeakerEventHandler DialogueSpoken;
        public static event Action DialogueSkipped;
        
        [Header("Dialogue Settings")]
        [SerializeField] private bool startDialogueOnStart;
        [SerializeField] private DialogueSequenceDatum currentDialogueSequenceDatum;

        private int _currentDialogueLineIndex;
        private bool _isDialogueActive;
        
        private InputActionMap _inputActionMap;

        private void Start()
        {
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Dialogue");
            if (!startDialogueOnStart) return;
            StartDialogueSequence(currentDialogueSequenceDatum);
        }

        private void OnDisable()
        {
            UnassignControls();
        }

        private void AssignControls()
        {
            var skipInputAction = _inputActionMap.FindAction("Skip");
            skipInputAction.performed += OnSkip;
        }

        private void UnassignControls()
        {
            var skipInputAction = _inputActionMap.FindAction("Skip");
            skipInputAction.performed -= OnSkip;
        }
        
        private void OnSkip(InputAction.CallbackContext context)
        {
            AdvanceDialogueSequence();
        }

        public void StartDialogueSequence(DialogueSequenceDatum dialogueSequence)
        {
            GameManager.Instance.SwitchInputActionMaps("Dialogue");
            AssignControls();
            
            SequenceStarted?.Invoke(dialogueSequence);
            currentDialogueSequenceDatum = dialogueSequence;
            _isDialogueActive = true;
            _currentDialogueLineIndex = 0;
            DisplayDialogueLine(dialogueSequence);
        }

        public void AdvanceDialogueSequence()
        {
            if (!_isDialogueActive)
            {
                if (currentDialogueSequenceDatum) StartDialogueSequence(currentDialogueSequenceDatum);
                return;
            }
            
            _currentDialogueLineIndex++;
            if (currentDialogueSequenceDatum.DialogueLineData.Length <= _currentDialogueLineIndex)
            {
                StopDialogueSequence();
            }
            else
            {
                DisplayDialogueLine(currentDialogueSequenceDatum);
            }
        }

        public void StopDialogueSequence()
        {
            UnassignControls();
            GameManager.Instance.ResetInputActionMapToDefault();
            
            SequenceFinished?.Invoke(currentDialogueSequenceDatum);
            currentDialogueSequenceDatum = null;
            _isDialogueActive = false;
            _currentDialogueLineIndex = 0;
        }

        private void DisplayDialogueLine(DialogueSequenceDatum dialogueSequence) => DisplayDialogueLine(dialogueSequence.DialogueLineData[_currentDialogueLineIndex]);

        private void DisplayDialogueLine(DialogueLineDatum dialogueLineDatum)
        {
            DialogueSpoken?.Invoke(dialogueLineDatum);
        }

        private void SkipDialogueLine()
        {
            DialogueSkipped?.Invoke();
        }
    }
}
