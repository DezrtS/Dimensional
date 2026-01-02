using System;
using UnityEngine;

namespace Utilities
{
    public class ObjectId : MonoBehaviour
    {
        [SerializeField] private string id;

        public string Id => id;

        private void Reset()
        {
            RandomizeId();
        }

        public void RandomizeId()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
