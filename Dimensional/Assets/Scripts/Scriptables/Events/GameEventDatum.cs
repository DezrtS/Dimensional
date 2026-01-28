using Systems.Events;
using UnityEngine;

namespace Scriptables.Events
{
    public abstract class GameEventDatum : ScriptableObject
    {
        public abstract GameEvent GameEvent { get; }
    }
}
