using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Snakes;

[CustomEditor(typeof(SnakePartCharacter))]
public class SnakePartCharacterEditor : CellableEntityEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		SnakePartCharacter script = target as SnakePartCharacter;

		EditorGUILayout.LabelField("Snake Part Info");
		EditorGUI.indentLevel++;

		GUI.enabled = false;
		EditorGUILayout.EnumPopup("State", script.snakePartState);
		GUI.enabled = true;

		EditorGUI.indentLevel--;

		serializedObject.ApplyModifiedProperties();
	}
}

