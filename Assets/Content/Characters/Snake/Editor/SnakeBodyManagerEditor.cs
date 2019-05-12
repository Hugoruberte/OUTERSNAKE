using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Snakes;

[CustomEditor(typeof(SnakeBodyManager))]
public class SnakeBodyManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SnakeBodyManager script = target as SnakeBodyManager;

		DrawDefaultInspector();

		if(EditorApplication.isPlaying){GUI.enabled = false;}
		script.bodyLength = EditorGUILayout.IntSlider("Snake Body Length", script.bodyLength, SnakeBodyManager.SNAKE_MINIMAL_LENGTH, 1000);
		if(EditorApplication.isPlaying){GUI.enabled = true;}

		if(GUILayout.Button("Hide body")) {
			
		}
	}
}

