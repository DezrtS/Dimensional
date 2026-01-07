using Managers;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "LoadNextSceneEventDatum", menuName = "Scriptable Objects/Events/LoadNextSceneEventDatum")]
    public class LoadNextSceneEventDatum : EventDatum
    {
        [SerializeField] private bool saveData = true;
        
        public override void HandleEvent()
        {
            SceneManager.Instance.LoadScene(saveData);
        }
    }
}
