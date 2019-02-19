using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SaveScript))]
public class SaveScriptEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		SaveScript myScript = (SaveScript)target;
		if(Application.isPlaying)
		{
			if(GUILayout.Button("Reset"))
				myScript.Reset();
			if(GUILayout.Button("Save"))
				myScript.Save();
		}
	}
}
