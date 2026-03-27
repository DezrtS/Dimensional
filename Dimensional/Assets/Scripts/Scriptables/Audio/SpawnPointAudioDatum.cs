using System;
using System.Linq;
using FMODUnity;
using Scriptables.Save;
using UnityEngine;
using Utilities;

namespace Scriptables.Audio
{
    [Serializable]
    public class SpawnPointAudio
    {
        public EventReferenceWrapper musicReference;
        public BoolVariableInstance[] boolVariableInstances;

        public bool IsActive()
        {
            return boolVariableInstances.Length == 0 || boolVariableInstances.All(boolVariableInstance => boolVariableInstance.IsEnabled());
        }
    }
    
    [CreateAssetMenu(fileName = "SpawnPointAudioDatum", menuName = "Scriptable Objects/SpawnPointAudioDatum")]
    public class SpawnPointAudioDatum : ScriptableObject
    {
        [SerializeField] private EventReferenceWrapper defaultMusicReference;
        [SerializeField] private SpawnPointAudio[] spawnPointMusics;

        public EventReference GetMusicReference()
        {
            foreach (var spawnPointMusic in spawnPointMusics)
            {
                if (spawnPointMusic.IsActive()) return spawnPointMusic.musicReference.eventRef;
            }
            
            return defaultMusicReference.eventRef;
        }
    }
}
