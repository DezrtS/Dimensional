using Systems.Objectives;
using UnityEngine;

namespace Scriptables.Objectives
{
    [CreateAssetMenu(fileName = "CollectablesObjectiveDatum", menuName = "Scriptable Objects/Objectives/CollectablesObjectiveDatum")]
    public class CollectablesObjectiveDatum : ObjectiveDatum
    {
        [SerializeField] private int requiredAmount;
        public int RequiredAmount => requiredAmount;

        public override Objective CreateObjective()
        {
            var collectablesObjective = new CollectablesObjective();
            collectablesObjective.Initialize(this);
            return collectablesObjective;
        }

        public override Objective CreateObjective(string rawData)
        {
            var collectablesObjective = JsonUtility.FromJson<CollectablesObjective>(rawData);
            collectablesObjective.Initialize(this);
            return collectablesObjective;
        }
    }
}
