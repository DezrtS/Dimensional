using System;
using Scriptables.Save;
using Systems.Actions.Movement;
using UnityEngine;

namespace Utilities
{
    public class UnlockAction : MonoBehaviour
    {
        [SerializeField] private MovementActionType[] movementActionTypes;
        [SerializeField] private IntListVariable unlockedActionsSaveData;

        private bool _isTriggered;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered || !other.CompareTag("Player")) return;
            _isTriggered = true;
            
            foreach (var movementActionType in movementActionTypes)
            {
                if (unlockedActionsSaveData.Value.list.Contains((int)movementActionType)) continue;
                unlockedActionsSaveData.AddValue((int)movementActionType);
            }
        }
    }
}
