using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Scriptables.Quests;
using Systems.Events;
using Systems.Events.Busses;
using Systems.Objectives;
using UnityEngine;

namespace Systems.Quests
{
    public class Quest : MonoBehaviour
    {
        private QuestDatum _questDatum;
        private QuestState _questState;
        private List<Objective> _objectives;

        public void Initialize(QuestDatum questDatum)
        {
            _questDatum = questDatum;
            _questState = questDatum.DefaultQuestState;
        }

        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            
            SaveManager.Saving += SaveManagerOnSaving;
            SaveManager.Loaded += SaveManagerOnLoaded;
            
            QuestEventBus.EventFired += QuestEventBusOnEventFired;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            
            SaveManager.Saving -= SaveManagerOnSaving;
            SaveManager.Loaded -= SaveManagerOnLoaded;
            
            QuestEventBus.EventFired -= QuestEventBusOnEventFired;
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Preparing) return;
            _objectives ??= _questDatum.QuestObjectives
                .Select(datum => datum.CreateObjective())
                .ToList();
        }

        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Quest)) return;
            var questInstanceData = new QuestInstanceData
            {
                Id = _questDatum.QuestKey,
                Objectives = _objectives.Select(objective => objective.GetSaveData()).ToArray(),
                State = _questState
            };
            
            saveData.questData.quests.Add(questInstanceData);
        }

        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Quest)) return;
            
            var savedQuest = saveData.questData.quests
                .FirstOrDefault(q => q.Id == _questDatum.QuestKey);
            
            if (savedQuest == null)
            {
                _objectives = _questDatum.QuestObjectives
                    .Select(datum => datum.CreateObjective())
                    .ToList();
                return;
            }
            
            _questState = savedQuest.State;
            
            var savedLookup = new Dictionary<string, string>();
            foreach (var savedObj in savedQuest.Objectives)
            {
                var split = savedObj.Split("|");
                var id = split[0];
                var raw = split.Length > 1 ? split[1] : "";
                savedLookup[id] = raw;
            }
            
            _objectives = new List<Objective>();

            foreach (var objectiveDatum in _questDatum.QuestObjectives)
            {
                _objectives.Add(savedLookup.TryGetValue(objectiveDatum.ObjectiveId, out var rawData)
                    ? objectiveDatum.CreateObjective(rawData)
                    : objectiveDatum.CreateObjective());
            }
        }

        
        private void QuestEventBusOnEventFired(GameEvent gameEvent)
        {
            foreach (var objective in _objectives)
            {
                objective.TryProgress(gameEvent);
            }
        }

        private void UpdateCompletedState()
        {
            if (_objectives.Any(objective => !objective.IsCompleted)) return;
            _questState = QuestState.Completed;
        }
    }
}