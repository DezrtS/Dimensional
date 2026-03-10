using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GroundPlacer : MonoBehaviour
{
    public float radius = 0.5f;
    public float offset = 0f;
    public LayerMask groundMask;

    private const float maxDistance = 1000f;

#if UNITY_EDITOR

    public void PlaceOnGround()
    {
        Vector3 origin = transform.position + Vector3.up * maxDistance;
        origin.y += offset;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDistance * 2f, groundMask))
        {
            Vector3 pos = hit.point;
            pos.y += offset;

            Undo.RecordObject(transform, "Place On Ground");
            transform.position = pos;
        }
    }

    public void PlaceWithNormalRotation()
    {
        Vector3[] sampleOffsets =
        {
            Vector3.zero,
            Vector3.forward * radius,
            Vector3.back * radius,
            Vector3.left * radius,
            Vector3.right * radius
        };

        Vector3 normalSum = Vector3.zero;
        Vector3 centerHitPoint = transform.position;

        int hitCount = 0;

        foreach (var sample in sampleOffsets)
        {
            Vector3 worldSample = transform.position + sample;
            worldSample.y += offset;

            Vector3 origin = worldSample + Vector3.up * maxDistance;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxDistance * 2f, groundMask))
            {
                normalSum += hit.normal;

                if (sample == Vector3.zero)
                    centerHitPoint = hit.point;

                hitCount++;
            }
        }

        if (hitCount == 0)
            return;

        Vector3 avgNormal = (normalSum / hitCount).normalized;

        Undo.RecordObject(transform, "Place With Ground Normal");

        transform.position = centerHitPoint + avgNormal * offset;

        Quaternion alignRotation = Quaternion.FromToRotation(transform.up, avgNormal);
        transform.rotation = alignRotation * transform.rotation;
    }

#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(GroundPlacer))]
public class GroundPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GroundPlacer placer = (GroundPlacer)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Place On Ground"))
        {
            placer.PlaceOnGround();
        }

        if (GUILayout.Button("Place With Normal Rotation"))
        {
            placer.PlaceWithNormalRotation();
        }
    }
}
#endif