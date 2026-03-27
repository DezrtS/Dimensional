using Systems.Objectives;
using UnityEngine;

namespace Scriptables.Objectives
{
    [CreateAssetMenu(fileName = "ObjectiveDatum", menuName = "Scriptable Objects/Objectives/ObjectiveDatum")]
    public class ObjectiveDatum : ScriptableObject
    {
        [SerializeField] private string objectiveId;
        [SerializeField] private string objectiveName;
        [TextArea(3, 8)] [SerializeField] private string objectiveDescription;
        
        public string ObjectiveId => objectiveId;
        public string ObjectiveName => objectiveName;
        public string ObjectiveDescription => objectiveDescription;
    }
}
