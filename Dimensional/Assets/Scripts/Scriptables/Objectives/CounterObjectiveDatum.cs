using Systems.Objectives;
using UnityEngine;

namespace Scriptables.Objectives
{
    [CreateAssetMenu(fileName = "CounterObjectiveDatum", menuName = "Scriptable Objects/Objectives/CounterObjectiveDatum")]
    public class CounterObjectiveDatum : ObjectiveDatum
    {
        [SerializeField] private string counterId;
        [SerializeField] private int requiredAmount;
        
        public string CounterId => counterId;
        public int RequiredAmount => requiredAmount;


        public override Objective CreateObjective()
        {
            var counterObjective = new CounterObjective();
            counterObjective.Initialize(this);
            return counterObjective;
        }

        public override Objective CreateObjective(string rawData)
        {
            var counterObjective = JsonUtility.FromJson<CounterObjective>(rawData);
            counterObjective.Initialize(this);
            return counterObjective;
        }
    }
}
