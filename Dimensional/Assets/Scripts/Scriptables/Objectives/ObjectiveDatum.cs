using Systems.Objectives;
using UnityEngine;

namespace Scriptables.Objectives
{
    public abstract class ObjectiveDatum : ScriptableObject
    {
        [SerializeField] private string objectiveId;
        [SerializeField] private bool canUndo;
        [Space]
        [SerializeField] private string objectiveName;
        [TextArea(3, 8)] [SerializeField] private string objectiveDescription;
        
        public string ObjectiveId => objectiveId;
        public bool CanUndo => canUndo;
        
        public string ObjectiveName => objectiveName;
        public string ObjectiveDescription => objectiveDescription;

        public abstract Objective CreateObjective();
        public abstract Objective CreateObjective(string rawData);
    }
}
