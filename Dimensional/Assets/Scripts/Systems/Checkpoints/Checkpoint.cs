using System;
using System.Collections;
using Interfaces;
using Managers;
using UnityEngine;
using Utilities;

namespace Systems.Checkpoints
{
    public class Checkpoint : MonoBehaviour, ISpawnPoint
    {
        private static readonly int BlendProperty = Shader.PropertyToID("_Blend");
        
        public event Action<ISpawnPoint> Entered;

        [SerializeField] private bool isDefaultCheckpoint;
        [SerializeField] private Vector3 spawnOffset;
        [Space] 
        [SerializeField] private Animator animator;
        [Space] 
        [SerializeField] private float transitionTime;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [Space] 
        [SerializeField] private bool isLimitedCheckpoint;
        [SerializeField] private int limit;
        [SerializeField] private Texture2D[] damagedTextures;

        private Material _material;
        private ObjectId _objectId;

        private bool _isActive;
        private int _respawnCount;
        
        public string Id => _objectId.Id;
        public bool IsDefaultSpawnPoint => isDefaultCheckpoint;

        public Vector3 Position => transform.position + spawnOffset;

        private void Awake()
        {
            _material = Instantiate(defaultMaterial);
            meshRenderer.material = _material;
            
            _objectId = GetComponent<ObjectId>();
        }

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            CheckpointManager.LastSpawnPointChanged += InstanceOnLastSpawnPointChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            CheckpointManager.LastSpawnPointChanged -= InstanceOnLastSpawnPointChanged;
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Initializing) return;
            CheckpointManager.Instance.AddSpawnPoint(this);
        }

        private void InstanceOnLastSpawnPointChanged(ISpawnPoint spawnPoint)
        {
            if (!spawnPoint.Id.Equals(Id)) return;
            Disable();
        }

        public void SpawnAt()
        {
            if (isLimitedCheckpoint)
            {
                var ratio = Mathf.Clamp(_respawnCount / (float)limit * (damagedTextures.Length), 0, damagedTextures.Length - 1);
                _material.SetTexture("_Mask", damagedTextures[(int)ratio]);
                _respawnCount++;

                if (_respawnCount >= limit) StartCoroutine(ExitedRoutine());
            }
            if (!animator) return;
            animator.SetTrigger("Open");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isActive) return;

            if (!other.gameObject.CompareTag("Player")) return;
            Entered?.Invoke(this);
            _isActive = true;
            StartCoroutine(EnteredRoutine());
        }

        private void Disable()
        {
            if (!_isActive) return;
            
            _isActive = false;
            StartCoroutine(ExitedRoutine());
        }
        
        private IEnumerator ExitedRoutine()
        {
            var timer = 0f;
            while (timer < transitionTime)
            {
                yield return null;
                timer += Time.deltaTime;
                _material.SetFloat(BlendProperty, 1 - timer / transitionTime);
            }
            
            _material.SetFloat(BlendProperty, 0);
        }

        private IEnumerator EnteredRoutine()
        {
            var timer = 0f;
            while (timer < transitionTime)
            {
                yield return null;
                timer += Time.deltaTime;
                _material.SetFloat(BlendProperty, timer / transitionTime);
            }
            
            _material.SetFloat(BlendProperty, 1);
        }
    }
}
