using System;
using UnityEngine;

namespace Systems.Platforms
{
    public class Platform : MonoBehaviour
    {
        public Vector3 Velocity { get; private set; }
        private Vector3 _previousPosition;

        private void Awake()
        {
            _previousPosition = transform.position;
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            var position = transform.position;
            Velocity = (position - _previousPosition) / deltaTime;
            _previousPosition = position;
        }
    }
}
