using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using My.Tools;

namespace Interactive.Engine
{
	[CustomEditor(typeof(InteractiveEngine))]
	public class InteractiveEngineEditor : Editor
	{
		private StringBuilder builder = new StringBuilder();

		private readonly Color grey = new Color32(170, 170, 170, 255);
		private readonly Color dark = new Color32(140, 140, 140, 255);

		// private float val = 0f;


		public override void OnInspectorGUI()
		{
			InteractiveEngine script = target as InteractiveEngine;

			DrawDefaultInspector();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Elements", EditorStyles.boldLabel);

			Rect rect = EditorGUILayout.GetControlRect(true, 0);
			// Rect n = rect;
			// n.x += 30;
			// n.y += 600;
			// n.width = 200;
			// n.height = 20;
			// val = EditorGUI.FloatField(n, "Val", val);


			ChemicalToArrayData e;
			Rect elementrect = rect;
			Rect colorrect;

			if(script.inspector_showDetails == null || script.inspector_showDetails.Length != script.primaries.Count) {
				script.inspector_showDetails = new bool[script.primaries.Count];
			}
			
			for(int i = 0; i < script.primaries.Count; i++)
			{
				e = script.primaries[i];

				elementrect.x = rect.x + 3;
				elementrect.width = rect.width + -15;
				elementrect.height = 22;

				colorrect = elementrect;
				colorrect.x += -3;
				colorrect.y += -3;
				colorrect.width += 6;
				colorrect.height += 6;
				EditorGUI.DrawRect(colorrect, grey);
				EditorGUI.DrawRect(elementrect, dark);

				elementrect.x += 18;
				elementrect.y += 3;
				elementrect.width = 200;
				elementrect.height = 20;
				script.inspector_showDetails[i] = EditorGUI.Foldout(elementrect, script.inspector_showDetails[i], e.element.ToString());

				if(script.inspector_showDetails[i])
				{
					elementrect.x += -18;
					elementrect.y += elementrect.height - 1;
					elementrect.width = rect.width + -15;
					elementrect.height = 40;

					colorrect = elementrect;
					colorrect.x += -3;
					colorrect.y += 3;
					colorrect.width += 6;
					EditorGUI.DrawRect(colorrect, grey);
					EditorGUI.DrawRect(elementrect, dark);

					this.builder.Clear();
					this.builder.Append(e.array[0].ToString());
					for(int j = 1; j < e.array.Length; j++) {
						this.builder.Append(" + ").Append(e.array[j].ToString());
					}

					elementrect.x += 9;
					elementrect.y += -2;
					EditorGUI.LabelField(elementrect, "Recipe :", EditorStyles.boldLabel);
					elementrect.x += 56;
					EditorGUI.LabelField(elementrect, this.builder.ToString(), EditorStyles.miniLabel);

					elementrect.x += -56;
					elementrect.y += 20;
					EditorGUI.LabelField(elementrect, "Weaknesses :", EditorStyles.boldLabel);

					ChemicalElement[] weaknesses = script.weaknesses.Find(x => x.element == e.element).array;
					int index = 0;

					elementrect.x += 93;
					elementrect.width = rect.width + -125;

					bool back = false;

					do
					{
						colorrect = elementrect;

						this.builder.Clear();
						this.builder.Append(weaknesses[index ++]);

						while(index < weaknesses.Length) {
							if(EditorUtilityExtension.GetTextWidth(this.builder.ToString() + ", " + weaknesses[index], EditorStyles.miniLabel) > elementrect.width) { break; }
							this.builder.Append(", ").Append(weaknesses[index ++]);
						}

						if(back) {
							colorrect.x += -105;
							colorrect.y += 10;
							colorrect.width += 116;
							colorrect.height = 15;
							EditorGUI.DrawRect(colorrect, grey);
							colorrect.x += 3;
							colorrect.y += -3;
							colorrect.width += -6;
							colorrect.height = 15;
							EditorGUI.DrawRect(colorrect, dark);
						}
						
						EditorGUI.LabelField(elementrect, this.builder.ToString(), EditorStyles.miniLabel);
						elementrect.y += 15;
						back = true;
					}
					while(index < weaknesses.Length);

					elementrect.y += (i == 0) ? 8 : 10;
					elementrect.x += -84;
				}
				else
				{
					elementrect.y += 22;
				}
			}

			if(script.primaries.Count == 0) {
				elementrect.x += 20;
				elementrect.y += -5;
			}

			elementrect.x += -18;
			elementrect.y += 5;
			elementrect.width = rect.width + -15;
			elementrect.height = 20;
			if(GUI.Button(elementrect, "WARM UP")) {
				script.WarmUp();
			}
		}
	}
}


