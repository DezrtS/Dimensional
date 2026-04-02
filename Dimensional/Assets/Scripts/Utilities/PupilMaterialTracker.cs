using System;
using Systems.Player;
using UnityEngine;

namespace Utilities
{
    public class PupilMaterialTracker : MonoBehaviour
    {
        [Header("Eye")]
        [SerializeField] private Material eyeMaterial;
        [SerializeField] private bool createInstance;

        [SerializeField] private Renderer[] eyeRenderers;
        
        [Header("Target")]
        [SerializeField] private bool trackPlayer;
        [SerializeField] private Transform target;

        [Header("Settings")]
        [SerializeField] private bool invertX;
        [SerializeField] private bool invertY;
        
        [SerializeField] private float strength = 1f;
        [SerializeField] private float lookSpeed = 10f;
        [SerializeField] private bool clampToCircle = true;

        private Material _material;
        private Vector2 _currentLook;
        private Transform _target;

        private static readonly int PupilDirID = Shader.PropertyToID("_Pupil_Direction");

        private void Awake()
        {
            _target = target;
           _material = createInstance ? Instantiate(eyeMaterial) : eyeMaterial;
           foreach (var eyeRenderer in eyeRenderers)
           {
               eyeRenderer.material = _material;
           }
        }

        private void Start()
        {
            if (!trackPlayer) return;
            target = PlayerController.Instance.transform;
            SetTargetToDefault();
        }

        public void SetTarget(Transform eyeTarget) => _target = eyeTarget; 
        public void SetTargetToDefault() => _target = target; 

        private void FixedUpdate()
        {
            if (!_target) return;
            
            Vector3 worldDir = (_target.position - transform.position).normalized;
            
            Vector3 localDir = transform.InverseTransformDirection(worldDir);
            
            Vector2 look = new Vector2(localDir.x, localDir.y);
            if (invertX) look.x = -look.x;
            if (invertY) look.y = -look.y;
            
            if (clampToCircle)
                look = Vector2.ClampMagnitude(look, 1f);
            else
            {
                look.x = Mathf.Clamp(look.x, -1f, 1f);
                look.y = Mathf.Clamp(look.y, -1f, 1f);
            }
            
            look *= strength;
            
            _currentLook = Vector2.Lerp(_currentLook, look, Time.fixedDeltaTime * lookSpeed);
            _material.SetVector(PupilDirID, new Vector4(_currentLook.x, _currentLook.y, 0f, 0f));
        }
    }
}