using System;
using Interfaces;
using Managers;
using Scriptables.Audio;
using Scriptables.Cutscenes;
using Scriptables.Save;
using Systems.Cutscenes;
using UnityEngine;

namespace Systems.Checkpoints
{
    public class LandingSpot : MonoBehaviour, ISpawnPoint
    {
        public event Action<ISpawnPoint> Entered;
        
        [SerializeField] private Transform spawnTransform;
        [SerializeField] private string id;
        [Space]
        [SerializeField] private SpawnPointAudioDatum spawnPointAudioDatum;
        [Space]
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;

        public string Id => id;
        public Vector3 Position => spawnTransform.position;
        public SpawnPointAudioDatum SpawnPointAudioDatum => spawnPointAudioDatum;
        public bool IsDefaultSpawnPoint => false;
        public bool IsLimited => false;
        
        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.Initializing:
                    CheckpointManager.Instance.AddSpawnPoint(this);
                    break;
            }
        }
        
        public void SpawnAt()
        {
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
        }
    }
}
