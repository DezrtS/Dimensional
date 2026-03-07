using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Utilities;
using Object = System.Object;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private EventReference defaultMusic;
        [SerializeField] private bool startMusicOnAwake;
        [Space]
        [SerializeField] private float musicChangeDelay;

        private static List<EventInstance> eventInstances;
        
        private EventInstance _musicInstance;
        private EventReference _nextMusicReference;
        private bool _musicIsChanging;

        private void Awake()
        {
            eventInstances = new List<EventInstance>();
            if (!startMusicOnAwake) return;
            PlayMusic(defaultMusic);
        }

        public void ChangeMusic(EventReference newMusic)
        {
            if (_nextMusicReference.Guid == newMusic.Guid) return;
            
            _nextMusicReference = newMusic;
            if (!_musicIsChanging)
            {
                StartCoroutine(ChangeMusicRoutine());
            }
        }

        private IEnumerator ChangeMusicRoutine()
        {
            _musicIsChanging = true;
            StopMusic(STOP_MODE.ALLOWFADEOUT);
            yield return new WaitForSeconds(musicChangeDelay);
            while (true)
            {
                _musicInstance.getPlaybackState(out var state);
                if (state != PLAYBACK_STATE.STOPPING) break;
                yield return null;
            }
            PlayMusic(_nextMusicReference);
            _musicIsChanging = false;
        }

        public void StopMusic(STOP_MODE stopMode) => _musicInstance.stop(stopMode);

        private void PlayMusic(EventReference musicReference)
        {
            _musicInstance = CreateEventInstance(musicReference);
            _musicInstance.start();
        }

        public static EventInstance CreateEventInstance(EventReference eventReference)
        {
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstances.Add(eventInstance);
            return eventInstance;
        }

        public static void AttachInstanceToGameObject(EventInstance eventInstance, GameObject attachTo, bool nonRigidbodyVelocity = false)
        {
            RuntimeManager.AttachInstanceToGameObject(eventInstance, attachTo, nonRigidbodyVelocity);
        }

        public static void RemoveEventInstance(EventInstance eventInstance)
        {
            eventInstances.Remove(eventInstance);
            RuntimeManager.DetachInstanceFromGameObject(eventInstance);
        }

        public static void PlayOneShot(EventReference eventReference)
        {
            if (eventReference.IsNull) return;
            RuntimeManager.PlayOneShot(eventReference);
        }
        
        public static void PlayOneShot(EventReference eventReference, Vector3 worldPosition)
        {
            if (eventReference.IsNull) return;
            RuntimeManager.PlayOneShot(eventReference, worldPosition);
        }
        
        public static void PlayOneShot(EventReference eventReference, GameObject attachTo)
        {
            if (eventReference.IsNull) return;
            RuntimeManager.PlayOneShotAttached(eventReference, attachTo);
        }

        public void CleanUp()
        {
            foreach (var eventInstance in eventInstances)
            {
                eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                eventInstance.release();
            }
            
            eventInstances.Clear();
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}