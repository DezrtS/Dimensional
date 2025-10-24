using Scriptables.Player;
using Systems.Player;
using UnityEngine;

namespace Utilities
{
    public class ResetPlayer : MonoBehaviour
    {
        [SerializeField] private Vector3 defaultResetPosition;
        [SerializeField] private ResetPlayerDatum resetPlayerDatum;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            PlayerController.Instance.ResetPlayer(resetPlayerDatum, defaultResetPosition);
        }
    }
}
