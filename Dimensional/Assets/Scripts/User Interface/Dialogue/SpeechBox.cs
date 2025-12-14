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
        private static readonly int IsTargetInRangeHash = Animator.StringToHash("IsTargetInRange");
        
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TypewriterByCharacter typewriterByCharacter;
        [Space] 
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        
        private Animator _animator;
        private DialogueLineDatum _dialogueLineDatum;
        private UIArm _uiArm;

        private bool _isShown;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _uiArm = GetComponent<UIArm>();
        }

        protected override void OnInitialize(WorldUIAnchorDatum worldUIAnchorDatum, GameObject holderGameObject, Transform worldTransform) {}
        
        public void SetDialogueLine(DialogueLineDatum dialogueLineDatum)
        {
            _dialogueLineDatum = dialogueLineDatum;
            nameText.text = dialogueLineDatum.DialogueSpeakerDatum.SpeakerName;
            iconImage.sprite = dialogueLineDatum.DialogueSpeakerDatum.SpeakerIcon;
            textAnimator.SetText(_dialogueLineDatum.Text, true);
        }

        public void ShowDialogue()
        {
            _animator.SetBool(IsTargetInRangeHash, true);
            typewriterByCharacter.StartShowingText();
        }

        public void SkipDialogue()
        {
            typewriterByCharacter.SkipTypewriter();
        }
        
        public void HideDialogue()
        {
            _animator.SetBool(IsTargetInRangeHash, false);
            typewriterByCharacter.StartDisappearingText();
        }

        protected override void OnFixedUpdate()
        {
            var elementScreenPos = Camera.WorldToScreenPoint(WorldTransform.position + WorldUIAnchorDatum.Offset);
            var targetScreenPos = Camera.WorldToScreenPoint(WorldTransform.position);
            _uiArm.UpdateArm(elementScreenPos, targetScreenPos);
        }

        protected override void OnSetIsTargetInRange(bool isTargetInRange)
        {
            _animator.SetBool(IsTargetInRangeHash, isTargetInRange);
            if (isTargetInRange) typewriterByCharacter.StartShowingText();
            else typewriterByCharacter.StartDisappearingText();
        }
    }
}