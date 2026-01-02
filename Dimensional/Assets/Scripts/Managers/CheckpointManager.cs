using System;
using System.Collections.Generic;
using Systems.Checkpoints;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CheckpointManager : Singleton<CheckpointManager>
    {
        private Dictionary<string, Checkpoint> _checkpoints;
        private string _lastCheckpointId = string.Empty;
        
        private void Awake()
        {
            _checkpoints = new Dictionary<string, Checkpoint>();
        }

        protected override void OnEnable()
        {
            SaveManager.Saving += SaveManagerOnSaving;
            SaveManager.Loaded += SaveManagerOnLoaded;
            base.OnEnable();
        }

        private void OnDisable()
        {
            SaveManager.Saving -= SaveManagerOnSaving;
            SaveManager.Loaded -= SaveManagerOnLoaded;
        }

        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.World)) return;
            saveData.worldData.checkpointId = _lastCheckpointId;
        }
        
        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.World)) return;
            _lastCheckpointId = saveData.worldData.checkpointId;
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
            _lastCheckpointId = checkpoint.Id;
        }
    }
}
