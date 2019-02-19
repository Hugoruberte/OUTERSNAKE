using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WhiteRabbit), true), CanEditMultipleObjects]
public class WhiteRabbitEditor : RabbitEditor
{
	public override void OnInspectorGUI()
	{
		WhiteRabbit script = target as WhiteRabbit;

		base.OnInspectorGUI();

		if(script.behaviour == null) {
			script.behaviour = FindObjectOfType<WhiteRabbitAI>();
		}

		serializedObject.ApplyModifiedProperties();
	}
}

