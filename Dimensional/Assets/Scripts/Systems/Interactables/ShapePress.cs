using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Scriptables.Cutscenes;
using Scriptables.Shapes;
using Scriptables.User_Interface;
using Systems.Cutscenes;
using Systems.Events;
using UnityEngine;

namespace Systems.Interactables
{
    public class ShapePress : Interactable
    {
        [SerializeField] private List<ShapeType> unlockableShapes = new List<ShapeType>();
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementTransform;
        
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;

        private void Start()
        {
            UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
        }
        
        public override bool CanInteract(InteractContext interactContext)
        {
            return true;
        }

        public override void Interact(InteractContext interactContext)
        {
            base.Interact(interactContext);
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
            foreach (var unlockableShape in unlockableShapes)
            {
                var unlockShapeEvent = new UnlockShapeEvent { ShapeType = unlockableShape };
                unlockShapeEvent.Handle();
            }
        }
    }
}
