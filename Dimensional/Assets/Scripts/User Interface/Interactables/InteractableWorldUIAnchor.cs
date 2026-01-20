using System;
using Interfaces;
using Managers;
using Scriptables.Dialogue;
using Scriptables.User_Interface;
using Systems.Interactables;
using UnityEngine;

namespace User_Interface.Interactables
{
    public class InteractableWorldUIAnchor : WorldUIAnchor
    {
        private static readonly int InteractedHash = Animator.StringToHash("Interacted");
        private static readonly int HoveredHash = Animator.StringToHash("Hovered");
        private static readonly int UnHoveredHash = Animator.StringToHash("UnHovered");
        private static readonly int IsTargetInRangeHash = Animator.StringToHash("IsTargetInRange");
        
        private Interactable _interactable;
        private Animator _animator;
        private UIArm _uiArm;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _uiArm = GetComponent<UIArm>();
            
            DialogueManager.SequenceStarted += DialogueManagerOnSequenceStarted;
            DialogueManager.SequenceFinished += DialogueManagerOnSequenceFinished;
        }

        private void OnDisable()
        {
            DialogueManager.SequenceStarted -= DialogueManagerOnSequenceStarted;
            DialogueManager.SequenceFinished -= DialogueManagerOnSequenceFinished;
        }

        protected override void OnInitialize(WorldUIAnchorDatum worldUIAnchorDatum, GameObject holderGameObject, Transform worldTransform)
        {
            _interactable = holderGameObject.GetComponent<Interactable>();
            _interactable.Interacted += InteractableOnInteracted;
            _interactable.Hovered += InteractableOnHovered;
            _interactable.UnHovered += InteractableOnUnHovered;
        }

        protected override void OnFixedUpdate()
        {
            if (!_uiArm) return;
            var elementScreenPos = Camera.WorldToScreenPoint(WorldTransform.position + WorldUIAnchorDatum.Offset);
            var targetScreenPos = Camera.WorldToScreenPoint(WorldTransform.position);
            _uiArm.UpdateArm(elementScreenPos, targetScreenPos);
        }

        private void InteractableOnInteracted(Interactable interactable, InteractContext interactContext)
        {
            _animator.SetTrigger(InteractedHash);
        }

        private void InteractableOnHovered(Interactable interactable, InteractContext interactContext)
        {
            _animator.SetTrigger(HoveredHash);
        }
        
        private void InteractableOnUnHovered(Interactable interactable, InteractContext interactContext)
        {
            _animator.SetTrigger(UnHoveredHash);
        }

        protected override void OnSetIsTargetInRange(bool isTargetInRange)
        {
            _animator.SetBool(IsTargetInRangeHash, isTargetInRange);
        }

        private void DialogueManagerOnSequenceStarted(DialogueSequenceDatum dialogueSequenceDatum)
        {
            DisableScaling = true;
            transform.localScale = Vector3.zero;
        }
        
        private void DialogueManagerOnSequenceFinished(DialogueSequenceDatum dialogueSequenceDatum)
        {
            DisableScaling = false;
            transform.localScale = Vector3.one;
        }
    }
}
