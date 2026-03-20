using System;
using Systems.Forces;
using UnityEngine;

namespace Utilities
{
    public class HidePlayer : MonoBehaviour
    {
        [SerializeField] private GameObject[] hideObjects;
        private ForceController _forceController;

        private void Awake()
        {
            _forceController = GetComponent<ForceController>();
        }

        public void SetIsHidden(bool isHidden)
        {
            foreach (var obj in hideObjects)
            {
                obj.SetActive(!isHidden);
            }
            _forceController.IsKinematic = isHidden;
        }
    }
}
