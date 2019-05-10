using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Interactive.Engine;
using System.Text;

[CustomEditor(typeof(InteractiveEngine))]
public class InteractiveEngineEditor : Editor
{
	private bool showPrimaries = true;
	private bool showWeaknesses = true;
	private StringBuilder builder = new StringBuilder();

	private float val = 0f;

	public override void OnInspectorGUI()
	{
		InteractiveEngine script = target as InteractiveEngine;

		DrawDefaultInspector();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Informations", EditorStyles.boldLabel);

		EditorGUI.indentLevel ++;

		this.showPrimaries = EditorGUILayout.Foldout(this.showPrimaries, "Primaries");
		if(this.showPrimaries)
		{
			EditorGUI.indentLevel ++;
			foreach(ChemicalToArrayData e in script.primaries)
			{
				Rect rect = EditorGUILayout.GetControlRect(true, 0);

				this.builder.Clear();
				this.builder.Append(e.element.ToString()).Append(" :");
				EditorGUILayout.LabelField(this.builder.ToString(), EditorStyles.boldLabel);

				Rect n = new Rect();
				n.x = rect.x + 30;
				n.y += 600;
				n.width = 200;
				n.height = 20;
				val = EditorGUI.FloatField(n, "Val", val);

				rect.x += this.GetWidth(this.builder.ToString()) * 1.4f + -5;
				rect.y += 2;
				rect.width = 300;
				rect.height = 20;

				this.builder.Clear();
				this.builder.Append(e.array[0].ToString());
				for(int i = 1; i < e.array.Length; i++) {
					this.builder.Append(" + ").Append(e.array[i].ToString());
				}

				EditorGUI.LabelField(rect, this.builder.ToString(), EditorStyles.miniLabel);
				EditorGUILayout.Space();
			}
			EditorGUILayout.Space();
			EditorGUI.indentLevel --;
		}
		
		this.showWeaknesses = EditorGUILayout.Foldout(this.showWeaknesses, "Weaknesses");
		if(this.showWeaknesses)
		{
			EditorGUI.indentLevel ++;
			foreach(ChemicalToArrayData e in script.weaknesses)
			{
				EditorGUILayout.LabelField(e.element.ToString(), EditorStyles.boldLabel);

				EditorGUI.indentLevel++;
				foreach(ChemicalElement k in e.array) {
					EditorGUILayout.LabelField(k.ToString(), EditorStyles.miniLabel);
				}
				EditorGUILayout.Space();
				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel --;
		}

		EditorGUI.indentLevel --;
		

		/*foreach(int c in script.couples.Keys) {
			ChemicalElementEntity e = script.couples[c];
			// Show element
		}

		foreach(int c in script.winners.Keys) {
			ChemicalElement e = script.winners[c];
			// Show element
		}*/
	}

	private float GetWidth(string s)
	{
		float min, max;

		GUIContent content = new GUIContent(s);
		GUIStyle style = EditorStyles.miniLabel;

		style.CalcMinMaxWidth(content, out min, out max);

		return max;
	}
}
