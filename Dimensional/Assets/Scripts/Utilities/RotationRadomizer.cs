using UnityEngine;

namespace Utilities
{
    [ExecuteAlways]
    public class RotationRandomizer : MonoBehaviour
    {
        [Header("Settings")]
        public bool useLocalRotation = true;

        [Header("Rotation Range")]
        public Vector2 xRange = new Vector2(0f, 360f);
        public Vector2 yRange = new Vector2(0f, 360f);
        public Vector2 zRange = new Vector2(0f, 360f);

        public void RandomizeX()
        {
            ApplyRotation(Vector3.right, xRange);
        }

        public void RandomizeY()
        {
            ApplyRotation(Vector3.up, yRange);
        }

        public void RandomizeZ()
        {
            ApplyRotation(Vector3.forward, zRange);
        }

        private void ApplyRotation(Vector3 axis, Vector2 range)
        {
            float angle = Random.Range(range.x, range.y);

            if (useLocalRotation)
            {
                // Rotate in local space
                transform.localRotation = Quaternion.AngleAxis(angle, axis) * transform.localRotation;
            }
            else
            {
                // Rotate in world space
                transform.rotation = Quaternion.AngleAxis(angle, axis) * transform.rotation;
            }
        }
    }
}