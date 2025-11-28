using System;
using Managers;
using Scriptables.Cutscenes;
using UnityEngine;
using UnityEngine.Playables;

namespace Systems.Cutscenes
{
    public class Cutscene : MonoBehaviour
    {
        public event Action<Cutscene> Played;
        public event Action<Cutscene> Paused;
        public event Action<Cutscene> Stopped;
        
        private CutsceneDatum _cutsceneDatum;
        private PlayableDirector _playableDirector;

        private void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
            _playableDirector.played += DirectorOnPlayed;
            _playableDirector.paused += DirectorOnPaused;
            _playableDirector.stopped += DirectorOnStopped;
        }

        public void Initialize(CutsceneDatum cutsceneDatum)
        {
            _cutsceneDatum = cutsceneDatum;
        }

        private void DirectorOnPlayed(PlayableDirector playableDirector)
        {
            Played?.Invoke(this);
        }

        private void DirectorOnPaused(PlayableDirector playableDirector)
        {
            Paused?.Invoke(this);
        }

        private void DirectorOnStopped(PlayableDirector playableDirector)
        {
            Stopped?.Invoke(this);
            EventManager.SendEvents(_cutsceneDatum.OnStopEventData);
            if (_cutsceneDatum.DestroyOnStopped) Destroy(gameObject);
        }

        public void Play()
        {
            _playableDirector.Play();
            EventManager.SendEvents(_cutsceneDatum.OnPlayEventData);
        }

        public void Pause()
        {
            _playableDirector.Pause();
        }

        public void Stop()
        {
            _playableDirector.Stop();
        }
    }
}
