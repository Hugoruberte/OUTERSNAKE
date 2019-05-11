using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PoolingData", menuName = "Scriptable Object/Data/PoolingData", order = 3)]
public class PoolingData : ScriptableObject
{
	[Serializable]
	public struct Pool {
		public GameObject prefab;
		public int size;
		public PoolableEntity entity;

		[NonSerialized] public PoolableEntity[] entities;
		[NonSerialized] public Transform activeFolder;
		[NonSerialized] public Transform inactiveFolder;

		// INSPECTOR
		public bool show;

		public Pool(GameObject p, int s) {
			this.prefab = p;
			this.size = s;
			this.entity = p?.GetComponent<PoolableEntity>();

			this.entities = null;
			this.activeFolder = null;
			this.inactiveFolder = null;

			this.show = false;
		}

		public Pool(Pool p, PoolableEntity[] a, Transform af, Transform iaf) {
			this.prefab = p.prefab;
			this.size = p.size;
			this.entity = this.prefab.GetComponent<PoolableEntity>();

			this.entities = a;
			this.activeFolder = af;
			this.inactiveFolder = iaf;

			this.show = false;
		}
	}


	[HideInInspector] public List<Pool> pools = new List<Pool>();


	public void CreatePool(Transform f)
	{
		Pool pp;
		GameObject g;
		Transform folder, objectFolder;
		Transform activeFolder, inactiveFolder;
		PoolableEntity p;
		PoolableEntity[] objs;

		// scriptable object objectFolder
		g = new GameObject();
		g.name = this.name;
		folder = g.transform;
		folder.parent = f;

		for(int k = 0; k < this.pools.Count; k++) {

			pp = this.pools[k];
			objs = new PoolableEntity[pp.size];

			// object folders
			g = new GameObject();
			g.name = pp.prefab.name;
			objectFolder = g.transform;
			objectFolder.parent = folder;

			g = new GameObject();
			g.name = pp.prefab.name + " (Active)";
			activeFolder = g.transform;
			activeFolder.parent = objectFolder;

			g = new GameObject();
			g.name = pp.prefab.name + " (Inactive)";
			inactiveFolder = g.transform;
			inactiveFolder.parent = objectFolder;

			for(int i = 0; i < pp.size; i++) {
				g = Instantiate(pp.prefab);
				p = g.GetComponent<PoolableEntity>();

				// initialize
				p.transform.parent = inactiveFolder;
				p.Reset();

				objs[i] = p;
			}

			this.pools[k] = new Pool(pp, objs, activeFolder, inactiveFolder);
		}
	}

	public void AddAt(int index)
	{
		Pool newcomer = new Pool(null, 1);
		this.pools.Insert(index, newcomer);
	}

	public void AddNew(GameObject g)
	{
		Pool newcomer = new Pool(g, 1);
		newcomer.show = true;

		this.pools.Add(newcomer);
	}
}