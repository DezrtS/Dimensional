using System.Collections.Generic;
using System.Linq;
using Managers;
using Scriptables.Objectives;
using Systems.Objectives;
using Systems.Quests;
using UnityEngine;

namespace Scriptables.Quests
{
    public enum QuestType
    {
        Primary,
        Optional,
        Background,
    }
    
    public enum QuestState
    {
        Hidden,
        Active,
        Completed,
    }
    
    [CreateAssetMenu(fileName = "QuestDatum", menuName = "Scriptable Objects/Quests/QuestDatum")]
    public class QuestDatum : ScriptableObject
    {
        [SerializeField] private string questKey;
        [SerializeField] private QuestType questType;
        [SerializeField] private RegionType questRegionType;
        [SerializeField] private QuestState defaultQuestState;
        [Space]
        [SerializeField] private string questName;
        [TextArea(3, 8)] [SerializeField] private string questDescription;
        [Space]
        [SerializeField] private ObjectiveDatum[] questObjectives;
        
        public string QuestKey => questKey;
        public QuestType QuestType => questType;
        public RegionType QuestRegionType => questRegionType;
        public QuestState DefaultQuestState => defaultQuestState;
        
        public string QuestName => questName;
        public string QuestDescription => questDescription;
        
        public ObjectiveDatum[] QuestObjectives => questObjectives;

        public Quest AttachQuest(GameObject questHolder)
        {
            var quest = questHolder.AddComponent<Quest>();
            quest.Initialize(this);
            return quest;
        }
    }
}
