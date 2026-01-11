using System;
using Scriptables.Quests;

namespace Systems.Quests
{
    [Serializable]
    public class QuestInstanceData
    {
        public string Id;
        public string[] Objectives;
        public QuestState State;
    }
}
