using System;
using System.Collections.Generic;
using Scriptables.Save;
using Systems.Checkpoints;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CheckpointManager : Singleton<CheckpointManager>
    {
        [SerializeField] private StringVariable lastCheckpointSaveData;
        
        private Dictionary<string, Checkpoint> _checkpoints;
        private string _lastCheckpointId = string.Empty;
        
        private void Awake()
        {
            _checkpoints = new Dictionary<string, Checkpoint>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Preparing) return;
            if (lastCheckpointSaveData.Value != string.Empty) _lastCheckpointId = lastCheckpointSaveData.Value;
        }
        
        public void AddCheckpoint(Checkpoint checkpoint)
        {
            if (checkpoint.Id == string.Empty)
            {
                Debug.LogError("Checkpoint Id is empty");
                return;
            }

            if (checkpoint.IsDefaultCheckpoint) _lastCheckpointId = checkpoint.Id;
            
            _checkpoints.Add(checkpoint.Id, checkpoint);
            checkpoint.Entered += CheckpointOnEntered;
        }

        private Checkpoint GetCheckpoint(string id)
        {
            if (id == string.Empty) return null;
            _checkpoints.TryGetValue(id, out var checkpoint);
            return checkpoint;
        }

        public Checkpoint GetLastCheckpoint() => GetCheckpoint(_lastCheckpointId);

        private void CheckpointOnEntered(Checkpoint checkpoint)
        {
            var lastCheckpoint = GetLastCheckpoint();
            if (lastCheckpoint) lastCheckpoint.Disable();
            _lastCheckpointId = checkpoint.Id;
            lastCheckpointSaveData.Value = checkpoint.Id;
        }
    }
}
