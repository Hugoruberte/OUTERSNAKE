using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Snakes;

[CustomEditor(typeof(SnakeBodyManager))]
public class SnakeBodyControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SnakeBodyManager script = target as SnakeBodyManager;

		DrawDefaultInspector();

		EditorGUILayout.LabelField($"current body length : {script.snakeBodyData.bodyLength}", EditorStyles.centeredGreyMiniLabel);
	}
}

