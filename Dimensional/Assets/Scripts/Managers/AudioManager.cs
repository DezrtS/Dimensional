using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Utilities;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private EventReference defaultMusic;
        [SerializeField] private bool startMusicOnAwake;

        private static List<EventInstance> _eventInstances;
        private static EventInstance _musicInstance;

        private void Awake()
        {
            _eventInstances = new List<EventInstance>();
            if (!startMusicOnAwake) return;
            ChangeMusic(defaultMusic);
        }

        public static void ChangeMusic(EventReference newMusic)
        {
            if (_eventInstances.Contains(_musicInstance))
            {
                _eventInstances.Remove(_musicInstance);
                _musicInstance.stop(STOP_MODE.ALLOWFADEOUT);
                _musicInstance.release();
            }
            
            _musicInstance = CreateEventInstance(newMusic);
            _musicInstance.start();
        }

        public static void StopMusic(STOP_MODE stopMode) => _musicInstance.stop(stopMode);

        public static EventInstance CreateEventInstance(EventReference eventReference)
        {
            var eventInstance = RuntimeManager.CreateInstance(eventReference);
            _eventInstances.Add(eventInstance);
            return eventInstance;
        }

        public static void AttachInstanceToGameObject(EventInstance eventInstance, GameObject attachTo)
        {
            RuntimeManager.AttachInstanceToGameObject(eventInstance, attachTo);
        }

        public static void RemoveEventInstance(EventInstance eventInstance)
        {
            _eventInstances.Remove(eventInstance);
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
            foreach (var eventInstance in _eventInstances)
            {
                eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
                eventInstance.release();
            }
            
            _eventInstances.Clear();
        }

        private void OnDestroy()
        {
            CleanUp();
        }
    }
}