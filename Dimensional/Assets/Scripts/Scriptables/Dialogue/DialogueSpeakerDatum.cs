using FMODUnity;
using UnityEngine;
using Utilities;

namespace Scriptables.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSpeakerDatum", menuName = "Scriptable Objects/Dialogue/DialogueSpeakerDatum")]
    public class DialogueSpeakerDatum : ScriptableObject
    {
        [SerializeField] private string speakerKey;
        [SerializeField] private string speakerName;
        [SerializeField] private Sprite speakerIcon;
        
        [SerializeField] private EventReferenceWrapper speakerSound;
        [SerializeField] private int soundFrequency;
        
        public string SpeakerKey => speakerKey;
        public string SpeakerName => speakerName;
        public Sprite SpeakerIcon => speakerIcon;
        
        public EventReference SpeakerSound => speakerSound.eventRef;
        public int SoundFrequency => soundFrequency;
    }
}