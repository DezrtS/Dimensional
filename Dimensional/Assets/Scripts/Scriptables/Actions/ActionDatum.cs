using Interfaces;
using Systems.Actions;
using UnityEngine;

namespace Scriptables.Actions
{
    
    public abstract class ActionDatum : ScriptableObject
    {
        [SerializeField] private float activationTime;
        public float ActivationTime => activationTime;

        public abstract Action AttachAction(GameObject actionHolder);
    }
}