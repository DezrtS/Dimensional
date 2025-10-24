using UnityEngine;

namespace Scriptables.Player
{
    public enum ResetResponseType
    {
        None,
        Kill,
    }
    
    [CreateAssetMenu(fileName = "ResetPlayerDatum", menuName = "Scriptable Objects/Player/ResetPlayerDatum")]
    public class ResetPlayerDatum : ScriptableObject
    {
        [SerializeField] private ResetResponseType resetResponseType;
        [SerializeField] private bool useTransition;
        [SerializeField] private bool followPlayer;
        [SerializeField] private bool lookAtPlayer;

        [SerializeField] private float resetDelay;
        [SerializeField] private float transitionDuration;
        
        public ResetResponseType ResetResponseType => resetResponseType;
        public bool UseTransition => useTransition;
        public bool FollowPlayer => followPlayer;
        public bool LookAtPlayer => lookAtPlayer;
        
        public float ResetDelay => resetDelay;
        public float TransitionDuration => transitionDuration;
    }
}
