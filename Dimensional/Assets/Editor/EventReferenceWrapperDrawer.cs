using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editor
{
    [CustomPropertyDrawer(typeof(EventReferenceWrapper))]
    public class EventReferenceWrapperDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin property
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, label);

            // Find the nested FMOD field
            var eventRefProp = property.FindPropertyRelative("eventRef");

            // Draw FMOD’s EventReference using Unity’s default drawer
            EditorGUI.PropertyField(position, eventRefProp, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // FMOD’s drawer has its own height, so we forward it
            var eventRefProp = property.FindPropertyRelative("eventRef");
            return EditorGUI.GetPropertyHeight(eventRefProp);
        }
    }
}