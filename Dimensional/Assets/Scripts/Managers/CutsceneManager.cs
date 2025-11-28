using System;
using Scriptables.Cutscenes;
using Systems.Cutscenes;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CutsceneManager : Singleton<CutsceneManager>
    {
        [SerializeField] private CutsceneDatum defaultCutsceneDatum;
        
        private Cutscene _selectedCutscene;
        
        private void Awake()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Playing) return;
            if (!defaultCutsceneDatum) return;
            PlayCutscene(defaultCutsceneDatum);
        }

        public void PlayCutscene(CutsceneDatum cutsceneDatum)
        {
            _selectedCutscene = cutsceneDatum.SpawnCutscene(transform);
            _selectedCutscene.Play();
            _selectedCutscene.Stopped += CutsceneOnStopped;
        }

        private void CutsceneOnStopped(Cutscene cutscene)
        {
            cutscene.Stopped -= CutsceneOnStopped;
            _selectedCutscene = null;
        }

        public void StopCutscene()
        {
            if (!_selectedCutscene) return;
            _selectedCutscene.Stop();
        }
    }
}
