using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParticleSystemBackwardSimulation))]
public class ParticleSystemBackwardSimulationEditor : Editor
{
	public override void OnInspectorGUI()
	{
		ParticleSystemBackwardSimulation script = target as ParticleSystemBackwardSimulation;

		DrawDefaultInspector();

		EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
		script.startFromEnd = EditorGUILayout.Toggle("Start From End", script.startFromEnd);

		if(!script.startFromEnd) {
			script.startTime = EditorGUILayout.FloatField("Start Time", script.startTime);
			if(script.startTime < 0f) {
				script.startTime = 0f;
			}
		}

		script.simulationSpeedScale = EditorGUILayout.Slider("Simulation Speed", script.simulationSpeedScale, 0.1f, 20f);
	}
}
