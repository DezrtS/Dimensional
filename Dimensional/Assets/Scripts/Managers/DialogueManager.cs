using System;
using System.Collections.Generic;
using Scriptables.Dialogue;
using Systems.Dialogue;
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
        
        public delegate void DialogueSpeakerEventHandler(DialogueLine dialogueLine);
        public static event DialogueSpeakerEventHandler DialogueSpoken;
        
        [Header("Dialogue Settings")]
        [SerializeField] private bool startDialogueOnStart;
        [SerializeField] private DialogueSequenceDatum currentDialogueSequenceDatum;

        private DialogueLine[] _dialogueLines;
        
        private int _currentDialogueLineIndex;
        private bool _isDialogueActive;

        private readonly List<DialogueSpeaker> _activeDialogueSpeakers = new List<DialogueSpeaker>();
        
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

        public void AddActiveDialogueSpeaker(DialogueSpeaker dialogueSpeaker)
        {
            _activeDialogueSpeakers.Add(dialogueSpeaker);
        }

        public void RemoveActiveDialogueSpeaker(DialogueSpeaker dialogueSpeaker)
        {
            _activeDialogueSpeakers.Remove(dialogueSpeaker);
        }
        
        private void OnSkip(InputAction.CallbackContext context)
        {
            if (_activeDialogueSpeakers.Count > 0)
            {
                if (!currentDialogueSequenceDatum.IsDialogueSkippable) return;
                foreach (var dialogueSpeaker in _activeDialogueSpeakers)
                {
                    dialogueSpeaker.SkipDialogue();
                }

                return;
            }
            
            AdvanceDialogueSequence();
        }

        public void StartDialogueSequence(DialogueSequenceDatum dialogueSequence)
        {
            currentDialogueSequenceDatum = dialogueSequence;
            LoadDialogueFromText(dialogueSequence.FileName, dialogueSequence.DialogueSpeakersData);
        }

        private void LoadDialogueFromText(string fileName, DialogueSpeakerDatum[] speakers)
        {
            StartCoroutine(FileLoader.LoadText(fileName, "Dialogue", (rawText) =>
            {
                var lines = DialogueTextParser.Parse(rawText, speakers);
                _dialogueLines = lines.ToArray();
                StartDialogueSequence();
            }));

        }

        private void StartDialogueSequence()
        {
            GameManager.Instance.SwitchInputActionMaps("Dialogue");
            AssignControls();
            
            SequenceStarted?.Invoke(currentDialogueSequenceDatum);
            _isDialogueActive = true;
            _currentDialogueLineIndex = 0;
            DisplayDialogueLine(_dialogueLines[_currentDialogueLineIndex]);
        }


        public void AdvanceDialogueSequence()
        {
            if (!_isDialogueActive)
            {
                if (currentDialogueSequenceDatum) StartDialogueSequence(currentDialogueSequenceDatum);
                return;
            }
            
            _currentDialogueLineIndex++;
            if (_dialogueLines.Length <= _currentDialogueLineIndex)
            {
                StopDialogueSequence();
            }
            else
            {
                DisplayDialogueLine(_dialogueLines[_currentDialogueLineIndex]);
            }
        }

        public void StopDialogueSequence()
        {
            UnassignControls();
            GameManager.Instance.ResetInputActionMapToDefault();
            
            SequenceFinished?.Invoke(currentDialogueSequenceDatum);
            currentDialogueSequenceDatum = null;
            _dialogueLines = null;
            _isDialogueActive = false;
            _currentDialogueLineIndex = 0;
        }
        
        private void DisplayDialogueLine(DialogueLine dialogueLine)
        {
            DialogueSpoken?.Invoke(dialogueLine);
        }
    }
}
