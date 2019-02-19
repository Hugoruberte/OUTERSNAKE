using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TeleporterGizmo : MonoBehaviour
{
	private Transform Balises;
	private Transform[] Teleporters;
	private int len = 0;

	void Start()
	{
		Search();
	}

	public void Search()
	{
		GameObject[] tmp = GameObject.FindGameObjectsWithTag("Teleporter");
		len = tmp.Length;
		Teleporters = new Transform[len];
		for(int i = 0; i < len; i++)
			Teleporters[i] = tmp[i].transform;
	}

	void OnDrawGizmos()
	{
		for(int i = 0; i < len; i++)
		{
			if(Teleporters[i] && Teleporters[i].name.Contains("A"))
			{
				Balises = Teleporters[i].parent.Find("Balises");
				
				if(Balises && Balises.childCount > 1)
				{
					for(int j = 0; j < Balises.childCount - 1; j++)
					{
						Gizmos.color = Color.green;
						Gizmos.DrawLine(Balises.GetChild(j).position, Balises.GetChild(j+1).position);
					}
				}
				else
				{
					Gizmos.color = Color.green;
					Gizmos.DrawLine(Teleporters[i].position, Teleporters[i].parent.Find("TeleporterB").position);
				}
			}
		}
	}
}
