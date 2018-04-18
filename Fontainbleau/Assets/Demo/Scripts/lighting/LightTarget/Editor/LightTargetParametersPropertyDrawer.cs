using UnityEditor;
using UnityEngine;
using LightingTools;

[CustomPropertyDrawer(typeof(LightTargetParameters))]
public class LightTargetParametersPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("name"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("drawGizmo"));
        EditorLightingUtilities.DrawSplitter();

        #region Rig
        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawHeader("Rig");
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Yaw"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Pitch"));
        if (property.serializedObject.FindProperty("lightCookie") != null && property.serializedObject.FindProperty("lightCookie").objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Roll"));
        }
        EditorGUILayout.PropertyField(property.FindPropertyRelative("distance"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("offset"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("linkToCameraRotation"));
        #endregion

        EditorGUI.EndProperty();
    }
}