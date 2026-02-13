using System;
using Managers;

namespace Systems.Events
{
    [Serializable]
    public class LoadSceneEvent : GameEvent
    {
        public override EventBusType BusType => EventBusType.World;
        public string SceneName;
        public bool LoadWithScreenTransition;

        public override void Handle()
        {
            if (LoadWithScreenTransition) SceneManager.Instance.LoadSceneWithTransition(SceneName);
            else SceneManager.Instance.LoadScene(SceneName);
        }
    }
}
