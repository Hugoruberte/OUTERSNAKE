using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Tools;


[CustomEditor(typeof(LazerTrapData))]
public class LazerTrapDataEditor : Editor
{
	private Vector2 temp = Vector2.zero;
	private string[] users;
	private readonly string[] plus = new string[] {"Default"};

	void OnEnable()
	{
		this.users = this.plus.Concat(LayerMaskExtension.UserLayerMask()).ToArray();
	}

	public override void OnInspectorGUI()
	{
		LazerTrapData script = target as LazerTrapData;

		DrawDefaultInspector();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);
		script.lazerPrefab = EditorGUILayout.ObjectField("Lazer Prefab", script.lazerPrefab, typeof(GameObject), false) as GameObject;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
		script.omega = EditorGUILayout.Slider("Omega", script.omega, 0.1f, 100f);
		script.rangeOfView = EditorGUILayout.Slider("Range Of View", script.rangeOfView, 0.1f, 100f);
		script.targetLayerMask = EditorGUILayoutExtension.MappedMaskField("Target Layer Mask", script.targetLayerMask, this.users);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Wander", EditorStyles.boldLabel);
		script.wanderRotationDurationInterval = EditorGUILayoutExtension.IntervalField("Rotation duration interval", script.wanderRotationDurationInterval, new float[2]{0f, 10f});
		script.wanderMaxPauseQuarter = EditorGUILayout.IntSlider("Maximum number of pause quarter", script.wanderMaxPauseQuarter, 1, 5);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Shoot", EditorStyles.boldLabel);
		script.shootDelay = EditorGUILayout.Slider("After shoot delay", script.shootDelay, 0f, 5f);
	}
}