using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Snakes;

[CustomEditor(typeof(SnakeBodyController))]
public class SnakeBodyControllerEditor : Editor
{
	private int editorBodyLength;

	public override void OnInspectorGUI()
	{
		SnakeBodyController script = target as SnakeBodyController;

		DrawDefaultInspector();

		if(EditorApplication.isPlaying) {
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Snake Body Modifier", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			editorBodyLength = EditorGUILayout.IntSlider("Change Body Length", editorBodyLength, SnakeBodyController.SNAKE_MINIMAL_LENGTH, 100);
			if(GUILayout.Button("Apply")){
				script.bodyLength = editorBodyLength;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField($"current body length : {script.bodyLength}", EditorStyles.centeredGreyMiniLabel);
		}

		

		serializedObject.ApplyModifiedProperties();
	}
}

