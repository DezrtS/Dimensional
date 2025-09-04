using UnityEngine;
using Utilities;

namespace Managers
{
    public class EntityManager : Singleton<EntityManager>
    {
        private static uint _nextEntityId = 0;

        public static uint GetNextEntityId()
        {
            _nextEntityId++;
            return _nextEntityId;
        }
    }
}
