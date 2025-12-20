using Managers;
using UnityEngine;

namespace Scriptables.Events
{
    [CreateAssetMenu(fileName = "GameStateEventDatum", menuName = "Scriptable Objects/Events/GameStateEventDatum")]
    public class GameStateEventDatum : EventDatum
    {
        [SerializeField] private GameState gameState;    
    
        public override void HandleEvent()
        {
            GameManager.Instance.ChangeGameState(gameState);
        }
    }
}
