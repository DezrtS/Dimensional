using System;
using Systems.Events;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "TeleportEventDatum", menuName = "Scriptable Objects/Events/TeleportEventDatum")]
    public class TeleportEventDatum : EventDatum
    {
        public static event Action<int> Teleported;

        [SerializeField] private int id;
        
        public override void HandleEvent()
        {
            Teleported?.Invoke(id);
        }
    }
}
