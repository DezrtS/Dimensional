using FMOD.Studio;
using Managers;
using Scriptables.Actions;
using Systems.Actions;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Systems.Audio_Effects
{
    public class ActionAudioEffect
    {
        private readonly ActionAudioEvent _actionAudioEvent;
        private readonly GameObject _gameObject;
        private EventInstance _eventInstance;
        
        public ActionAudioEffect(ActionAudioEvent actionAudioEvent, GameObject gameObject)
        {
            _actionAudioEvent = actionAudioEvent;
            _gameObject = gameObject;

            if (_actionAudioEvent.CreateInstance) _eventInstance = AudioManager.CreateEventInstance(_actionAudioEvent.EventReference);
        }

        public void Play(ActionEventType actionEventType)
        {
            if (_actionAudioEvent.ActivationEventType != actionEventType) return;

            if (_actionAudioEvent.CreateInstance)
            {
                _eventInstance.start();
                if (_actionAudioEvent.AttachToGameObject) AudioManager.AttachInstanceToGameObject(_eventInstance, _gameObject);
            }
            else if (_actionAudioEvent.AttachToGameObject) AudioManager.PlayOneShot(_actionAudioEvent.EventReference, _gameObject);
            else AudioManager.PlayOneShot(_actionAudioEvent.EventReference);
        }

        public void Stop()
        {
            if (_actionAudioEvent.CreateInstance) _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }

        public void Destroy()
        {
            if (_actionAudioEvent.CreateInstance) AudioManager.RemoveEventInstance(_eventInstance);
        }
    }
}
