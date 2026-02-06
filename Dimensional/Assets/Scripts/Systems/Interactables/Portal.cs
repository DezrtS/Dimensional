using System;
using Interfaces;
using Managers;
using Scriptables.Events;
using Scriptables.Levels;
using Scriptables.User_Interface;
using Systems.Cameras;
using UnityEngine;
using User_Interface.Interactables;
using Utilities;

namespace Systems.Interactables
{
    public class Portal : Interactable
    {
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementTransform;
        [SerializeField] private GameEventDatum[] gameEventData;
        [Space]
        [SerializeField] private Transform spawnTransform;
        [Space]
        [SerializeField] private LevelDatum levelDatum;
        
        private ObjectId _objectId;
        private CameraTransition _cameraTransition;

        private bool _isActive;

        private void Awake()
        {
            _objectId = GetComponent<ObjectId>();
            if (!TryGetComponent(out _cameraTransition)) return;
            _cameraTransition.TransitionToFinished += CameraTransitionOnTransitionToFinished;
        }

        private void CameraTransitionOnTransitionToFinished()
        {
            EventManager.HandleEvents(gameEventData);
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
            if (newValue != GameState.Initializing) return;
            var levelUIAnchor = (LevelUIAnchor)UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
            levelUIAnchor.SetLevelDatum(levelDatum);
        }

        public override bool CanInteract(InteractContext interactContext)
        {
            return true;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            _isActive = true;
            if (_cameraTransition) _cameraTransition.TransitionTo();
            else EventManager.HandleEvents(gameEventData);
        }
    }
}
