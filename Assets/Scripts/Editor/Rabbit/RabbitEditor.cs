using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(RabbitEntity), true), CanEditMultipleObjects]
public class RabbitEditor : LivingEntityEditor
{
	private Vector2 tempo;
	private float ws, jh;
	private int id;
	private bool showParameter = false;

	// private float val = 0f;

	public override void OnInspectorGUI()
	{
		RabbitEntity script = target as RabbitEntity;

		base.OnInspectorGUI();

		/*Rect n = new Rect();
		n.x = 10;
		n.y = 500;
		n.width = 350;
		n.height = 18;
		val = EditorGUI.FloatField(n, "Val", val);*/

		showParameter = EditorGUILayout.Foldout(showParameter, "");
		rect = EditorGUILayout.GetControlRect(true, 0);
		rect.y += -18;
		rect.width = 200;
		rect.height = 50;
		EditorGUI.LabelField(rect, script.name + " parameters", EditorStyles.boldLabel);

		if(showParameter) {
			script.speed = EditorGUILayout.Slider("Walk Speed", script.speed, 0.1f, 20f);
			script.jumpHeight = EditorGUILayout.Slider("Jump Height", script.jumpHeight, 0.1f, 5f);
			script.maxStepDistance = EditorGUILayout.IntSlider("Max Step Distance", script.maxStepDistance, 1, 6);
			script.rangeOfView = EditorGUILayout.Slider("Range Of View", script.rangeOfView, 1f, 20f);

			tempo.Set(script.minAfterJumpTempo, script.maxAfterJumpTempo);
			tempo = EditorGUILayout.Vector2Field("After Jump Tempo Interval", tempo);
			script.minAfterJumpTempo = Mathf.Min(Mathf.Max(tempo.x, 0f), tempo.y);
			script.maxAfterJumpTempo = Mathf.Max(tempo.x, Mathf.Min(tempo.y, 10f));

			serializedObject.Update();

			EditorGUILayout.Space();
		}

		if(!EditorApplication.isPlaying && GUI.changed)
		{
			EditorUtility.SetDirty(script);
			EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
		}

		// GUILayout.Space(19);

		serializedObject.ApplyModifiedProperties();
	}
}

