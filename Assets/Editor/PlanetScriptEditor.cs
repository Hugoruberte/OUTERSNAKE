/*using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlanetScript))]
public class PlanetScriptEditor : Editor
{
	public override void OnInspectorGUI()
	{
		PlanetScript myScript = (PlanetScript)target;

		EditorGUILayout.Space();
		var origFontStyle = EditorStyles.label.fontStyle;
		EditorStyles.label.fontStyle = FontStyle.Bold;
		myScript.MainPlanet = EditorGUILayout.Toggle("Main Planet", myScript.MainPlanet, EditorStyles.toggle);
		EditorStyles.label.fontStyle = origFontStyle;

		EditorGUILayout.Space();
		origFontStyle = EditorStyles.label.fontStyle;
		EditorStyles.label.fontStyle = FontStyle.Bold;
		myScript.All = EditorGUILayout.Toggle("All Objects", myScript.All, EditorStyles.toggle);
		EditorStyles.label.fontStyle = origFontStyle;

		if(!myScript.All)
		{
			EditorGUILayout.Space();
			myScript.Environment = EditorGUILayout.BeginToggleGroup("Environment", myScript.Environment);
			myScript.Apple = EditorGUILayout.Toggle("Apple", myScript.Apple);
			myScript.Rabbit = EditorGUILayout.Toggle("Rabbit", myScript.Rabbit);
			myScript.Rocket = EditorGUILayout.Toggle("Rocket", myScript.Rocket);
			myScript.Trees = EditorGUILayout.Toggle("Trees", myScript.Trees);
			EditorGUILayout.EndToggleGroup();

			if(!myScript.Environment)
			{
				myScript.Apple = false;
				myScript.Rabbit = false;
				myScript.Rocket = false;
				myScript.Trees = false;
			}
			

			myScript.Trap = EditorGUILayout.BeginToggleGroup("Trap", myScript.Trap);
			myScript.Lazer = EditorGUILayout.Toggle("Lazer", myScript.Lazer);
			myScript.RedRabbit = EditorGUILayout.Toggle("RedRabbit", myScript.RedRabbit);
			myScript.NuclearSwitch = EditorGUILayout.Toggle("NuclearSwitch", myScript.NuclearSwitch);
			EditorGUILayout.EndToggleGroup();

			if(!myScript.Trap)
			{
				myScript.Lazer = false;
				myScript.RedRabbit = false;
				myScript.NuclearSwitch = false;
			}
			

			myScript.Events = EditorGUILayout.BeginToggleGroup("Event", myScript.Events);
			myScript.SuperLazer = EditorGUILayout.Toggle("SuperLazer", myScript.SuperLazer);
			myScript.CircularSaw = EditorGUILayout.Toggle("CircularSaw", myScript.CircularSaw);
			myScript.CasterBlaster = EditorGUILayout.Toggle("CasterBlaster", myScript.CasterBlaster);
			myScript.Meteore = EditorGUILayout.Toggle("Meteore", myScript.Meteore);
			EditorGUILayout.EndToggleGroup();

			if(!myScript.Events)
			{
				myScript.SuperLazer = false;
				myScript.CircularSaw = false;
				myScript.CasterBlaster = false;
				myScript.Meteore = false;
			}
		}
		else
		{
			myScript.Environment = true;
			myScript.Trap = true;
			myScript.Events = true;

			myScript.Apple = true;
			myScript.Rabbit = true;
			myScript.Rocket = true;
			myScript.Trees = true;
			myScript.Lazer = true;
			myScript.RedRabbit = true;
			myScript.NuclearSwitch = true;
			myScript.SuperLazer = true;
			myScript.CircularSaw = true;
			myScript.CasterBlaster = true;
			myScript.Meteore = true;
		}
	}

	void OnFocus()
	{
		if(EditorPrefs.HasKey("All"))
			showRoundPosition = EditorPrefs.GetBool("All");
		if(EditorPrefs.HasKey("ShowRoundRotation"))
			showRoundPosition = EditorPrefs.GetBool("ShowRoundRotation");
	}

	void OnLostFocus()
	{
		EditorPrefs.SetBool("ShowRoundPosition", showRoundPosition);
		EditorPrefs.SetBool("ShowRoundRotation", showRoundRotation);
	}

	void OnDestroy()
	{
		EditorPrefs.SetBool("ShowRoundPosition", showRoundPosition);
		EditorPrefs.SetBool("ShowRoundRotation", showRoundRotation);
	}
}*/
