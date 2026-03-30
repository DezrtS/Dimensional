using System;
using System.Linq;
using Interfaces;
using Managers;
using Scriptables.Dialogue;
using Scriptables.Events;
using Scriptables.Save;
using Scriptables.User_Interface;
using Systems.Events;
using Systems.Interactables;
using UnityEngine;
using User_Interface;

namespace Systems.NPCs
{
    [Serializable]
    public class CharacterDialogue
    {
        public DialogueSequenceDatum dialogueSequenceDatum;
        public BoolVariableInstance[] boolVariableInstances;

        public bool IsActive()
        {
            return boolVariableInstances.Length == 0 || boolVariableInstances.All(boolVariableInstance => boolVariableInstance.IsEnabled());
        }
    }
    
    public class Character : Interactable
    {
        [SerializeField] private CharacterDialogue[] characterDialogue;
        
        [SerializeField] private Transform elementTransform;
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private GameEventDatum[] gameEventData;
        
        private WorldUIAnchor _worldUIAnchor;
        private bool _wasDisabled;

        public override bool CanInteract(InteractContext interactContext)
        {
            return characterDialogue.Any(dialogue => dialogue.IsActive());
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            EventManager.HandleEvents(gameEventData);
            foreach (var dialogue in characterDialogue)
            {
                if (!dialogue.IsActive()) continue;
                var dialogueEvent = new DialogueEvent() { DialogueSequence = dialogue.dialogueSequenceDatum };
                dialogueEvent.Handle();
                return;
            }
        }
        
        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            foreach (var dialogue in characterDialogue)
            {
                foreach (var boolVariableInstance in dialogue.boolVariableInstances)
                {
                    boolVariableInstance.boolVariable.ValueChanged += OnValueChanged;
                }
            }

            if (!_wasDisabled) return;
            _wasDisabled = false;
            
            if (!_worldUIAnchor) return; 
            _worldUIAnchor.SetIsDisabled(!CanInteract(new InteractContext()));
        }
        
        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            foreach (var dialogue in characterDialogue)
            {
                foreach (var boolVariableInstance in dialogue.boolVariableInstances)
                {
                    boolVariableInstance.boolVariable.ValueChanged -= OnValueChanged;
                }
            }
            
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(true);
            _wasDisabled = true;
        }
        
        private void OnValueChanged()
        {
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(!CanInteract(new InteractContext()));
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.Initializing:
                    _worldUIAnchor = UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
                    break;
                case GameState.Preparing:
                    if (gameObject.activeInHierarchy) _worldUIAnchor.SetIsDisabled(!CanInteract(new InteractContext()));
                    break;
            }
        }
    }
}
