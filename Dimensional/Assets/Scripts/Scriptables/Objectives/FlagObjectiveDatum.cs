using Systems.Objectives;
using UnityEngine;

namespace Scriptables.Objectives
{
    [CreateAssetMenu(fileName = "FlagObjectiveDatum", menuName = "Scriptable Objects/Objectives/FlagObjectiveDatum")]
    public class FlagObjectiveDatum : ObjectiveDatum
    {
        [SerializeField] private string flagId;
        
        public string FlagId => flagId;

        public override Objective CreateObjective()
        {
            var flagObjective = new FlagObjective();
            flagObjective.Initialize(this);
            return flagObjective;
        }

        public override Objective CreateObjective(string rawData)
        {
            var flagObjective = JsonUtility.FromJson<FlagObjective>(rawData);
            flagObjective.Initialize(this);
            return flagObjective;
        }
    }
}
