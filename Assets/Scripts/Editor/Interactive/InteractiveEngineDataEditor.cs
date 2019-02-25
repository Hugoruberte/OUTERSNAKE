using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Interactive.Engine;

[CustomEditor(typeof(InteractiveEngineData))]
public class InteractiveEngineDataEditor : Editor
{
	private bool showPrimaries = true;
	private bool showWeaknesses = true;

	private ChemicalElement[] array;

	public override void OnInspectorGUI()
	{
		InteractiveEngineData script = target as InteractiveEngineData;

		DrawDefaultInspector();

		showPrimaries = EditorGUILayout.Foldout(showPrimaries, "Primaries");
		if(showPrimaries) {
			EditorGUI.indentLevel++;
			foreach(ChemicalElement e in script.primaries.Keys) {
				EditorGUILayout.LabelField(e.ToString());

				array = script.primaries[e];

				EditorGUI.indentLevel++;
				foreach(ChemicalElement k in array) {
					EditorGUILayout.EnumPopup("Element", k);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;
		}
		
		showWeaknesses = EditorGUILayout.Foldout(showWeaknesses, "Weaknesses");
		if(showWeaknesses) {
			EditorGUI.indentLevel++;
			foreach(ChemicalElement e in script.weaknesses.Keys) {
				EditorGUILayout.LabelField(e.ToString());

				array = script.weaknesses[e];

				if(array == null) {
					Debug.Log(e + " -> null");
				} else {
					EditorGUI.indentLevel++;
					foreach(ChemicalElement k in array) {
						EditorGUILayout.EnumPopup("Element", k);
					}
					EditorGUI.indentLevel--;
				}
			}
			EditorGUI.indentLevel--;
		}
		

		/*foreach(int c in script.couples.Keys) {
			ChemicalElementEntity e = script.couples[c];
			// Show element
		}

		foreach(int c in script.winners.Keys) {
			ChemicalElement e = script.winners[c];
			// Show element
		}*/
	}
}
