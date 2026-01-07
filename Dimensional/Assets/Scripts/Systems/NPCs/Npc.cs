using System;
using Managers;
using Scriptables.User_Interface;
using UnityEngine;

namespace Systems.NPCs
{
    public class Npc : MonoBehaviour
    {
        [SerializeField] private Transform elementTransform;
        [SerializeField] private WorldUIAnchorDatum worldUIAnchorDatum;

        private void Start()
        {
            UIManager.Instance.SpawnWorldUIAnchor(worldUIAnchorDatum, gameObject, elementTransform);
        }
    }
}