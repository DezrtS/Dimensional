using System;
using System.Collections.Generic;
using Managers;
using Scriptables.Save;
using Systems.Player;
using UnityEngine;
using Utilities;

namespace Systems.Collectables
{
    public class Collectable : MonoBehaviour
    {
        private static readonly int IsCollectedHash = Animator.StringToHash("IsCollected");

        [SerializeField] private StringListVariable collectedCollectablesSaveData;
        
        [SerializeField] private MeshRenderer meshRenderer;
        
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material ghostMaterial;
        
        private Collider _collider;
        private Animator _animator;
        private ObjectId _objectId;

        private bool _isGhosted;
        private bool _isCollected;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _animator = GetComponent<Animator>();
            _objectId = GetComponent<ObjectId>();
        }

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Preparing || !collectedCollectablesSaveData) return;
            if (collectedCollectablesSaveData.Value.list.Contains(_objectId.Id)) Ghost();
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }

        private void Ghost()
        {
            _isGhosted = true;
            meshRenderer.material = ghostMaterial;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isCollected || !other.CompareTag("Player")) return;
            if (!_isGhosted) other.GetComponent<Inventory>().AddCollectables(1);
            _collider.enabled = false;
            _isCollected = true;
            collectedCollectablesSaveData.AddValue(_objectId.Id);
            _animator.SetBool(IsCollectedHash, true);
        }
    }
}