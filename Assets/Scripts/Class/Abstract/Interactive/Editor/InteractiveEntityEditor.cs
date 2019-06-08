using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Interactive.Engine;

[CustomEditor(typeof(InteractiveEntity), true), CanEditMultipleObjects]
public class InteractiveEntityEditor : Editor
{
	private bool showInteractiveInfo = false;
	private bool showCellInfo = false;
	private bool hasCell;
	
	protected Rect rect;

	// float val = 0f;

	public override void OnInspectorGUI()
	{
		InteractiveEntity script = target as InteractiveEntity;

		DrawDefaultInspector();

		if(!Application.isPlaying) {
			return;
		}

		EditorGUILayout.Space();
		showInteractiveInfo = EditorGUILayout.Foldout(showInteractiveInfo, "");
		rect = EditorGUILayout.GetControlRect(true, 0);
		rect.y += -18;
		rect.height += 20;
		EditorGUI.LabelField(rect, "Interactive Information", EditorStyles.boldLabel);

		// Rect n = new Rect();
		// n.x = rect.x;
		// n.y += 500;
		// n.width = 200;
		// n.height = 16;
		// val = EditorGUI.FloatField(n, "Val", val);

		if(showInteractiveInfo) {
			EditorGUI.indentLevel++;
			GUI.enabled = false;

			rect.x += rect.width + -110;

			if(EditorApplication.isPlaying) {
				// physical state
				rect.y += 19;
				EditorGUILayout.LabelField("Physical state");
				EditorGUI.LabelField(rect, script.physical.ToString(), EditorStyles.boldLabel);

				// chemical element
				rect.y += 18;
				EditorGUILayout.LabelField("Chemical element");
				EditorGUI.LabelField(rect, script.chemical.ToString(), EditorStyles.boldLabel);

				// chemical material
				rect.y += 18;
				EditorGUILayout.LabelField("Chemical material");
				EditorGUI.LabelField(rect, script.material.ToString(), EditorStyles.boldLabel);
			}

			// life
			GUI.enabled = true;
			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
		}


		if(script.cellable.initialized) {
			hasCell = (script.cellable.currentCell != null);

			showCellInfo = EditorGUILayout.Foldout(showCellInfo, "");
			rect = EditorGUILayout.GetControlRect(true, 0);
			rect.y += -18;
			rect.width = 150;
			rect.height = 50;
			EditorGUI.LabelField(rect, "Cell Information", EditorStyles.boldLabel);

			if(showCellInfo) {
				EditorGUI.indentLevel++;
				GUI.enabled = false;

				if(hasCell) {
					EditorGUILayout.Vector3Field("Position :", script.cellable.currentCell.position);
					EditorGUILayout.Toggle("Is Inner :", script.cellable.currentCell.isInner);
				}

				EditorGUILayout.Toggle("Is Walkable :", script.cellable.isWalkable);

				GUI.enabled = true;
				EditorGUI.indentLevel--;
			}
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}

