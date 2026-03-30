using System;
using Managers;
using Scriptables.Objectives;
using Scriptables.Save;
using UnityEngine;

namespace Systems.Objectives
{
    public class ObjectiveLocation : MonoBehaviour
    {
        [SerializeField] private BoolVariableInstance[] boolVariableInstances;
        [SerializeField] private ObjectiveDatum objectiveDatum;

        [SerializeField] private GameObject locationMarker;
        
        private void OnEnable()
        {
            User_Interface.Objectives.ObjectiveLocated += ObjectivesOnObjectiveLocated;
        }

        private void OnDisable()
        {
            User_Interface.Objectives.ObjectiveLocated -= ObjectivesOnObjectiveLocated;
        }

        private void ObjectivesOnObjectiveLocated(ObjectiveDatum locatedObjectiveDatum)
        {
            var isLocating = locatedObjectiveDatum == objectiveDatum;
            locationMarker.SetActive(isLocating);
            if (isLocating) QuestManager.Instance.SetObjectiveTarget(gameObject);
        }
    }
}
