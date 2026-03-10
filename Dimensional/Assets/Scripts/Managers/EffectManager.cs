using System;
using Systems.Grass;
using Systems.Visual_Effects;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class EffectManager : Singleton<EffectManager>
    {
        [SerializeField] private bool startRainOnAwake;
        
        public GrassSystem GrassSystem { get; private set; }
        
        private RainEffect _rainEffect;
        private SkyboxBlender _skyboxBlender;

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
            _rainEffect = GetComponentInChildren<RainEffect>();
            _skyboxBlender = GetComponent<SkyboxBlender>();
            
            if (startRainOnAwake) _rainEffect.StartRain();
        }

        public void StartStorm()
        {
            if (_skyboxBlender) _skyboxBlender.StartBlend();
            _rainEffect.StartRain();
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
