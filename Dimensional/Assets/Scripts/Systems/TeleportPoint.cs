using System;
using Scriptables.Events;
using Systems.Player;
using UnityEngine;

namespace Systems
{
    public class TeleportPoint : MonoBehaviour
    {
        [SerializeField] private int id;

        private void OnEnable()
        {
            TeleportEventDatum.Teleported += Teleport;
        }

        private void OnDisable()
        {
            TeleportEventDatum.Teleported -= Teleport;
        }

        private void Teleport(int teleportId)
        {
            if (id != teleportId) return;
            PlayerController.Instance.PlayerMovementController.ForceController.Teleport(transform.position);
        }
    }
}
