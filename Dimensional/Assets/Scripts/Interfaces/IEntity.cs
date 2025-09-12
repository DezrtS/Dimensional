using Scriptables.Entities;
using UnityEngine;

namespace Interfaces
{
    public interface IEntity
    {
        public EntityDatum EntityDatum { get; }
        public GameObject GameObject { get; }
        public uint Id { get; }
    }
}
