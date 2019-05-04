using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tools;

[CustomEditor(typeof(HeartManager))]
public class HeartManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		HeartManager script = target as HeartManager;

		DrawDefaultInspector();

		EditorGUILayout.LabelField("Heart", EditorStyles.boldLabel);
		GUI.enabled = false;
		EditorGUILayout.Vector3Field("Rotation", script.heart.rotation.eulerAngles);
		GUI.enabled = true;

		Repaint();
	}

	protected virtual void OnSceneGUI()
	{
		HeartManager script = target as HeartManager;

		Vector3 pos = script.transform.position + Vector3Extension.ONE * 15f;
		float size = HandleUtility.GetHandleSize(pos);
		GUIStyle style = new GUIStyle("Label");
		style.fontStyle = FontStyle.Bold;
		Handles.Label(pos, "Heart", style); 
		this.DrawArrow(script, pos, size);
	}

	private void DrawArrow(HeartManager script, Vector3 pos, float size)
	{
		Handles.color = Handles.xAxisColor;
		Handles.ArrowHandleCap(
			0,
			pos,
			Quaternion.LookRotation(script.heart.right),
			size,
			EventType.Repaint
		);

		Handles.color = Handles.yAxisColor;
		Handles.ArrowHandleCap(
			0,
			pos,
			Quaternion.LookRotation(script.heart.up),
			size,
			EventType.Repaint
		);

		Handles.color = Handles.zAxisColor;
		Handles.ArrowHandleCap(
			0,
			pos,
			Quaternion.LookRotation(script.heart.forward),
			size,
			EventType.Repaint
		);
	}
}
