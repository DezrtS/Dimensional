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
            if (newValue != GameState.Preparing || _objectives == null) return;
            _objectives = new List<Objective>();
            foreach (var questObjective in _questDatum.QuestObjectives)
            {
                var objective = questObjective.CreateObjective();
                objective.CompletionStateChanged += ObjectiveOnCompletionStateChanged;
                _objectives.Add(objective);
            }
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
            
            _objectives = new List<Objective>();
            var savedQuest = saveData.questData.quests
                .FirstOrDefault(q => q.Id == _questDatum.QuestKey);
            
            if (savedQuest == null)
            {
                foreach (var questObjective in _questDatum.QuestObjectives)
                {
                    var objective = questObjective.CreateObjective();
                    objective.CompletionStateChanged += ObjectiveOnCompletionStateChanged;
                    _objectives.Add(objective);
                }
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
            
            foreach (var questObjective in _questDatum.QuestObjectives)
            {
                var objective = savedLookup.TryGetValue(questObjective.ObjectiveId, out var rawData)
                    ? questObjective.CreateObjective(rawData)
                    : questObjective.CreateObjective();
                objective.CompletionStateChanged += ObjectiveOnCompletionStateChanged;
                _objectives.Add(objective);
            }
        }
        
        private void ObjectiveOnCompletionStateChanged(Objective objective, bool isCompleted)
        {
            UpdateCompletedState();
        }

        
        private void QuestEventBusOnEventFired(GameEvent gameEvent)
        {
            if (gameEvent is StartQuestEvent startQuestEvent && startQuestEvent.QuestId == _questDatum.QuestKey)
            {
                if (_questState == QuestState.Completed && !startQuestEvent.ForceStart) return;
                _questState = QuestState.Active;
                return;
            }
            
            if (_objectives == null) return;
            foreach (var objective in _objectives)
            {
                objective.TryProgress(gameEvent);
            }
        }

        private void UpdateCompletedState()
        {
            switch (_questState)
            {
                case QuestState.Hidden:
                    break;
                case QuestState.Active:
                    if (_objectives.Any(objective => !objective.IsCompleted)) return;
                    _questState = QuestState.Completed;
                    Debug.Log($"Quest [{_questDatum.QuestKey}] Completed");
                    break;
                case QuestState.Completed:
                    if (_objectives.All(objective => objective.IsCompleted)) return;
                    _questState = QuestState.Active;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}