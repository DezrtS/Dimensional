using System;
using System.Collections.Generic;
using Interfaces;
using Scriptables.Save;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CheckpointManager : Singleton<CheckpointManager>
    {
        public static event Action<ISpawnPoint> LastSpawnPointChanged;
        
        [SerializeField] private StringVariable lastSpawnPointSaveData;
        
        private Dictionary<string, ISpawnPoint> _spawnPoints;
        private string _lastSpawnPointId = string.Empty;

        private bool _lastSpawnPointExists;
        
        private void Awake()
        {
            _spawnPoints = new Dictionary<string, ISpawnPoint>();
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
            if (!lastSpawnPointSaveData) return;
            if (lastSpawnPointSaveData.Value == string.Empty) return;
            if (_spawnPoints.ContainsKey(lastSpawnPointSaveData.Value)) _lastSpawnPointId = lastSpawnPointSaveData.Value;
        }

        public void AddSpawnPoint(ISpawnPoint spawnPoint)
        {
            if (spawnPoint.Id == string.Empty)
            {
                Debug.LogWarning("Spawn Point Id is Missing");
                return;
            }
            
            if (!_spawnPoints.TryAdd(spawnPoint.Id, spawnPoint))
            {
                Debug.LogWarning("Duplicate Spawn Point Id");
                return;
            }

            if (spawnPoint.IsDefaultSpawnPoint) _lastSpawnPointId = spawnPoint.Id;
            spawnPoint.Entered += SpawnPointOnEntered;
        }

        private void SpawnPointOnEntered(ISpawnPoint spawnPoint)
        {
            var lastSpawnPoint = GetLastSpawnPoint();
            if (lastSpawnPoint != null) LastSpawnPointChanged?.Invoke(lastSpawnPoint);
            _lastSpawnPointId = spawnPoint.Id;
            lastSpawnPointSaveData.Value = spawnPoint.Id;
        }

        private ISpawnPoint GetSpawnPoint(string id)
        {
            if (id == string.Empty) return null;
            _spawnPoints.TryGetValue(id, out var spawnPoint);
            return spawnPoint;
        }
        
        public ISpawnPoint GetLastSpawnPoint() => GetSpawnPoint(_lastSpawnPointId);
    }
}
