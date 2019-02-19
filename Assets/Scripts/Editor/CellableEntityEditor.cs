using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CellableEntity), true), CanEditMultipleObjects]
public class CellableEntityEditor : InteractiveEntityEditor
{
	private bool showCellInfo = false;
	private bool hasCell;

	public override void OnInspectorGUI()
	{
		CellableEntity script = target as CellableEntity;

		base.OnInspectorGUI();

		hasCell = (script.currentCell != null);

		showCellInfo = EditorGUILayout.Foldout(showCellInfo, "");
		rect = EditorGUILayout.GetControlRect(true, 0);
		rect.y += -18;
		rect.width = 150;
		rect.height = 50;
		EditorGUI.LabelField(rect, "Cell Information", EditorStyles.boldLabel);

		if(showCellInfo) {
			EditorGUI.indentLevel++;
			GUI.enabled = false;

			EditorGUILayout.Toggle("Is Walkable :", script.isWalkable);

			if(hasCell) {
				EditorGUILayout.Vector3Field("Position :", script.currentCell.position);
				EditorGUILayout.Toggle("Is Inner :", script.currentCell.isInner);
			}

			GUI.enabled = true;
			EditorGUILayout.Space();
			EditorGUI.indentLevel--;
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}

