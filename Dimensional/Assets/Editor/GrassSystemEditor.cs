using Systems.Grass;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editor
{
    [CustomEditor(typeof(GrassSystem))]
    public class GrassSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var grassSystem = (GrassSystem)target;
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            if (GUILayout.Button("Refresh Grass Settings"))
            {
                //grassSystem.UpdateChunks();
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}