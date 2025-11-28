using Scriptables.Events;
using Systems.Cutscenes;
using UnityEngine;

namespace Scriptables.Cutscenes
{
    [CreateAssetMenu(fileName = "CutsceneDatum", menuName = "Scriptable Objects/Cutscenes/CutsceneDatum")]
    public class CutsceneDatum : ScriptableObject
    {
        [SerializeField] private GameObject cutscenePrefab;
        [SerializeField] private string cutsceneName;
        [SerializeField] private bool destroyOnStopped;
        
        [SerializeField] private EventDatum[] onPlayEventData;
        [SerializeField] private EventDatum[] onStopEventData;
        
        public string CutsceneName => cutsceneName;
        public bool DestroyOnStopped => destroyOnStopped;
        
        public EventDatum[] OnPlayEventData => onPlayEventData;
        public EventDatum[] OnStopEventData => onStopEventData;

        public Cutscene SpawnCutscene(Transform parent)
        {
            var cutsceneObject = Instantiate(cutscenePrefab, parent);
            var cutscene = cutsceneObject.GetComponent<Cutscene>();
            cutscene.Initialize(this);
            return cutscene;
        }
    }
}
