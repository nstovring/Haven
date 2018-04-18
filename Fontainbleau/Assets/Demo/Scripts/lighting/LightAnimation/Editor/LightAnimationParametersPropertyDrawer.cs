using UnityEditor;
using UnityEngine;
using LightingTools;

// IngredientDrawer
[CustomPropertyDrawer (typeof (LightAnimationParameters))]
public class LightAnimationParametersPropertyDrawer : PropertyDrawer {
	
	// Draw the property inside the given rect
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);
		
		EditorGUILayout.PropertyField(property.FindPropertyRelative ("animationMode"));
		if (property.FindPropertyRelative("animationMode").enumValueIndex == 0)
		{
			EditorGUILayout.PropertyField(property.FindPropertyRelative ("animationCurve"));	
		}
		if (property.FindPropertyRelative("animationMode").enumValueIndex == 1)
		{
			EditorGUILayout.PropertyField(property.FindPropertyRelative ("noiseParameters"));	
		}
		if (property.FindPropertyRelative("animationMode").enumValueIndex == 2)
		{
			EditorGUILayout.PropertyField(property.FindPropertyRelative ("animationClip"));	
		}
		EditorGUILayout.PropertyField(property.FindPropertyRelative ("animationLength"));
		
		
		EditorGUI.EndProperty ();
	
	}

}