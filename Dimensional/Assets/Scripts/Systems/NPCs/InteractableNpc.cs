using System;
using Interfaces;
using Managers;
using Scriptables.Events;
using Scriptables.Save;
using Scriptables.User_Interface;
using Systems.Interactables;
using UnityEngine;
using User_Interface;

namespace Systems.NPCs
{
    public class InteractableNpc : Interactable
    {
        [SerializeField] private Transform elementTransform;
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private GameEventDatum[] gameEventData;

        [SerializeField] private bool canInteractOnFalse;
        [SerializeField] private BoolVariable canInteractSaveData;
        
        private WorldUIAnchor _worldUIAnchor;

        public override bool CanInteract(InteractContext interactContext)
        {
            return !canInteractSaveData || canInteractOnFalse != canInteractSaveData.Value;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            EventManager.HandleEvents(gameEventData);
        }
        
        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            if (canInteractSaveData) canInteractSaveData.ValueChanged += CanInteractSaveDataOnValueChanged;
            if (!_worldUIAnchor) return; 
            if (canInteractSaveData) _worldUIAnchor.SetIsDisabled(canInteractOnFalse == canInteractSaveData.Value);
            else _worldUIAnchor.SetIsDisabled(false);
        }
        
        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            if (canInteractSaveData) canInteractSaveData.ValueChanged -= CanInteractSaveDataOnValueChanged;
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(true);
        }
        
        private void CanInteractSaveDataOnValueChanged()
        {
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(canInteractOnFalse == canInteractSaveData.Value);
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.Initializing:
                    _worldUIAnchor = UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
                    break;
                case GameState.Preparing:
                    if (canInteractSaveData && gameObject.activeInHierarchy) _worldUIAnchor.SetIsDisabled(canInteractOnFalse == canInteractSaveData.Value);
                    break;
            }
        }
    }
}