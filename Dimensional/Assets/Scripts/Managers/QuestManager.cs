using System;
using System.Collections.Generic;
using Scriptables.Quests;
using Systems.Quests;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class QuestManager : Singleton<QuestManager>
    {
        [SerializeField] private QuestDatum[] questData;

        private void Awake()
        {
            foreach (var questDatum in questData)
            {
                questDatum.AttachQuest(gameObject);
            }
        }
    }
}