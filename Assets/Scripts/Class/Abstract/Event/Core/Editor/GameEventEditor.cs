using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GameEvent script = target as GameEvent;

		DrawDefaultInspector();

		if(!Application.isPlaying) { GUI.enabled = false; }
		if(GUILayout.Button("Invoke")) {
			script.Invoke();
		}
		if(!Application.isPlaying) { GUI.enabled = true; }
	}
}
