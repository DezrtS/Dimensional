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
        [SerializeField] private float fadeDuration = 1f;

        private static List<EventInstance> eventInstances;
        
        private EventInstance _musicInstance;
        private EventReference _nextMusicReference;
        private bool _musicIsChanging;
        private bool _isMusicMuted;

        private void Awake()
        {
            eventInstances = new List<EventInstance>();
            if (!startMusicOnAwake) return;
            PlayMusic(defaultMusic);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.M)) return;
            _isMusicMuted = !_isMusicMuted;
            _musicInstance.setVolume(_isMusicMuted ? 0f : 1f);
        }

        public void ChangeMusic(EventReference newMusic)
        {
            if (!_nextMusicReference.IsNull && _nextMusicReference.Guid == newMusic.Guid) return;
            
            _nextMusicReference = newMusic;
            if (!_musicIsChanging)
            {
                StartCoroutine(ChangeMusicRoutine());
            }
        }

        private IEnumerator ChangeMusicRoutine()
        {
            _musicIsChanging = true;

            // Store current instance
            EventInstance oldInstance = _musicInstance;

            // Create and start new music at 0 volume
            EventInstance newInstance = CreateEventInstance(_nextMusicReference);
            newInstance.start();
            newInstance.setVolume(0f);

            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / fadeDuration;

                float oldVolume = Mathf.Lerp(1f, 0f, t);
                float newVolume = Mathf.Lerp(0f, 1f, t);

                oldInstance.setVolume(_isMusicMuted ? 0f : oldVolume);
                newInstance.setVolume(_isMusicMuted ? 0f : newVolume);

                yield return null;
            }

            // Ensure final values
            newInstance.setVolume(_isMusicMuted ? 0f : 1f);
            oldInstance.setVolume(0f);

            // Stop and release old music
            oldInstance.stop(STOP_MODE.IMMEDIATE);
            oldInstance.release();

            // Swap reference
            _musicInstance = newInstance;

            _musicIsChanging = false;
        }

        public void StopMusic()
        {
            if (_musicInstance.isValid())
            {
                StartCoroutine(FadeOutAndStop(_musicInstance));
            }
        }
        
        private IEnumerator FadeOutAndStop(EventInstance instance)
        {
            float time = 0f;

            instance.getVolume(out float startVolume);

            while (time < fadeDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / fadeDuration;

                float volume = Mathf.Lerp(startVolume, 0f, t);
                instance.setVolume(volume);

                yield return null;
            }

            instance.setVolume(0f);
            instance.stop(STOP_MODE.IMMEDIATE);
            instance.release();
        }

        private void PlayMusic(EventReference musicReference)
        {
            _musicInstance = CreateEventInstance(musicReference);
            _musicInstance.start();
            _musicInstance.setVolume(_isMusicMuted ? 0f : 1f);
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