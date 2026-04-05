using FMODUnity;
using Managers;
using UnityEngine;

namespace Utilities
{
    public class ChangeMusic : MonoBehaviour
    {
        [SerializeField] private EventReference eventReference;
        
        public void StopMusic() => AudioManager.Instance.StopMusic();
        public void ChangeMusicTo() => AudioManager.Instance.ChangeMusic(eventReference);
    }
}
