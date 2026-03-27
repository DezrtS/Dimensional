using System;
using Interfaces;
using Managers;
using Scriptables.Cutscenes;
using Scriptables.Save;
using Scriptables.Scene_Transitions;
using Scriptables.User_Interface;
using Systems.Cutscenes;
using UnityEngine;
using User_Interface;

namespace Systems.Interactables
{
    public class Catapult : Interactable
    {
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementPoint;

        [SerializeField] private StringVariable lastCheckpointSaveData;
        [SerializeField] private bool reverse;
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private string destinationId;
        
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;
        
        private WorldUIAnchor _worldUIAnchor;

        private void OnEnable()
        {
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(false);
        }

        private void OnDisable()
        {
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(true);
        }
        
        private void Start()
        {
            _worldUIAnchor = UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementPoint);
        }

        public void DisableWorldUIAnchor()
        {
            if (_worldUIAnchor) _worldUIAnchor.SetIsDisabled(true);
        }

        public override bool CanInteract(InteractContext interactContext)
        {
            return true;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            lastCheckpointSaveData.Value = destinationId;
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
        }
        
        public void LoadNextScene() 
        {
            SceneManager.Instance.LoadSceneWithTransition(reverse ? sceneTransition.SceneName : sceneTransition.DestinationSceneName);
        }
    }
}
