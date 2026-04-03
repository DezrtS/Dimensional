using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Utilities
{
    [ExecuteAlways]
    public class SplineObjectSpawner : MonoBehaviour
    {
        public SplineContainer spline;

        [Header("Spawn Settings")]
        public GameObject prefab;
        public float spacing = 1f;
        public float heightOffset = 0.5f;

        [Header("Ground Detection")]
        public LayerMask groundMask = -1;
        public float maxDistance = 100f;

        [Header("Options")]
        public bool matchGroundNormal = true;

        [HideInInspector]
        public List<GameObject> spawnedObjects = new();

        const float EPSILON = 0.0001f;

        public void ClearObjects()
        {
            for (int i = spawnedObjects.Count - 1; i >= 0; i--)
            {
                if (spawnedObjects[i] != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(spawnedObjects[i]);
#else
                Destroy(spawnedObjects[i]);
#endif
                }
            }

            spawnedObjects.Clear();
        }

        public void Spawn()
        {
            if (spline == null || prefab == null)
                return;

            ClearObjects();

            Spline s = spline.Spline;
            float length = s.GetLength();

            int count = Mathf.CeilToInt(length / spacing);

            for (int i = 0; i <= count; i++)
            {
                if (i == count && spline.Spline.Closed) break;
                float t = Mathf.Clamp01((float)i / count);

                Vector3 pos = spline.EvaluatePosition(t);
                Vector3 tangent = spline.EvaluateTangent(t);
                tangent = tangent.normalized;

                Ray ray = new Ray(pos + Vector3.up * maxDistance * 0.5f, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, groundMask, QueryTriggerInteraction.Ignore))
                {
                    pos = hit.point + hit.normal * heightOffset;

                    Quaternion rot;

                    if (matchGroundNormal)
                    {
                        rot = Quaternion.LookRotation(tangent, hit.normal);
                    }
                    else
                    {
                        rot = Quaternion.LookRotation(tangent, Vector3.up);
                    }

                    GameObject obj = Instantiate(prefab, pos, rot, transform);
                    spawnedObjects.Add(obj);
                }
            }
        }

        public void SnapSplineToGround()
        {
            if (spline == null)
                return;

            var s = spline.Spline;

            for (int i = 0; i < s.Count; i++)
            {
                var knot = s[i];

                Vector3 pos = spline.transform.TransformPoint(knot.Position);

                Ray ray = new Ray(pos + Vector3.up * maxDistance * 0.5f, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, groundMask, QueryTriggerInteraction.Ignore))
                {
                    pos = hit.point + hit.normal * heightOffset;
                    knot.Position = spline.transform.InverseTransformPoint(pos);
                    s[i] = knot;
                }
            }
        }
    }
}