using System;
using System.Collections;
using Managers;
using Systems.Movement;
using Systems.Player;
using UnityEngine;

namespace Utilities
{
    public class ResetPlayer : MonoBehaviour
    {
        [SerializeField] private Vector3 resetPosition;
        [SerializeField] private float delay;
        [SerializeField] private float transitionDuration;
        
        private bool _resetting;

        private void OnTriggerEnter(Collider other)
        {
            if (_resetting || !other.gameObject.CompareTag("Player")) return;
            StartCoroutine(ResetRoutine(other.transform));
        }

        private IEnumerator ResetRoutine(Transform player)
        {
            _resetting = true;
            var cameraManager = CameraManager.Instance;
            var playerController = player.GetComponent<PlayerController>();
            cameraManager.SetFollow(null);
            cameraManager.SetLookAt(player);
            yield return new WaitForSeconds(delay);
            UIManager.Instance.Transition(true, transitionDuration);
            yield return new WaitForSeconds(transitionDuration);
            UIManager.Instance.Transition(false);
            cameraManager.SetFollow(playerController.PlayerLook.Root);
            cameraManager.SetLookAt(null);
            player.GetComponent<ForceController>().Teleport(resetPosition);
            _resetting = false;
        }
    }
}
