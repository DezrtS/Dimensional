using System;
using System.Collections.Generic;
using Systems.Checkpoints;
using Utilities;

namespace Managers
{
    public class CheckpointManager : Singleton<CheckpointManager>
    {
        private Dictionary<int, Checkpoint> _checkpoints;
        private int _lastCheckpointId;

        protected override void OnEnable()
        {
            SaveManager.Saving += SaveManagerOnSaving;
            SaveManager.Loaded += SaveManagerOnLoaded;
            base.OnEnable();
        }

        private void Awake()
        {
            _checkpoints = new Dictionary<int, Checkpoint>();
            _lastCheckpointId = 0;
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
            _checkpoints.Add(checkpoint.Id, checkpoint);
            checkpoint.Entered += CheckpointOnEntered;
        }

        public Checkpoint GetCheckpoint(int id)
        {
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
