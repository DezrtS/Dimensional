using System;
using FMODUnity;
using Managers;
using UnityEngine;

namespace Systems.Triggers
{
    public class MusicTrigger : MonoBehaviour
    {
        [SerializeField] private EventReference musicReference;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.Instance.ChangeMusic(musicReference);
            }
        }
    }
}
