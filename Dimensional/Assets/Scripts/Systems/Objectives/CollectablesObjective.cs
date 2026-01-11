using System;
using Scriptables.Objectives;
using Systems.Events;
using UnityEngine;

namespace Systems.Objectives
{
    public class CollectablesObjective : Objective
    {
        private CollectablesObjectiveDatum _collectablesObjectiveDatum;
        
        public override void Initialize(ObjectiveDatum objectiveDatum)
        {
            base.Initialize(objectiveDatum);
            _collectablesObjectiveDatum = objectiveDatum as CollectablesObjectiveDatum;
        }

        public override void TryProgress(GameEvent gameEvent)
        {
            if (gameEvent is CollectablesChangedEvent collectablesChangedEvent)
            {
                SetIsCompleted(collectablesChangedEvent.Collectables >= _collectablesObjectiveDatum.RequiredAmount);
            }
        }

        public override string GetSaveData()
        {
            return base.GetSaveData() + JsonUtility.ToJson(this);
        }
    }
}
