using System;
using Managers;
using UnityEngine;

namespace Systems.Triggers
{
    public class AreaTitleTrigger : MonoBehaviour
    {
        [SerializeField] private string areaName;
        [SerializeField] private float duration;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            UIManager.Instance.AreaTitle.ShowArea(areaName, duration);
        }
    }
}
