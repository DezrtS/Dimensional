using UnityEngine;

namespace Interfaces
{
    public interface IAim
    {
        public Vector3 GetInput();

        public static Vector3 GetInput(GameObject target)
        {
            return !target.TryGetComponent(out IAim aim) ? Vector3.zero : aim.GetInput();
        }
    }
}
