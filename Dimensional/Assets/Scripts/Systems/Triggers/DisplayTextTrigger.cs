using System;
using System.Collections.Generic;
using Managers;
using Systems.Events;
using UnityEngine;
using Utilities;

namespace Systems.Triggers
{
    public class DisplayTextTrigger : MonoBehaviour
    {
        [SerializeField] private DisplayTextEvent displayTextEvent;
        [SerializeField] private bool showAfterTrigger;
        [SerializeField] private bool hideOnExit;
        
        private ObjectId _objectId;
        private bool _isTriggered;
        private bool _isCompleted;

        private void OnEnable()
        {
            SaveManager.Saving += SaveManagerOnSaving;
            SaveManager.Loaded += SaveManagerOnLoaded;
        }

        private void OnDisable()
        {
            SaveManager.Saving -= SaveManagerOnSaving;
            SaveManager.Loaded -= SaveManagerOnLoaded;
        }

        private void Awake()
        {
            _objectId = GetComponent<ObjectId>();
        }
        
        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (_isCompleted || !_isTriggered) return;
            if (!dataTypes.Contains(DataType.World)) return;
            saveData.worldData.completedTutorials.Add(_objectId.Id);
        }
        
        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.World)) return;
            if (saveData.worldData.completedTutorials.Contains(_objectId.Id)) _isCompleted = true;
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