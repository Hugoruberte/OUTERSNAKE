/*using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WhiteYellowRabbitController))]
public class WhiteYellowControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		WhiteYellowRabbitController myScript = (WhiteYellowRabbitController)target;

		EditorGUILayout.Space();

		if(myScript.myCell >= 0)
		{
			EditorGUILayout.LabelField("Cell :", myScript.myCell.ToString());
		}
		else
		{
			EditorGUILayout.LabelField("Cell : None");
		}
	}
}*/
