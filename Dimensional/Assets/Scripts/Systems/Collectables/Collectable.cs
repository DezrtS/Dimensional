using System;
using System.Collections.Generic;
using Managers;
using Systems.Player;
using UnityEngine;
using Utilities;

namespace Systems.Collectables
{
    public class Collectable : MonoBehaviour
    {
        private static readonly int IsCollectedHash = Animator.StringToHash("IsCollected");
        
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
            SaveManager.Saving += SaveManagerOnSaving;
            SaveManager.Loaded += SaveManagerOnLoaded;
        }
        
        private void OnDisable()
        {
            SaveManager.Saving -= SaveManagerOnSaving;
            SaveManager.Loaded -= SaveManagerOnLoaded;
        }

        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Collectable) || _isGhosted || !_isCollected) return;
            saveData.collectableData.collectedCollectables.Add(_objectId.Id);
        }
        
        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Collectable)) return;
            if (!saveData.collectableData.collectedCollectables.Contains(_objectId.Id)) return;
            Ghost();
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
            _animator.SetBool(IsCollectedHash, true);
        }
    }
}