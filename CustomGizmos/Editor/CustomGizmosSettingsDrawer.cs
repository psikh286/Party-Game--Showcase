using Party.CustomGizmos;
using UnityEditor;
using UnityEngine;

namespace Party._Project.Scripts.CustomGizmos.Editor
{
    [CustomPropertyDrawer(typeof(CustomGizmosSettings<float>))]
    public class CustomGizmosSettingsDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // Draw foldout for the main settings
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded, "Settings", true);


            if (property.isExpanded)
            {
                // Indent the content when unfolded
                EditorGUI.indentLevel++;

                // Draw the individual properties
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("DrawGizmos"));
                EditorGUI.PropertyField(new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("DrawSolid"));
                EditorGUI.PropertyField(new Rect(position.x, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("Size"));
                EditorGUI.PropertyField(new Rect(position.x, position.y + 4 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("Color"));

                // Reset the indent level
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}
