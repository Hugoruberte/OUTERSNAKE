using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using My.Tools;

[CustomEditor(typeof(PoolingData))]
public class PoolingDataEditor : Editor
{
	private Texture trashTexture;
	private Texture plusTexture;

	private readonly Color grey = new Color32(170, 170, 170, 255);
	private readonly Color dark = new Color32(140, 140, 140, 255);

	private PoolingData script;

	// private float val = 0f;

	void OnEnable()
	{
		this.trashTexture = EditorGUIUtility.FindTexture("d_TreeEditor.Trash");
		this.plusTexture = EditorGUIUtility.FindTexture("Toolbar Plus");
	}

	public override void OnInspectorGUI()
	{
		this.script = target as PoolingData;

		DrawDefaultInspector();

		EditorGUILayout.Space();
		if(GUILayout.Button("ADD NEW POOL")) {
			script.AddAt(script.pools.Count);
		}

		if(GUILayout.Button("REMOVE ALL POOLS")) {
			script.pools.Clear();
		}

		Rect rect = EditorGUILayout.GetControlRect(true, 0);
		Rect baserect = rect;

		EditorGUILayoutExtension.DragAndDropField(635, "", false, this.OnDrag, this.Rules);

		if(script.pools.Count == 0) {
			return;
		}

		baserect.y += 9;
		baserect.width = 50;
		baserect.height = 20;
		EditorGUI.LabelField(baserect, "Pool", EditorStyles.boldLabel);

		// Rect n = rect;
		// n.x += 30;
		// n.y += 450;
		// n.width = 200;
		// n.height = 20;
		// val = EditorGUI.FloatField(n, "Value", val);

		GUIStyle style = new GUIStyle(EditorStyles.label);
		GUIStyle n_style = new GUIStyle(EditorStyles.miniLabel);

		n_style.fontSize = 8;
		style.richText = true;
		
		PoolingData.Pool pool;

		baserect.y += 18;

		for(int i = 0; i < script.pools.Count; ++i)
		{
			pool = script.pools[i];

			baserect.x = rect.x + -8;
			baserect.width = rect.width + 7;
			baserect.height = 21;
			EditorGUI.DrawRect(baserect, grey);

			baserect.x += 15;
			baserect.y += 3;
			baserect.width = 0;
			baserect.height = 15;
			pool.show = EditorGUI.Foldout(baserect, pool.show, "");

			baserect.y += -1;
			baserect.width = 1000;
			baserect.height = 40;
			if(pool.prefab) {
				EditorGUI.LabelField(baserect, pool.prefab.name, style);

				baserect.x = EditorUtilityExtension.GetTextWidth(pool.prefab.name, EditorStyles.label) + 21;
				EditorGUI.LabelField(baserect, $"({pool.size})", n_style);
			} else {
				EditorGUI.LabelField(baserect, "<color=yellow>Empty</color>", style);
			}

			baserect.x = rect.width + -7;
			baserect.y += 1;
			baserect.width = 20;
			baserect.height = 20;
			if(GUI.Button(baserect, this.plusTexture, GUIStyle.none)) {
				script.AddAt(i + 1);
				return;
			}

			if(pool.show)
			{
				baserect.x = rect.width + -24;
				baserect.width = 16;
				baserect.height = 16;
				if(GUI.Button(baserect, this.trashTexture, GUIStyle.none)) {
					script.pools.RemoveAt(i);
					return;
				}

				baserect.x = rect.x + -8;
				baserect.y += 18;
				baserect.width = rect.width + 7;
				baserect.height = 46;
				EditorGUI.DrawRect(baserect, grey);

				baserect.x += 3;
				baserect.width += -6;
				baserect.height += -3;
				EditorGUI.DrawRect(baserect, dark);

				baserect.x += 1;
				baserect.y += 3;
				baserect.width = rect.width + -2;
				baserect.height = 17;
				EditorGUIUtility.labelWidth += -77;
				pool.prefab = EditorGUI.ObjectField(baserect, "Prefab", pool.prefab, typeof(GameObject), false) as GameObject;

				EditorGUIUtility.labelWidth += 1;
				baserect.y += 20;
				baserect.width = rect.width + -4;
				baserect.height = 17;
				pool.size = EditorGUI.IntSlider(baserect, "Size", pool.size, 1, 1000);

				baserect.y += 5;

				EditorGUIUtility.labelWidth += 76;
			}

			baserect.y += 21;

			script.pools[i] = pool;
		}

		EditorUtilityExtension.SetDirtyOnGUIChange(script);
	}

	private bool Rules(Object o)
	{
		if(!(o is GameObject)) {
			Debug.LogWarning("WARNING : " + o + " is not a GameObject.");
			return false;
		}

		GameObject g = o as GameObject;

		if(g.GetComponent<PoolableEntity>() == null) {
			Debug.LogWarning("WARNING : " + g + " is not a Poolable Entity.");
			return false;
		}

		return true;
	}

	private void OnDrag(Object dragged)
	{
		GameObject g = dragged as GameObject;
		this.script.AddNew(g);
	}
}
