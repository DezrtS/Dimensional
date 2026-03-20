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
        
        private SpeechBox _speechBox;

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }
        
        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            if (_speechBox) _speechBox.SetIsDisabled(true);
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Initializing) return;
            _speechBox = (SpeechBox)UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
            if (dialogueLineDatum) _speechBox.SetDialogueLine(dialogueLineDatum.DialogueLine);
        }
    }
}