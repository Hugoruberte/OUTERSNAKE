using UnityEngine;
using UnityEditor;
using Tools;
using System.Collections.Generic;
using Lazers;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LazerData))]
public class LazerDataEditor : Editor
{
	private Vector2 temp;

	public override void OnInspectorGUI()
	{
		LazerData script = target as LazerData;

		DrawDefaultInspector();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Mode", EditorStyles.boldLabel);
		script.mode = (LazerData.LazerMode)EditorGUILayout.EnumPopup("Lazer Mode", script.mode);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
		script.speed = EditorGUILayout.Slider("Speed", script.speed, 0.1f, 100f);
		script.lifetime = EditorGUILayout.Slider("Lifetime", script.lifetime, 0.1f, 100f);
		script.flatten = EditorGUILayout.Toggle("Flatten", script.flatten);
		script.bounce = EditorGUILayout.Toggle("Bounce", script.bounce);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Layer Mask", EditorStyles.boldLabel);
		script.hitLayerMask = EditorGUILayoutExtension.ConcatenatedMaskField("Hit Layer Mask", script.hitLayerMask);
		if(script.bounce) {
			script.bounceLayerMask = EditorGUILayoutExtension.ConcatenatedMaskField("Bounce Layer Mask", script.bounceLayerMask, script.hitLayerMask.MaskToNames());
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Width", EditorStyles.boldLabel);
		script.width = EditorGUILayout.Slider("Width", script.width, 0.01f, 10f);
		script.widthSpeed = EditorGUILayout.Slider("Width Decrease Speed", script.widthSpeed, 0.01f, 10f);

		if(script.mode == LazerData.LazerMode.Shot) {
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Length", EditorStyles.boldLabel);
			script.length = EditorGUILayout.Slider("Length", script.length, 0.1f, 20f);
		} else {
			script.widthPointSpeed = EditorGUILayout.Slider("Width Point Decrease Speed", script.widthPointSpeed, 0.01f, 10f);

			temp = script.distancePerPointMinMax;
			temp = EditorGUILayout.Vector2Field("Distance Per Point Interval", temp);
			script.distancePerPointMinMax.Set(Mathf.RoundToInt(Mathf.Min(Mathf.Max(temp.x, 0f), temp.y)), Mathf.RoundToInt(Mathf.Max(temp.x, Mathf.Min(temp.y, 10f))));
		}

		if(script.bounce) {
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Bounce Parameters", EditorStyles.boldLabel);
			script.maxBounceCount = EditorGUILayout.IntSlider("Max Bounce Count", script.maxBounceCount, 1, 200);
		
			script.lastBounceMode = (LastBounceMode)EditorGUILayout.EnumPopup("Last Bounce Mode", script.lastBounceMode);

			EditorGUI.indentLevel ++;
			script.coneAngle = EditorGUILayout.Slider("Cone Angle", script.coneAngle, 0f, 45f);

			if(script.lastBounceMode == LastBounceMode.Curve || script.lastBounceMode == LastBounceMode.Random) {
				script.gravityForceMultiplier = EditorGUILayout.Slider("Gravity Force Multiplier", script.gravityForceMultiplier, 0f, 10f);
				script.forwardForceMultiplier = EditorGUILayout.Slider("Forward Force Multiplier", script.forwardForceMultiplier, 0f, 2f);

				script.forceDampling = EditorGUILayout.Slider("Force Dampling", script.forceDampling, 0f, 1f);
			}

			EditorGUI.indentLevel --;
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Miscellaneous", EditorStyles.boldLabel);
		script.autoAim = EditorGUILayout.Toggle("Easy Aim", script.autoAim);


		if(!EditorApplication.isPlaying && GUI.changed)
		{
			EditorUtility.SetDirty(script);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}
	}
}