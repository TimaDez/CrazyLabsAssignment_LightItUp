using System;
using LightItUp;
using UnityEngine;
using Object = System.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class StringAsPath : PropertyAttribute {

	public StringAsPath() {
		
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(StringAsPath))]
public class StringAsPathDrawer : PropertyDrawer
{
	private GameObject obj;
	
	// Draw the property inside the given rect
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		
		EditorGUI.PropertyField(position, property, label, true);
		
		/*var att = attribute as StringAsPath;
		
		GameObject myMonoA = null;
		obj = (GameObject)EditorGUILayout.ObjectField ("Gun Object", obj, typeof(GameObject), false);

		return;*/
		
		/*
		
		string path = AssetDatabase.GetAssetPath(obj);
		if (path.Length != 0)
		{
			property.stringValue = path;
		}			*/	
	}
}
#endif