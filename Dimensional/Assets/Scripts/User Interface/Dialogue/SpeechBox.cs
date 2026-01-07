using System;
using Febucci.UI;
using FMOD.Studio;
using Managers;
using Scriptables.Dialogue;
using Scriptables.User_Interface;
using Systems.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace User_Interface.Dialogue
{
    public class SpeechBox : WorldUIAnchor
    {
        public event Action TypewriterFinished;
        
        private static readonly int IsTargetInRangeHash = Animator.StringToHash("IsTargetInRange");
        
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TypewriterByCharacter typewriterByCharacter;
        [Space] 
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        
        private Animator _animator;
        private DialogueLine _dialogueLine;
        private UIArm _uiArm;
        
        private EventInstance _eventInstance;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _uiArm = GetComponent<UIArm>();

            typewriterByCharacter.onTextShowed.AddListener(OnTextShown);
        }

        private void OnTextShown()
        {
            typewriterByCharacter.SetTypewriterSpeed(1);
            TypewriterFinished?.Invoke();
        }

        protected override void OnInitialize(WorldUIAnchorDatum worldUIAnchorDatum, GameObject holderGameObject, Transform worldTransform) {}
        
        public void SetDialogueLine(DialogueLine dialogueLine)
        {
            _dialogueLine = dialogueLine;
            nameText.text = dialogueLine.DialogueSpeakerDatum.SpeakerName;
            iconImage.sprite = dialogueLine.DialogueSpeakerDatum.SpeakerIcon;
            textAnimator.SetText(_dialogueLine.Text, true);

            if (_eventInstance.isValid()) _eventInstance.stop(STOP_MODE.IMMEDIATE);
            
            if (!dialogueLine.HasVoiceActing) return;
            _eventInstance = AudioManager.CreateEventInstance(dialogueLine.EventReferenceWrapper.eventRef);
            AudioManager.AttachInstanceToGameObject(_eventInstance, WorldTransform.gameObject);
        }

        public void ShowDialogue()
        {
            _animator.SetBool(IsTargetInRangeHash, true);
            typewriterByCharacter.StartShowingText();
            
            if (!_dialogueLine.HasVoiceActing) return;
            _eventInstance.start();
        }

        public void SkipDialogue()
        {
            typewriterByCharacter.SetTypewriterSpeed(4);
            //typewriterByCharacter.SkipTypewriter();
        }
        
        public void HideDialogue()
        {
            _animator.SetBool(IsTargetInRangeHash, false);
            typewriterByCharacter.StartDisappearingText();
            
            if (!_dialogueLine.HasVoiceActing) return;
            _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
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