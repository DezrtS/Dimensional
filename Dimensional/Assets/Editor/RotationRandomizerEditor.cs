using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editor
{
    [CustomEditor(typeof(RotationRandomizer))]
    [CanEditMultipleObjects]
    public class RotationRandomizerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            GUILayout.Label("Randomize Rotation", EditorStyles.boldLabel);

            if (GUILayout.Button("Randomize X Axis"))
            {
                ApplyToTargets(r => r.RandomizeX());
            }

            if (GUILayout.Button("Randomize Y Axis"))
            {
                ApplyToTargets(r => r.RandomizeY());
            }

            if (GUILayout.Button("Randomize Z Axis"))
            {
                ApplyToTargets(r => r.RandomizeZ());
            }
        }

        private void ApplyToTargets(System.Action<RotationRandomizer> action)
        {
            foreach (var t in targets)
            {
                RotationRandomizer randomizer = (RotationRandomizer)t;

                if (randomizer == null)
                    continue;

                // Record undo for ALL children too
                Undo.RegisterFullObjectHierarchyUndo(randomizer.gameObject, "Randomize Rotation");

                action.Invoke(randomizer);

                EditorUtility.SetDirty(randomizer);
            }
        }
    }
}