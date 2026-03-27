using System;
using Managers;
using UnityEngine;

namespace Utilities
{
    public class PupilMaterialMouseTracker : MonoBehaviour
    {
        [Header("Eye")]
        [SerializeField] private Material eyeMaterial;
        [SerializeField] private bool createInstance;
        [SerializeField] private Renderer[] eyeRenderers;

        [Header("Camera")]
        [SerializeField] private Camera cam;

        [Header("Settings")]
        [SerializeField] private bool invertX;
        [SerializeField] private bool invertY;

        [SerializeField] private float strength = 1f;
        [SerializeField] private float lookSpeed = 10f;
        [SerializeField] private bool clampToCircle = true;

        [Tooltip("Distance from camera for mouse projection (important!)")]
        [SerializeField] private float projectionDistance = 10f;

        private Material _material;
        private Vector2 _currentLook;

        private static readonly int PupilDirID = Shader.PropertyToID("_Pupil_Direction");

        private void Awake()
        {
            _material = createInstance ? Instantiate(eyeMaterial) : eyeMaterial;

            foreach (var eyeRenderer in eyeRenderers)
            {
                eyeRenderer.material = _material;
            }
        }

        private void LateUpdate()
        {
            // 1. Convert mouse → world point
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = projectionDistance; // distance from camera

            Vector3 worldTarget = cam.ScreenToWorldPoint(mousePos);

            // 2. Direction from eye → mouse world point
            Vector3 worldDir = (worldTarget - transform.position).normalized;

            // 3. Convert to local space
            Vector3 localDir = transform.InverseTransformDirection(worldDir);

            // 4. Extract XY
            Vector2 look = new Vector2(localDir.x, localDir.y);

            if (invertX) look.x = -look.x;
            if (invertY) look.y = -look.y;

            // 5. Clamp
            if (clampToCircle)
                look = Vector2.ClampMagnitude(look, 1f);
            else
            {
                look.x = Mathf.Clamp(look.x, -1f, 1f);
                look.y = Mathf.Clamp(look.y, -1f, 1f);
            }

            // 6. Apply strength
            look *= strength;

            // 7. Smooth
            _currentLook = Vector2.Lerp(_currentLook, look, Time.deltaTime * lookSpeed);

            // 8. Send to shader
            _material.SetVector(PupilDirID, new Vector4(_currentLook.x, _currentLook.y, 0f, 0f));
        }
    }
}