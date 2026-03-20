using System;
using Scriptables.Cutscenes;
using Systems.Cutscenes;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CutsceneManager : Singleton<CutsceneManager>
    {
        public static event Action CutsceneFinished;
        [SerializeField] private CutsceneDatum defaultCutsceneDatum;
        
        private Cutscene _selectedCutscene;

        public bool IsPlaying { get; private set; }

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
            if (newValue != GameState.Starting) return;
            if (!defaultCutsceneDatum) return;
            PlayCutscene(defaultCutsceneDatum);
        }

        public void PlayCutscene(CutsceneDatum cutsceneDatum)
        {
            var cutscene = cutsceneDatum.SpawnCutscene(transform);
            PlayCutscene(cutscene);
        }

        public void PlayCutscene(Cutscene cutscene, CutsceneDatum cutsceneDatum)
        {
            if (cutsceneDatum) cutscene.Initialize(cutsceneDatum);
            PlayCutscene(cutscene);
        }

        private void PlayCutscene(Cutscene cutscene)
        {
            _selectedCutscene = cutscene;
            _selectedCutscene.Play();
            _selectedCutscene.Stopped += CutsceneOnStopped;
            IsPlaying = true;
            UIManager.Instance.SetUIHidden(true);
        }

        private void CutsceneOnStopped(Cutscene cutscene)
        {
            cutscene.Stopped -= CutsceneOnStopped;
            CutsceneFinished?.Invoke();
            _selectedCutscene = null;
            IsPlaying = false;
            UIManager.Instance.SetUIHidden(false);
        }

        public void StopCutscene()
        {
            if (!_selectedCutscene) return;
            _selectedCutscene.Stop();
        }
    }
}
