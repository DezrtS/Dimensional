using System;
using Systems.Grass;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class EffectManager : Singleton<EffectManager>
    {
        public GrassSystem GrassSystem { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }

        private void Awake()
        {
            GrassSystem = GetComponentInChildren<GrassSystem>();
        }
        
        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue == GameState.Preparing)
            {
                GrassSystem.GenerateGrass();
            }
        }
    }
}
