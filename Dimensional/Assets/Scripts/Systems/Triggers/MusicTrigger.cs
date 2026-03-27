using System;
using FMODUnity;
using Managers;
using Scriptables.Audio;
using UnityEngine;

namespace Systems.Triggers
{
    public class MusicTrigger : MonoBehaviour
    {
        [SerializeField] private SpawnPointAudioDatum audioDatum;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.Instance.ChangeMusic(audioDatum.GetMusicReference());
            }
        }
    }
}
