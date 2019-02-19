using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TeleporterScript))]
public class TeleporterEditor : Editor
{
	/*public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		TeleporterScript myScript = (TeleporterScript)target;
		TeleporterStationScript teleAScript = myScript.transform.Find("TeleporterA").GetComponent<TeleporterStationScript>();
		TeleporterStationScript teleBScript = myScript.transform.Find("TeleporterB").GetComponent<TeleporterStationScript>();

		if(myScript.Type == TeleporterType.Planet)
		{
			myScript.PlanetA = (Transform)EditorGUILayout.ObjectField("Planet A", myScript.PlanetA, typeof(Transform), true);
			myScript.PlanetB = (Transform)EditorGUILayout.ObjectField("Planet B", myScript.PlanetB, typeof(Transform), true);

			if(myScript.PlanetA != null && teleAScript.Planet == null)
				teleAScript.Planet = myScript.PlanetA;

			if(myScript.PlanetB != null && teleBScript.Planet == null)
				teleBScript.Planet = myScript.PlanetB;
		}
		else
		{
			teleAScript.Planet = null;
			teleBScript.Planet = null;
		}

		if(myScript.Mode == TeleporterMode.Transfert)
		{
			myScript.Duration = EditorGUILayout.Slider("Duration", myScript.Duration, 0.1f, 10.0f);
			if(!myScript.transform.Find("Balises"))
			{
				GameObject balise = new GameObject();
				balise.transform.parent = myScript.transform;
				balise.name = "Balises";
			}
		}
		else if(myScript.transform.Find("Balises"))
		{
			DestroyImmediate(myScript.transform.Find("Balises").gameObject);
		}
	}*/

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		TeleporterScript myScript = (TeleporterScript)target;

		if(myScript.Mode == TeleporterMode.Transfert)
		{
			if(!myScript.transform.Find("Balises"))
			{
				GameObject balise = new GameObject();
				balise.transform.parent = myScript.transform;
				balise.name = "Balises";
			}
		}
		else if(myScript.transform.Find("Balises"))
		{
			DestroyImmediate(myScript.transform.Find("Balises").gameObject);
		}
	}
}