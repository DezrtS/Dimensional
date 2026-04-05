using System;
using Managers;
using Scriptables.Objectives;
using Scriptables.User_Interface;
using UnityEngine;
using User_Interface;

namespace Systems.Objectives
{
    public class ObjectiveLocation : MonoBehaviour
    {
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;
        [SerializeField] private Transform elementPoint;
        
        private WorldUIAnchor _worldUIAnchor;
        private bool _isEnabled;
        private bool _canReveal = true;

        private void Awake()
        {
            QuestManager.ObjectivesRevealedStateChanged += QuestManagerOnObjectivesRevealedStateChanged;
        }

        private void OnDestroy()
        {
            QuestManager.ObjectivesRevealedStateChanged -= QuestManagerOnObjectivesRevealedStateChanged;
        }

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            //QuestManager.ObjectivesRevealedStateChanged += QuestManagerOnObjectivesRevealedStateChanged;
            _isEnabled = true;
            if (_worldUIAnchor && _canReveal) _worldUIAnchor.SetIsDisabled(!_isEnabled);
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            //QuestManager.ObjectivesRevealedStateChanged -= QuestManagerOnObjectivesRevealedStateChanged;
            _isEnabled = false;
            if (_worldUIAnchor && _canReveal) _worldUIAnchor.SetIsDisabled(!_isEnabled);
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.Initializing:
                    _worldUIAnchor = UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementPoint);
                    break;
            }
        }
        
        private void QuestManagerOnObjectivesRevealedStateChanged(bool isRevealed)
        {
            _canReveal = isRevealed;
            if (!_worldUIAnchor) return;
            if (isRevealed && _isEnabled) _worldUIAnchor.SetIsDisabled(false);
            else _worldUIAnchor.SetIsDisabled(true);
        }
    }
}
