using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Rabbit), true), CanEditMultipleObjects]
public class RabbitEditor : CellableEntityEditor
{
	private Vector2 tempo;
	private float ws, jh;
	private int id;
	private bool showParameter = false;
	private bool showAI = false;
	private Rect r;
	private UtilityAction act;

	// private float val = 0f;

	public override void OnInspectorGUI()
	{
		Rabbit script = target as Rabbit;

		base.OnInspectorGUI();

		/*Rect n = new Rect();
		n.x = 10;
		n.y = 500;
		n.width = 350;
		n.height = 18;
		val = EditorGUI.FloatField(n, "Val", val);*/

		showParameter = EditorGUILayout.Foldout(showParameter, "");
		r = EditorGUILayout.GetControlRect(true, 0);
		r.y += -18;
		r.width = 150;
		r.height = 50;
		EditorGUI.LabelField(r, "Rabbit parameters", EditorStyles.boldLabel);

		if(showParameter) {
			script.speed = EditorGUILayout.Slider("Walk Speed", script.speed, 0.1f, 20f);
			script.jumpHeight = EditorGUILayout.Slider("Jump Height", script.jumpHeight, 0.1f, 5f);
			script.maxStepDistance = EditorGUILayout.IntSlider("Max Step Distance", script.maxStepDistance, 1, 6);
			script.rangeOfView = EditorGUILayout.Slider("Range Of View", script.rangeOfView, 1f, 20f);

			tempo.Set(script.minAfterJumpTempo, script.maxAfterJumpTempo);
			tempo = EditorGUILayout.Vector2Field("After Jump Tempo Interval", tempo);
			script.minAfterJumpTempo = Mathf.Min(Mathf.Max(tempo.x, 0f), tempo.y);
			script.maxAfterJumpTempo = Mathf.Max(tempo.x, Mathf.Min(tempo.y, 10f));

			EditorGUILayout.Space();
		}

		r = EditorGUILayout.GetControlRect(true, 0);
		r.x += 120;
		r.y += 1;
		r.width += -120;
		r.height = 16;

		GUI.enabled = false;
		EditorGUI.ObjectField(r, script.behaviour, typeof(UtilityBehaviourAI), false);
		// EditorGUI.ObjectField(r, MonoScript.FromMonoBehaviour(script.behaviour), typeof(UtilityBehaviourAI), false);
		GUI.enabled = true;

		r.x += -120;
		
		EditorGUI.LabelField(r, "Rabbit AI", EditorStyles.boldLabel);

		if(EditorApplication.isPlaying) {
			showAI = EditorGUILayout.Foldout(showAI, "");

			if(showAI) {
				EditorGUI.indentLevel ++;
				act = script.behaviour.GetCurrentAction(script);
				EditorGUILayout.LabelField($"Current action : {act.method} (score = {act.score})", EditorStyles.miniLabel);
				EditorGUI.indentLevel --;
			}
		} else {
			GUILayout.Space(19);
		}

		serializedObject.ApplyModifiedProperties();
	}
}

