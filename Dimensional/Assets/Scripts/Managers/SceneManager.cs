using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class SceneManager : Singleton<SceneManager>
    {
        private string _nextScene = "MovementTest";
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

        public void SetNextScene(string nextScene)
        {
            _nextScene = nextScene;
        }

        public void LoadScene(string sceneName, bool saveData = true)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName) return;
            _nextScene = sceneName;
            _loadingScene = true;
            if (saveData) SaveManager.Instance.Save();
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public void LoadScene(bool saveData = true)
        {
            LoadScene(_nextScene, saveData);
        }

        public static void ReloadScene()
        {
            SaveManager.Instance.RequestSave(new List<DataType>() { DataType.Player, DataType.Action, DataType.Collectable, DataType.Quest, DataType.World });
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}