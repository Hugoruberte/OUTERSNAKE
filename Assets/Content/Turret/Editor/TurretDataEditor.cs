using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using My.Tools;


[CustomEditor(typeof(TurretData))]
public class TurretDataEditor : Editor
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
		TurretData script = target as TurretData;

		DrawDefaultInspector();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);
		script.lazerPrefab = EditorGUILayout.ObjectField("Lazer Prefab", script.lazerPrefab, typeof(GameObject), false) as GameObject;
		script.loadPrefab = EditorGUILayout.ObjectField("Load Prefab", script.loadPrefab, typeof(GameObject), false) as GameObject;
		script.shootPrefab = EditorGUILayout.ObjectField("Shoot Prefab", script.shootPrefab, typeof(GameObject), false) as GameObject;
		script.haloPrefab = EditorGUILayout.ObjectField("Halo Prefab", script.haloPrefab, typeof(GameObject), false) as GameObject;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
		script.rangeOfView = EditorGUILayout.Slider("Range Of View", script.rangeOfView, 0.1f, 100f);
		script.targetLayerMask = EditorGUILayoutExtension.MappedMaskField("Target Layer Mask", script.targetLayerMask, this.users);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Aim", EditorStyles.boldLabel);
		script.aimOmega = EditorGUILayout.Slider("Aim Omega", script.aimOmega, 0.1f, 500f);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Wander", EditorStyles.boldLabel);
		script.wanderOmega = EditorGUILayout.Slider("Wander Omega", script.wanderOmega, 0.1f, 500f);
		script.wanderRotationDurationInterval = EditorGUILayoutExtension.IntervalField("Rotation duration interval", script.wanderRotationDurationInterval, new float[2]{0f, 10f});
		script.wanderMaxPauseQuarter = EditorGUILayout.IntSlider("Maximum number of pause quarter", script.wanderMaxPauseQuarter, 1, 5);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Shoot", EditorStyles.boldLabel);
		script.loadDuration = EditorGUILayout.Slider("Load Duration", script.loadDuration, 0f, 5f);
		script.shootDelay = EditorGUILayout.Slider("After shoot delay", script.shootDelay, 0f, 5f);

		EditorUtilityExtension.SetDirtyOnGUIChange(script);
	}
}