using System;
using Febucci.UI;
using Scriptables.Dialogue;
using Scriptables.User_Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace User_Interface.Dialogue
{
    public class SpeechBox : WorldUIAnchor
    {
        private static readonly int Show = Animator.StringToHash("Show");
        private static readonly int Hide = Animator.StringToHash("Hide");
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TypewriterByCharacter typewriterByCharacter;
        [Space] 
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        
        private DialogueLineDatum _dialogueLineDatum;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        protected override void OnInitialize(WorldUIAnchorDatum worldUIAnchorDatum, Transform worldTransform) {}

        public void SetDialogueLine(DialogueLineDatum dialogueLineDatum)
        {
            _dialogueLineDatum = dialogueLineDatum;
            nameText.text = dialogueLineDatum.DialogueSpeakerDatum.SpeakerName;
            iconImage.sprite = dialogueLineDatum.DialogueSpeakerDatum.SpeakerIcon;
            textAnimator.SetText(_dialogueLineDatum.Text, true);
        }

        protected override void OnSetTargetTransform(Transform targetTransform)
        {
            if (!targetTransform)
            {
                HideSpeechBox();
            }
            else
            {
                ShowSpeechBox();
            }
        }

        [ContextMenu("Show")]
        public void ShowSpeechBox()
        {
            _animator.SetTrigger(Show);
            ShowText();
        }
        
        private void ShowText()
        {
            typewriterByCharacter.StartShowingText();
        }

        [ContextMenu("Hide")]
        public void HideSpeechBox()
        {
            HideText();
            _animator.SetTrigger(Hide);
        }
        
        private void HideText()
        {
            typewriterByCharacter.StartDisappearingText();
        }
    }
}