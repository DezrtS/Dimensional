using System;
using Managers;
using UnityEngine;

namespace Systems.Visual_Effects
{
    public class RainEffect : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0, 35, 0);
        private Transform _targetTransform;

        private void Start()
        {
            _targetTransform = CameraManager.Instance.Camera.transform;
        }

        private void FixedUpdate()
        {
            transform.position = _targetTransform.position + offset;
        }
    }
}
