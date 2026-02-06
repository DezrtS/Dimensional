using System.Collections.Generic;
using Managers;
using Scriptables.Levels;
using TMPro;
using UnityEngine;

namespace User_Interface.Interactables
{
    public class LevelUIAnchor : InteractableWorldUIAnchor
    {
        [SerializeField] private TextMeshProUGUI levelTitleText;
        [SerializeField] private TextMeshProUGUI regionText;
        [SerializeField] private TextMeshProUGUI levelTypeText;
        [SerializeField] private TextMeshProUGUI collectablesText;

        private LevelDatum _levelDatum;
        
        public void SetLevelDatum(LevelDatum levelDatum)
        {
            _levelDatum = levelDatum;
            levelTitleText.text = _levelDatum.LevelName;
            regionText.text = _levelDatum.RegionType.ToString();
            levelTypeText.text = _levelDatum.LevelType.ToString();
            collectablesText.text = $"Axioms Collected (0/{_levelDatum.CollectablesCount})";
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SaveManager.Loaded += SaveManagerOnLoaded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SaveManager.Loaded -= SaveManagerOnLoaded;
        }
        
        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            //throw new System.NotImplementedException();
        }
    }
}
