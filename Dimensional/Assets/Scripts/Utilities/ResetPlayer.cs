using System;
using Systems.Movement;
using UnityEngine;

namespace Utilities
{
    public class ResetPlayer : MonoBehaviour
    {
        [SerializeField] private Vector3 resetPosition;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<ForceController>().Teleport(resetPosition);
            }
        }
    }
}
