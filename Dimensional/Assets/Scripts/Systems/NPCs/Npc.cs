using System;
using Managers;
using Scriptables.Dialogue;
using Scriptables.User_Interface;
using UnityEngine;
using User_Interface.Dialogue;

namespace Systems.NPCs
{
    public class Npc : MonoBehaviour
    {
        [SerializeField] private Transform elementTransform;
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        
        [SerializeField] private DialogueLineDatum dialogueLineDatum;

        private void Start()
        {
            var speechBox = (SpeechBox)UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
            if (dialogueLineDatum) speechBox.SetDialogueLine(dialogueLineDatum.DialogueLine);
        }
    }
}