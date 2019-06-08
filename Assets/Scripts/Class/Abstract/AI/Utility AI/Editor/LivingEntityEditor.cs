using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Utility.AI;

[CustomEditor(typeof(LivingEntity), true), CanEditMultipleObjects]
public class LivingEntityEditor : InteractiveEntityEditor
{
	private string[] guids;
	private string path;
	private string nameAI;

	private bool showAI = false;
	private bool showDetails = false;
	private UtilityAction[] acts;

	// float val = 0f;

	void OnEnable()
	{
		LivingEntity script = target as LivingEntity;

		nameAI = script.GetType().ToString();
		nameAI = nameAI.Replace(" ", string.Empty);
		nameAI += "AI";
	}


	public override void OnInspectorGUI()
	{
		// Rect n = new Rect();
		// n.x = rect.x;
		// n.y += 500;
		// n.width = 200;
		// n.height = 16;
		// val = EditorGUI.FloatField(n, "Val", val);

		LivingEntity script = target as LivingEntity;

		base.OnInspectorGUI();

		if(script.behaviour == null) {
			// guids = AssetDatabase.FindAssets("t:" + nameAI);
			// if(guids.Length > 0) {
			// 	path = AssetDatabase.GUIDToAssetPath(guids[0]);
			// 	script.behaviour = AssetDatabase.LoadAssetAtPath(path, typeof(WhiteRabbitAI)) as WhiteRabbitAI;
			// }
			return;
		}

		rect = EditorGUILayout.GetControlRect(true, 0);
		rect.y += 1;
		rect.width += -120;
		rect.height = 16;

		EditorGUI.LabelField(rect, script.GetType() + " AI", EditorStyles.boldLabel);

		rect.x += 120;
		rect.height = 17;

		GUI.enabled = false;
		EditorGUI.ObjectField(rect, MonoScript.FromScriptableObject(script.behaviour), typeof(UtilityAIBehaviour), false);
		GUI.enabled = true;

		GUILayout.Space(19);

		rect.x += -120;
		rect.y += -0.5f;
		showAI = EditorGUI.Foldout(rect, showAI, "");

		if(showAI)
		{
			if(EditorApplication.isPlaying) {
				Repaint();

				this.acts = script.behaviour.GetCurrentActions(script);
				foreach(UtilityAction a in this.acts) {
					if(a == null) {
						continue;
					}
					EditorGUILayout.LabelField($"{a.method} -> score: {a.score}", EditorStyles.centeredGreyMiniLabel);
				}
			} else {
				EditorGUILayout.LabelField($"No action running", EditorStyles.centeredGreyMiniLabel);
			}

			EditorGUI.indentLevel ++;
			showDetails = EditorGUILayout.Foldout(showDetails, "Details");
			if(showDetails) {
				foreach(UtilityAction act in script.behaviour.actions) {
					if(act.discarded_max) {
						EditorGUILayout.LabelField($"{act.method} discarded (> max: {act.Max()})");
					} else if(act.discarded_active) {
						EditorGUILayout.LabelField($"{act.method} discarded (not active)");
					} else {
						EditorGUILayout.LabelField($"{act.method}'s score: {act.score}");
					}
				}
			}
			EditorGUI.indentLevel --;
			
			GUILayout.Space(5);
		}

		serializedObject.ApplyModifiedProperties();
	}
}

