using System;
using Managers;
using UnityEngine;

namespace User_Interface
{
    public class ToggleObjectives : MonoBehaviour
    {
        private static readonly int IsRevealedHash = Animator.StringToHash("IsRevealed");
        private Animator _animator;
        
        private void OnEnable()
        {
            QuestManager.ObjectivesRevealedStateChanged += QuestManagerOnObjectivesRevealedStateChanged;
        }

        private void OnDisable()
        {
            QuestManager.ObjectivesRevealedStateChanged -= QuestManagerOnObjectivesRevealedStateChanged;
        }

        private void QuestManagerOnObjectivesRevealedStateChanged(bool isRevealed)
        {
            _animator.SetBool(IsRevealedHash, isRevealed);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
    }
}
