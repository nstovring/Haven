using UnityEditor;
using UnityEngine;
using LightingTools;

[CustomPropertyDrawer(typeof(LightParameters))]
public class LightParametersPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorLightingUtilities.DrawSplitter();

        #region General
        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawHeader("General");
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("intensity"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("useColorTemperature"));
        
        if (property.FindPropertyRelative("useColorTemperature").boolValue == true)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("colorTemperature"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("colorFilter"));
        }
        if (property.FindPropertyRelative("useColorTemperature").boolValue == false)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("colorFilter"), new GUIContent("Color"));
        }
        EditorGUILayout.PropertyField(property.FindPropertyRelative("mode"), new GUIContent("Light mode"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("indirectIntensity"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("range"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("lightCookie"));
        #endregion

        EditorLightingUtilities.DrawSplitter();

        #region LightShape
        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawHeader("Light shape");
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("lightShape"));
        if (property.FindPropertyRelative("lightShape").enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightAngle"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("innerAngle"));
        }
        if (property.FindPropertyRelative("lightShape").enumValueIndex != 3 && property.FindPropertyRelative("lightShape").enumValueIndex != 7 && property.FindPropertyRelative("lightShape").enumValueIndex != 5)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightRadius"));
        }
        if (property.FindPropertyRelative("lightShape").enumValueIndex == 3 || property.FindPropertyRelative("lightShape").enumValueIndex == 7)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightWidth"));
        }
        if (property.FindPropertyRelative("lightShape").enumValueIndex == 3 || property.FindPropertyRelative("lightShape").enumValueIndex == 7 || property.FindPropertyRelative("lightShape").enumValueIndex == 5)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightLength"));
        }
        #endregion

        EditorLightingUtilities.DrawSplitter();

        #region Shadows
        // Shadows
        EditorGUI.indentLevel = 0;
        property.FindPropertyRelative("shadows").boolValue = EditorLightingUtilities.DrawHeader("Shadows", property.FindPropertyRelative("shadows").boolValue);
        EditorGUI.indentLevel = 1;
        if (property.FindPropertyRelative("shadows").boolValue == true)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowsType"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowQuality"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ShadowNearClip"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowBias"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowNormalBias"));  
        }
        #endregion

        #region ShadowCascades
        if (property.FindPropertyRelative("lightShape").enumValueIndex == 2 && property.FindPropertyRelative("shadows").boolValue == true)
        {
            EditorLightingUtilities.DrawSplitter();
            EditorGUI.indentLevel = 0;
            EditorLightingUtilities.DrawHeader("Shadow cascades");
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(property.FindPropertyRelative("cascadeparams.count"));
            if (property.FindPropertyRelative("cascadeparams.count").intValue > 1)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("cascadeparams.split0"));
            }
            if (property.FindPropertyRelative("cascadeparams.count").intValue > 2)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("cascadeparams.split1"));
            }
            if (property.FindPropertyRelative("cascadeparams.count").intValue > 3)
            { 
                EditorGUILayout.PropertyField(property.FindPropertyRelative("cascadeparams.split2"));
            }
        }
        #endregion

        EditorLightingUtilities.DrawSplitter();

        #region Advanced
        // Advanced
        EditorGUI.indentLevel = 0;
        property.FindPropertyRelative("advancedEnabled").boolValue = EditorLightingUtilities.DrawHeaderFoldout("Additional settings", property.FindPropertyRelative("advancedEnabled").boolValue);
        EditorGUI.indentLevel = 1;
        if (property.FindPropertyRelative("advancedEnabled").boolValue)
        {
            EditorGUILayout.LabelField(new GUIContent("General"), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("fadeDistance"));
            if (property.FindPropertyRelative("mode").enumValueIndex != 0)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("affectDiffuse"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("affectSpecular"));
            }
            EditorGUILayout.LabelField(new GUIContent("Shadows"), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowFadeDistance"));
        }

        #endregion

        EditorLightingUtilities.DrawSplitter();

        EditorGUI.EndProperty();

    }
}