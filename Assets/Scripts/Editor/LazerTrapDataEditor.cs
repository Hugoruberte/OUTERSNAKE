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
		EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
		script.omega = EditorGUILayout.Slider("Omega", script.omega, 0.1f, 100f);
		script.targetLayerMask = EditorGUILayoutExtension.MappedMaskField("Target Layer Mask", script.targetLayerMask, this.users);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Wander", EditorStyles.boldLabel);
		this.MinMaxInterval(script);
		script.wanderMaxPauseQuarter = EditorGUILayout.IntSlider("Maximum number of pause quarter", script.wanderMaxPauseQuarter, 1, 5);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Shoot", EditorStyles.boldLabel);
		script.shootDelay = EditorGUILayout.Slider("After shoot delay", script.shootDelay, 0f, 5f);
	}

	private void MinMaxInterval(LazerTrapData script)
	{
		this.temp.Set(script.wanderRotationDurationInterval[0], script.wanderRotationDurationInterval[1]);
		this.temp = EditorGUILayout.Vector2Field("Rotation duration interval", this.temp);
		script.wanderRotationDurationInterval[0] = Mathf.Min(this.temp[0], this.temp[1]);
		script.wanderRotationDurationInterval[1] = Mathf.Max(this.temp[0], this.temp[1]);
	}
}