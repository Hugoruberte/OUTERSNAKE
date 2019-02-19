using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TeleporterGizmo))]
public class TeleporterGizmoEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TeleporterGizmo myScript = (TeleporterGizmo)target;
		if(GUILayout.Button("Search"))
		{
			myScript.Search();
		}
	}
}
