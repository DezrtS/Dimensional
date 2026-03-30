using System;
using Scriptables.Audio;
using UnityEngine;

namespace Interfaces
{
    public interface ISpawnPoint
    {
        public event Action<ISpawnPoint> Entered;
        
        public string Id { get; }
        public Vector3 Position { get; }
        public SpawnPointAudioDatum SpawnPointAudioDatum { get; }
        public bool IsDefaultSpawnPoint { get; }

        public void SpawnAt();
    }
}
