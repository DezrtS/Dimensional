using System;
using UnityEngine;

namespace Interfaces
{
    public interface ISpawnPoint
    {
        public event Action<ISpawnPoint> Entered;
        
        public string Id { get; }
        public Vector3 Position { get; }
        public bool IsDefaultSpawnPoint { get; }

        public void SpawnAt();
    }
}
