using UnityEngine;
using Utilities;

namespace Managers
{
    public class SceneManager : Singleton<SceneManager>
    {
        private string _nextScene;
        private bool _loadingScene;

        public void LoadSceneWithTransition(string nextScene)
        {
            if (_loadingScene) return;
            
            _nextScene = nextScene;
            _loadingScene = true;
            UIManager.TransitionFinished += UIManagerOnTransitionFinished;
            UIManager.Instance.Transition(true, true);
        }

        private void UIManagerOnTransitionFinished()
        {
            UIManager.TransitionFinished -= UIManagerOnTransitionFinished;
            LoadScene(_nextScene);
        }

        public static void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}