using System;
using System.Collections.Generic;
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
        
        public static EventInstance CreateEventInstance(EventReference eventReference, GameObject attachTo)
        {
            var eventInstance = CreateEventInstance(eventReference);
            RuntimeManager.AttachInstanceToGameObject(eventInstance, attachTo);
            return eventInstance;
        }

        public static void RemoveEventInstance(EventInstance eventInstance)
        {
            _eventInstances.Remove(eventInstance);
            RuntimeManager.DetachInstanceFromGameObject(eventInstance);
        }

        public static void PlayOneShop(EventReference eventReference)
        {
            if (eventReference.IsNull) return;
            RuntimeManager.PlayOneShot(eventReference);
        }
        
        public static void PlayOneShop(EventReference eventReference, Vector3 worldPosition)
        {
            if (eventReference.IsNull) return;
            RuntimeManager.PlayOneShot(eventReference, worldPosition);
        }
        
        public static void PlayOneShop(EventReference eventReference, GameObject attachTo)
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