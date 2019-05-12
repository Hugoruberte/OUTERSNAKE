using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using My.Tools;

[CustomEditor(typeof(RabbitData), true), CanEditMultipleObjects]
public class RabbitDataEditor : Editor
{
	private readonly float[] AFTER_JUMP_MAGNITUDE = new float[2] {0f, 10f};

	public override void OnInspectorGUI()
	{
		RabbitData script = target as RabbitData;

		base.OnInspectorGUI();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
		script.speed = EditorGUILayout.Slider("Walk Speed", script.speed, 0.1f, 20f);
		script.maxStepDistance = EditorGUILayout.IntSlider("Max Step Distance", script.maxStepDistance, 1, 6);
		script.rangeOfView = EditorGUILayout.Slider("Range Of View", script.rangeOfView, 0f, 20f);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Jump", EditorStyles.boldLabel);
		script.jumpHeight = EditorGUILayout.Slider("Jump Height", script.jumpHeight, 0.1f, 5f);
		script.afterJumpTempoInterval = EditorGUILayoutExtension.IntervalField("After Jump Delay", script.afterJumpTempoInterval, AFTER_JUMP_MAGNITUDE);
		
		EditorUtilityExtension.SetDirtyOnGUIChange(script);

		serializedObject.ApplyModifiedProperties();
	}
}

