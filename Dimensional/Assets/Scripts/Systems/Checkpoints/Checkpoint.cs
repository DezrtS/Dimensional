using System;
using Managers;
using UnityEngine;

namespace Systems.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        public event Action<Checkpoint> Entered;
        
        [SerializeField] private int id;
        [SerializeField] private Vector3 spawnOffset;
        
        public int Id => id;
        public Vector3 SpawnPosition => transform.position + spawnOffset;

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
            if (newValue == GameState.SettingUp) CheckpointManager.Instance.AddCheckpoint(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Entered?.Invoke(this);
            }
        }
    }
}
