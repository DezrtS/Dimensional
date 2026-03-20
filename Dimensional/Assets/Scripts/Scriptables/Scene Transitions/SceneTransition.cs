using UnityEngine;

namespace Scriptables.Scene_Transitions
{
    [CreateAssetMenu(fileName = "SceneTransition", menuName = "Scriptable Objects/SceneTransition")]
    public class SceneTransition : ScriptableObject
    {
        [SerializeField] private string sceneName;
        [SerializeField] private string destinationSceneName;
        
        public string SceneName => sceneName;
        public string DestinationSceneName => destinationSceneName;
    }
}
