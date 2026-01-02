using Managers;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "ActiveSceneEventDatum", menuName = "Scriptable Objects/Events/ActiveSceneEventDatum")]
    public class ActiveSceneEventDatum : EventDatum
    {
        [SerializeField] private string sceneName;
        
        public override void HandleEvent()
        {
            SceneManager.Instance.SetNextScene(sceneName);
        }
    }
}
