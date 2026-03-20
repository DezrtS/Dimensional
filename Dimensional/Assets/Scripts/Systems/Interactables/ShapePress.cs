using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Scriptables.Cutscenes;
using Scriptables.Events;
using Scriptables.Save;
using Scriptables.Shapes;
using Scriptables.User_Interface;
using Systems.Cutscenes;
using UnityEngine;
using User_Interface;

namespace Systems.Interactables
{
    public class ShapePress : Interactable
    {
        [SerializeField] private IntListVariable unlockedShapesSaveData;
        [SerializeField] private NewShapes newShapesSaveData;
        [SerializeField] private EventDatum[] eventData;
        
        [SerializeField] private List<ShapeType> unlockableShapes = new List<ShapeType>();
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementTransform;
        
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;
        
        [SerializeField] private BoolVariable disableOnBoolSaveData;
        private WorldUIAnchor _worldUIAnchor;

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            if (disableOnBoolSaveData) disableOnBoolSaveData.ValueChanged += DisableOnBoolSaveDataOnValueChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            if (disableOnBoolSaveData) disableOnBoolSaveData.ValueChanged -= DisableOnBoolSaveDataOnValueChanged;
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.Initializing:
                    _worldUIAnchor = UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
                    break;
                case GameState.Preparing:
                    if (disableOnBoolSaveData) _worldUIAnchor.SetIsDisabled(disableOnBoolSaveData.Value);
                    break;
            }
        }
        
        private void DisableOnBoolSaveDataOnValueChanged()
        {
            if (_worldUIAnchor)
            {
                _worldUIAnchor.SetIsDisabled(disableOnBoolSaveData.Value);
            }
        }
        
        public override bool CanInteract(InteractContext interactContext)
        {
            return !disableOnBoolSaveData || !disableOnBoolSaveData.Value;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
            foreach (var unlockableShape in unlockableShapes)
            {
                var index = (int)unlockableShape;
                newShapesSaveData.SetNewShape(unlockableShape);
                if (!unlockedShapesSaveData.Value.list.Contains(index)) unlockedShapesSaveData.AddValue(index);
            }
            if (eventData is { Length: > 0 })EventManager.SendEvents(eventData);
        }
    }
}
