using System;
using Managers;
using UnityEngine;
using Utilities;

namespace Systems.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        public event Action<Checkpoint> Entered;

        [SerializeField] private bool isDefaultCheckpoint;
        [SerializeField] private Vector3 spawnOffset;
        
        private Animator _animator;
        private ObjectId _objectId;
        
        public string Id => _objectId.Id;
        public bool IsDefaultCheckpoint => isDefaultCheckpoint;
        public Vector3 SpawnPosition => transform.position + spawnOffset;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _objectId = GetComponent<ObjectId>();
        }

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
            if (newValue == GameState.Initializing) CheckpointManager.Instance.AddCheckpoint(this);
        }

        public void RespawnAt()
        {
            if (!_animator) return;
            _animator.SetTrigger("Open");
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
