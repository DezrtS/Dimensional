using System;
using Scriptables.Objectives;
using Systems.Events;
using UnityEngine;

namespace Systems.Objectives
{
    [Serializable]
    public abstract class Objective
    {
        private ObjectiveDatum _objectiveDatum;
        
        public bool IsCompleted;

        public virtual void Initialize(ObjectiveDatum objectiveDatum)
        {
            _objectiveDatum = objectiveDatum;
        }

        public abstract void TryProgress(GameEvent gameEvent);
        
        public void SetIsCompleted(bool isCompleted)
        {
            if (!IsCompleted && !_objectiveDatum.CanUndo) return;
            IsCompleted = isCompleted;
        }

        public virtual string GetSaveData()
        {
            return $"{_objectiveDatum.ObjectiveId}|";
        }
    }
}
