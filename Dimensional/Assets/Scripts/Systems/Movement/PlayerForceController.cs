using System.Collections;
using Managers;
using Systems.Forces;
using UnityEngine;

namespace Systems.Movement
{
    public class PlayerForceController : ComplexRigidbodyForceController
    {
        protected override void OnTeleport(Vector3 position)
        {
            CameraManager.Instance.DisableDamping();
            base.OnTeleport(position);
            StartCoroutine(DisableDamping());
        }

        private static IEnumerator DisableDamping()
        {
            yield return new WaitForSeconds(0.5f);
            CameraManager.Instance.EnableDamping();
        }
    }
}
