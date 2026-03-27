using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables.Cutscenes;
using Scriptables.Quests;
using Systems.Cutscenes;
using Systems.Objectives;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Managers
{
    public class QuestManager : Singleton<QuestManager>
    {
        [SerializeField] private QuestDatum[] questData;
        
        [SerializeField] private CutsceneDatum cutsceneDatum;
        [SerializeField] private Cutscene cutscene;
        
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private Material locationMaterial;
        [SerializeField] private float materialChangeDuration;
        
        private ObjectiveLocation _objectiveLocation;
        
        private void Awake()
        {
            foreach (var questDatum in questData)
            {
                questDatum.AttachQuest(gameObject);
            }
        }

        public void SetObjectiveTarget(GameObject objectiveTarget)
        {
            cinemachineCamera.LookAt = objectiveTarget.transform;
            CutsceneManager.Instance.PlayCutscene(cutscene, cutsceneDatum);
        }

        public void StartMaterialChange()
        {
            // Fade IN (0 → 1)
            StopAllCoroutines();
            StartCoroutine(MaterialChangeRoutine(0f, 1f));
        }

        public void StopMaterialChange()
        {
            // Fade OUT (1 → 0)
            StopAllCoroutines();
            StartCoroutine(MaterialChangeRoutine(1f, 0f));
        }

        private IEnumerator MaterialChangeRoutine(float start, float end)
        {
            float elapsed = 0f;

            // Cache the property ID for performance
            int transparencyID = Shader.PropertyToID("_Transparency");

            // Ensure material starts at the correct value
            locationMaterial.SetFloat(transparencyID, start);

            while (elapsed < materialChangeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / materialChangeDuration;

                float value = Mathf.Lerp(start, end, t);
                locationMaterial.SetFloat(transparencyID, value);

                yield return null;
            }

            // Snap to final value
            locationMaterial.SetFloat(transparencyID, end);
        }

    }
}