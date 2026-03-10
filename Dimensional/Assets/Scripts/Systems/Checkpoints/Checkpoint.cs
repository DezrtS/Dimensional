using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Systems.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        private static readonly int BlendProperty = Shader.PropertyToID("_Blend");
        public event Action<Checkpoint> Entered;

        [SerializeField] private bool isDefaultCheckpoint;
        [SerializeField] private Vector3 spawnOffset;
        [Space] 
        [SerializeField] private Animator animator;
        [Space] 
        [SerializeField] private float transitionTime;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private SkinnedMeshRenderer meshRenderer;

        private Material _material;
        private ObjectId _objectId;

        private bool _isActive;
        
        public string Id => _objectId.Id;
        public bool IsDefaultCheckpoint => isDefaultCheckpoint;
        public Vector3 SpawnPosition => transform.position + spawnOffset;

        private void Awake()
        {
            _material = Instantiate(defaultMaterial);
            meshRenderer.material = _material;
            
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

        public void Disable()
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
