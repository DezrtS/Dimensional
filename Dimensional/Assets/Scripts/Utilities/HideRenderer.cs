using System;
using UnityEngine;

namespace Utilities
{
    public class HideRenderer : MonoBehaviour
    {
        [SerializeField] private bool hideOnAwake = true;

        private void Awake()
        {
            if (!hideOnAwake) return;
            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
        }
    }
}
