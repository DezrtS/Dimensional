using System.Collections;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CoroutineManager : Singleton<CoroutineManager>
    {
        public new Coroutine StartCoroutine(IEnumerator routine)
        {
            return base.StartCoroutine(routine);
        }

        public new void StopCoroutine(IEnumerator routine)
        {
            if (routine == null) return;
            base.StopCoroutine(routine);
        }
    }
}
