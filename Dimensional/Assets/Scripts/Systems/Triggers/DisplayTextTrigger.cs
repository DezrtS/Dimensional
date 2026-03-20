using System;
using System.Collections.Generic;
using Managers;
using Scriptables.Text;
using Systems.Events;
using UnityEngine;
using Utilities;

namespace Systems.Triggers
{
    public class DisplayTextTrigger : MonoBehaviour
    {
        [SerializeField] private TextDatum textDatum;
        [SerializeField] private DisplayTextEvent displayTextEvent;
        [SerializeField] private bool showAfterTrigger;
        [SerializeField] private bool hideOnExit;
        
        private bool _isTriggered;
        private bool _isCompleted;

        private void Awake()
        {
            if (textDatum) displayTextEvent.Text = textDatum.Text;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((_isTriggered && !showAfterTrigger) || _isCompleted) return;
            if (other.CompareTag("Player")) displayTextEvent.Handle();
        }

        private void OnTriggerExit(Collider other)
        {
            if ((_isTriggered && !showAfterTrigger) || _isCompleted) return;
            if (hideOnExit && other.CompareTag("Player")) new HideTextEvent { DisplayType = displayTextEvent.DisplayType }.Handle();
            _isTriggered = true;
        }
    }
}