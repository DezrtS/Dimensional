using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editor
{
    [CustomEditor(typeof(SplineObjectSpawner))]
    public class SplineObjectSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SplineObjectSpawner spawner = (SplineObjectSpawner)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Spawn Objects"))
            {
                spawner.Spawn();
            }

            if (GUILayout.Button("Clear Spawned Objects"))
            {
                spawner.ClearObjects();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Snap Spline To Ground"))
            {
                spawner.SnapSplineToGround();
            }
        }
    }
}