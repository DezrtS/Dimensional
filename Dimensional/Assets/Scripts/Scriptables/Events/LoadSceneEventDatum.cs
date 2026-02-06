using Systems.Events;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "LoadSceneEventDatum", menuName = "Scriptable Objects/Events/LoadSceneEventDatum")]
    public class LoadSceneEventDatum : GameEventDatum
    {
        [SerializeField] private LoadSceneEvent loadSceneEvent;
        public override GameEvent GameEvent => loadSceneEvent;
    }
}
