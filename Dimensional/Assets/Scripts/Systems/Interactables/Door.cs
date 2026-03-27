using System;
using Interfaces;
using Managers;
using Scriptables.Audio;
using Scriptables.Events;
using Scriptables.Save;
using Scriptables.Scene_Transitions;
using Scriptables.User_Interface;
using Systems.Cameras;
using Systems.Forces;
using UnityEngine;
using User_Interface;
using Utilities;

namespace Systems.Interactables
{
    public class Door : Interactable, ISpawnPoint
    {
        public delegate void DoorEventHandler(InteractContext interactContext, string from, string to);
        public static event DoorEventHandler Opened;
        public event Action<ISpawnPoint> Entered;
        
        [Space]
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementTransform;
        [SerializeField] private GameEventDatum[] gameEventData;
        [Space]
        [SerializeField] private Transform spawnTransform;
        [SerializeField] private string id;
        [Space]
        [SerializeField] private SpawnPointAudioDatum spawnPointAudioDatum;
        [Space]
        [SerializeField] private string destinationId;
        [Space]
        [SerializeField] private bool destinationIsScene;
        [SerializeField] private bool reverseDirection;
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private StringVariable lastCheckpointSaveData;
        [Space] 
        [SerializeField] private BoolVariable canInteractSaveData;
        [Space] 
        [SerializeField] private bool changeDefaultCamera;
        [SerializeField] private int cameraId;

        private CameraTransition _cameraTransition;
        private WorldUIAnchor _worldUIAnchor;

        public string Id => id;
        public Vector3 Position => spawnTransform.position;
        public SpawnPointAudioDatum SpawnPointAudioDatum => spawnPointAudioDatum;
        public bool IsDefaultSpawnPoint => false;

        private void OnEnable()
        {
            Opened += DoorOnOpened;
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            if (canInteractSaveData) canInteractSaveData.ValueChanged += CanInteractSaveDataOnValueChanged;
        }

        private void OnDisable()
        {
            Opened -= DoorOnOpened;
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            if (canInteractSaveData) canInteractSaveData.ValueChanged -= CanInteractSaveDataOnValueChanged;
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(true);
        }
        
        private void CanInteractSaveDataOnValueChanged()
        {
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(!canInteractSaveData.Value);
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.Initializing:
                    CheckpointManager.Instance.AddSpawnPoint(this);
                    break;
                case GameState.Preparing:
                    _worldUIAnchor = UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
                    if (canInteractSaveData) _worldUIAnchor.SetIsDisabled(!canInteractSaveData.Value);
                    break;
            }
        }

        private void Awake()
        {
            if (!TryGetComponent(out _cameraTransition)) return;
            _cameraTransition.TransitionToFinished += CameraTransitionOnTransitionToFinished;
            _cameraTransition.TransitionFromFinished += CameraTransitionOnTransitionFromFinished;
        }

        private void CameraTransitionOnTransitionToFinished()
        {
            if (changeDefaultCamera) CameraManager.Instance.InvokeSetCinemachineCamera(cameraId);
            HandleInteraction();
        }
        
        private void CameraTransitionOnTransitionFromFinished()
        {
            
        }
        
        public void SpawnAt()
        {
            if (changeDefaultCamera) CameraManager.Instance.InvokeSetCinemachineCamera(cameraId);
            if (_cameraTransition) _cameraTransition.TransitionFrom();
        }

        private void DoorOnOpened(InteractContext interactContext, string from, string to)
        {
            if (id != to) return;
            interactContext.SourceGameObject.GetComponent<ForceController>().Teleport(spawnTransform.position);
            if (_cameraTransition) _cameraTransition.TransitionFrom();
        }
        
        public override bool CanInteract(InteractContext interactContext)
        {
            return !canInteractSaveData || canInteractSaveData.Value;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            if (_cameraTransition) _cameraTransition.TransitionTo();
            else HandleInteraction();
        }

        private void HandleInteraction()
        {
            EventManager.HandleEvents(gameEventData);
            if (destinationId == string.Empty) return;
            
            if (destinationIsScene)
            {
                if (lastCheckpointSaveData) lastCheckpointSaveData.Value = destinationId;
                if (sceneTransition) SceneManager.Instance.LoadSceneWithTransition(reverseDirection ? sceneTransition.SceneName : sceneTransition.DestinationSceneName);
                else SceneManager.Instance.LoadSceneWithTransition(destinationId);
            }
            else
            {
                Opened?.Invoke(PreviousInteractContext, id, destinationId);   
            }
        }
    }
}
