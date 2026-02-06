using Managers;
using Systems.Events;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "GameStateEventDatum", menuName = "Scriptable Objects/Events/GameStateEventDatum")]
    public class GameStateEventDatum : GameEventDatum
    {
        [SerializeField] private GameStateEvent gameStateEvent;
        public override GameEvent GameEvent => gameStateEvent;
    }
}
