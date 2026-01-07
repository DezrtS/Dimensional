using Systems.Actions;
using UnityEngine;

namespace Scriptables.Actions.Modifiers
{
    [CreateAssetMenu(
        fileName = "AimingActionContextModifierDatum",
        menuName = "Scriptable Objects/Actions/Modifiers/AimingActionContextModifierDatum")]
    public class AimingActionContextModifierDatum : ActionContextModifierDatum
    {
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private float height = 2f;

        public override ActionContext Modify(ActionContext actionContext)
        {
            var startPos = actionContext.SourceGameObject.transform.position;
            var endPos = actionContext.TargetPosition;

            Vector3 launchVelocity = ComputeBallisticVelocity(startPos, endPos, height);

            actionContext.TargetDirection = launchVelocity.normalized;
            actionContext.ProjectileSpeed = launchVelocity.magnitude; // If you store speed separately

            return actionContext;
        }
        
        private Vector3 ComputeBallisticVelocity(Vector3 start, Vector3 end, float apexHeight)
        {
            // --- Vertical motion ---
            float startY = start.y;
            float endY = end.y;

            float apexY = Mathf.Max(startY, endY) + apexHeight;

            // Upward velocity needed to reach the apex
            float vy_up = Mathf.Sqrt(2f * gravity * (apexY - startY));

            // Time to reach apex
            float t_up = vy_up / gravity;

            // Downward velocity at apex needed to land on target
            float vy_down = Mathf.Sqrt(2f * gravity * (apexY - endY));

            // Time to fall from apex to target
            float t_down = vy_down / gravity;

            float totalTime = t_up + t_down;

            // --- Horizontal motion ---
            Vector3 horizontalDisplacement = new Vector3(
                end.x - start.x,
                0f,
                end.z - start.z
            );

            Vector3 horizontalVelocity = horizontalDisplacement / totalTime;

            // Combine horizontal + vertical
            Vector3 velocity = horizontalVelocity + Vector3.up * vy_up;

            return velocity;
        }
    }
}
