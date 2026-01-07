using Systems.Actions;
using UnityEngine;

namespace Scriptables.Actions.Modifiers
{
    public abstract class ActionContextModifierDatum : ScriptableObject
    {
        public abstract ActionContext Modify(ActionContext actionContext);
    }
}
