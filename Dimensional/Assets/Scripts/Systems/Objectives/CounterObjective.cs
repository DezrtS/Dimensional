using Scriptables.Objectives;
using Systems.Events;
using UnityEngine;

namespace Systems.Objectives
{
    public class CounterObjective : Objective
    {
        private CounterObjectiveDatum _counterObjectiveDatum;
        public int CounterValue;
        
        public override void Initialize(ObjectiveDatum objectiveDatum)
        {
            base.Initialize(objectiveDatum);
            _counterObjectiveDatum = objectiveDatum as CounterObjectiveDatum;
        }

        public override void TryProgress(GameEvent gameEvent)
        {
            if (gameEvent is not CounterEvent counterEvent ||
                counterEvent.CounterId != _counterObjectiveDatum.CounterId) return;
            CounterValue = counterEvent.CounterValue;
            SetIsCompleted(CounterValue >= _counterObjectiveDatum.RequiredAmount);
        }

        public override string GetSaveData()
        {
            return base.GetSaveData() + JsonUtility.ToJson(this);
        }
    }
}
