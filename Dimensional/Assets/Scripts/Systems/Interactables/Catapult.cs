using System;
using Interfaces;
using Managers;
using Scriptables.Cutscenes;
using Scriptables.User_Interface;
using Systems.Cameras;
using Systems.Cutscenes;
using UnityEngine;

namespace Systems.Interactables
{
    public class Catapult : Interactable
    {
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementPoint;
        
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;

        private void Start()
        {
            UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementPoint);
        }

        public override bool CanInteract(InteractContext interactContext)
        {
            return true;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
            //EventManager.SendEvents(InteractableDatum.EventData);
        }
    }
}
