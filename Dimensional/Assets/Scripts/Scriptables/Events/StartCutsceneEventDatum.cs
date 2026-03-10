using Managers;
using Scriptables.Cutscenes;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "StartCutsceneEventDatum", menuName = "Scriptable Objects/Events/StartCutsceneEventDatum")]
    public class StartCutsceneEventDatum : EventDatum
    {
        [SerializeField] private CutsceneDatum cutsceneDatum;
    
        public override void HandleEvent()
        {
            CutsceneManager.Instance.PlayCutscene(cutsceneDatum);
        }
    }
}
