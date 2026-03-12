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
        
        private ObjectId _objectId;
        private bool _isTriggered;
        private bool _isCompleted;

        private void Awake()
        {
            _objectId = GetComponent<ObjectId>();
        }
        
        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (_isCompleted || !_isTriggered) return;
            if (!dataTypes.Contains(DataType.World)) return;
            saveData.worldData.completedCutscenes.Add(_objectId.Id);
        }
        
        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.World)) return;
            if (saveData.worldData.completedCutscenes.Contains(_objectId.Id)) _isCompleted = true;
        }
        

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered || _isCompleted || !other.CompareTag("Player")) return;
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
            _isTriggered = true;
        }
    }
}
