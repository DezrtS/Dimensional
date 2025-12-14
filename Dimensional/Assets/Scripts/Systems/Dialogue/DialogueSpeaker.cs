using System;
using Managers;
using Scriptables.Dialogue;
using Scriptables.User_Interface;
using Systems.Cameras;
using UnityEngine;
using User_Interface.Dialogue;

namespace Systems.Dialogue
{
    public class DialogueSpeaker : MonoBehaviour
    {
        [SerializeField] private DialogueSpeakerDatum dialogueSpeakerDatum;
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementPoint;

        private bool _isSpeaking;
        private SpeechBox _speechBox;
        
        private CameraTransition _cameraTransition;

        private void Awake()
        {
            _cameraTransition = GetComponent<CameraTransition>();
        }

        private void Start()
        {
            _speechBox = (SpeechBox)UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementPoint);
        }

        private void OnEnable()
        {
            DialogueManager.DialogueSpoken += DialogueManagerOnDialogueSpoken;
            DialogueManager.DialogueSkipped += DialogueManagerOnDialogueSkipped;
            DialogueManager.SequenceFinished += DialogueManagerOnSequenceFinished;
        }

        private void OnDisable()
        {
            DialogueManager.DialogueSpoken -= DialogueManagerOnDialogueSpoken;
            DialogueManager.DialogueSkipped -= DialogueManagerOnDialogueSkipped;
            DialogueManager.SequenceFinished -= DialogueManagerOnSequenceFinished;
        }
        
        private void DialogueManagerOnSequenceFinished(DialogueSequenceDatum dialogueSequenceDatum)
        {
            if (!_isSpeaking) return;
            _isSpeaking = false;
            _speechBox.HideDialogue();
            if (_cameraTransition) _cameraTransition.TransitionFrom();
        }

        private void DialogueManagerOnDialogueSpoken(DialogueLineDatum dialogueLineDatum)
        {
            if (dialogueSpeakerDatum != dialogueLineDatum.DialogueSpeakerDatum)
            {
                if (!_isSpeaking) return;
                _isSpeaking = false;
                _speechBox.HideDialogue();
                return;
            }
            
            _isSpeaking = true;
            _speechBox.SetDialogueLine(dialogueLineDatum);
            _speechBox.ShowDialogue();
            if (_cameraTransition) _cameraTransition.TransitionTo();
        }
        
        private void DialogueManagerOnDialogueSkipped()
        {
            if (!_isSpeaking) return;
            _speechBox.SkipDialogue();
        }
    }
}
