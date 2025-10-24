using System;
using System.Collections;
using Managers;
using UnityEngine;

namespace Systems.Movement.Components
{
    public enum PowerLevel
    {
        Low,
        Medium,
        High,
    }
    
    public class GroundPoundMovementComponent : MovementComponent
    {
        public event Action GroundPounded;
        
        private Coroutine _groundPoundCoroutine;
        
        private float _groundPoundSpeed;
        private float _groundPoundTime;
        private AnimationCurve _groundPoundCurve;

        private float _groundPoundMediumPowerTimeThreshold;
        private float _groundPoundHighPowerTimeThreshold;
        
        public PowerLevel GroundPoundPowerLevel { get; private set; }

        public void SetGroundPoundMovementData(float groundPoundSpeed, float groundPoundTime, AnimationCurve groundPoundCurve, float groundPoundMediumPowerTimeThreshold, float groundPoundHighPowerTimeThreshold)
        {
            _groundPoundSpeed = groundPoundSpeed;
            _groundPoundTime = groundPoundTime;
            _groundPoundCurve = groundPoundCurve;
            
            _groundPoundMediumPowerTimeThreshold = groundPoundMediumPowerTimeThreshold;
            _groundPoundHighPowerTimeThreshold = groundPoundHighPowerTimeThreshold;
        }
        
        protected override void OnActivate()
        {
            MovementController.Grounded += MovementControllerOnGrounded;
            MovementController.ForceController.UseGravity = false;
            _groundPoundCoroutine = StartCoroutine(GroundPoundCoroutine());
        }

        protected override void OnDeactivate()
        {
            if (_groundPoundCoroutine != null) StopCoroutine(_groundPoundCoroutine);
            MovementController.ForceController.UseGravity = true;
            MovementController.Grounded -= MovementControllerOnGrounded;
        }
        
        private void MovementControllerOnGrounded(bool isGrounded)
        {
            Deactivate();
            GroundPounded?.Invoke();
        }
        
        private IEnumerator GroundPoundCoroutine()
        {
            GroundPoundPowerLevel = PowerLevel.Low;
            var groundPoundTimer = 0f;

            while (true)
            {
                var normalizedTime = Mathf.Clamp01(groundPoundTimer / _groundPoundTime);
                var verticalVelocity = _groundPoundCurve.Evaluate(normalizedTime) * _groundPoundSpeed;
                MovementController.ForceController.SetVelocity(Vector3.up * verticalVelocity);
                groundPoundTimer += Time.deltaTime;
                switch (GroundPoundPowerLevel)
                {
                    case PowerLevel.Low:
                        if (groundPoundTimer >= _groundPoundMediumPowerTimeThreshold) GroundPoundPowerLevel = PowerLevel.Medium;
                        break;
                    case PowerLevel.Medium:
                        if (groundPoundTimer >= _groundPoundHighPowerTimeThreshold) GroundPoundPowerLevel = PowerLevel.High;
                        break;
                    case PowerLevel.High:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                yield return null;
            }
        }
    }
}
