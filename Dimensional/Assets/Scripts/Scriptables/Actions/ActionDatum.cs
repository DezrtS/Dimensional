using UnityEngine;

namespace Scriptables.Actions
{
    public enum Type
    {
        None,
        // Movement
        JumpAction,
        DoubleJumpAction,
        DashAction,
        DiveAction,
        AirAction,
        RollAction,
        LeftSpecialAction,
        RightSpecialAction,
    }
    
    public abstract class ActionDatum : ScriptableObject
    {
        [SerializeField] private Type actionType;
        
        public Type ActionType => actionType;
    }
}