using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

[CustomEditor(typeof(LightTarget))]
public class LightTargetInspector : Editor
{
    public LightTarget lightTarget;
    SerializedProperty lightTargetParameters;
    SerializedProperty lightParameters;
    public List<Vector3> lightPivots;

    void OnEnable()
    {
        lightTarget = (LightTarget)serializedObject.targetObject;
        lightTargetParameters = serializedObject.FindProperty("lightTargetParameters");
        lightParameters = serializedObject.FindProperty("lightParameters");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(lightTargetParameters, true);
        EditorGUILayout.PropertyField(lightParameters, true);
        serializedObject.ApplyModifiedProperties();
    }
}
