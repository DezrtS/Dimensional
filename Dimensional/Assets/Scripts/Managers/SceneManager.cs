using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class SceneManager : Singleton<SceneManager>
    {
        private string _nextScene;
        private bool _loadingScene;

        private void Awake()
        {
            SaveManager.Saving += SaveManagerOnSaving;
            SaveManager.Loaded += SaveManagerOnLoaded;
        }
        
        private void OnDisable()
        {
            SaveManager.Saving -= SaveManagerOnSaving;
            SaveManager.Loaded -= SaveManagerOnLoaded;
        }

        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (dataTypes.Contains(DataType.Scene))
            {
                saveData.sceneData.scene = _loadingScene ? _nextScene : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            }
        }
        
        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (dataTypes.Contains(DataType.Scene))
            {
                LoadScene(saveData.sceneData.scene);
            }
        }

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

        public void LoadScene(string sceneName)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName) return;
            _nextScene = sceneName;
            _loadingScene = true;
            SaveManager.Instance.RequestSave(new List<DataType>() { DataType.Scene });
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public void ReloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}