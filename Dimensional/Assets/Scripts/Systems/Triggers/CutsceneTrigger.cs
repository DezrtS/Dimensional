using System;
using System.Collections.Generic;
using Managers;
using Scriptables.Cutscenes;
using Systems.Cutscenes;
using UnityEngine;
using Utilities;

namespace Systems.Triggers
{
    public class CutsceneTrigger : MonoBehaviour
    {
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;
        
        private bool _isTriggered;
        private bool _isCompleted;

        private void Awake()
        {
            cutscene.Initialize(cutsceneDatum);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered || _isCompleted || !other.CompareTag("Player")) return;
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
            _isTriggered = true;
        }
    }
}
