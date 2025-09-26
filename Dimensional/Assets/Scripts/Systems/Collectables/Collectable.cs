using System;
using Systems.Player;
using UnityEngine;

namespace Systems.Collectables
{
    public class Collectable : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            other.GetComponent<Inventory>().AddCollectables(1);
            Destroy(gameObject);
        }
    }
}