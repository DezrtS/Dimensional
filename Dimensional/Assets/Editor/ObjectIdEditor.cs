using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editor
{
    [CustomEditor(typeof(ObjectId))]
    [CanEditMultipleObjects]
    public class ObjectIdEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            if (GUILayout.Button("Randomize Id"))
            {
                foreach (var t in targets)
                {
                    var objectId = (ObjectId)t;
                    objectId.RandomizeId();

                    // Mark as dirty so Unity saves the change
                    EditorUtility.SetDirty(objectId);
                }
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}