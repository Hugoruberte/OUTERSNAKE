using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PoolingData", menuName = "Scriptable Object/Data/PoolingData", order = 3)]
public class PoolingData : ScriptableObject
{
	[System.Serializable]
	public struct Pool {
		public PoolableEntity prefab;
		public int size;

		[System.NonSerialized]
		public PoolableEntity[] objects;

		[System.NonSerialized]
		public Transform activeFolder;
		[System.NonSerialized]
		public Transform inactiveFolder;

		public Pool(PoolableEntity p, int s) {
			this.prefab = p;
			this.size = s;

			this.objects = null;
			this.activeFolder = null;
			this.inactiveFolder = null;
		}

		public Pool(Pool p, PoolableEntity[] a, Transform af, Transform iaf) {
			this.prefab = p.prefab;
			this.size = p.size;

			this.objects = a;
			this.activeFolder = af;
			this.inactiveFolder = iaf;
		}
	}


	public List<Pool> pools = new List<Pool>();


	public void CreatePool(Transform f)
	{
		Pool pp;
		GameObject g;
		Transform folder;
		Transform activeFolder;
		Transform inactiveFolder;
		PoolableEntity p;
		PoolableEntity[] objs;

		for(int k = 0; k < this.pools.Count; k++) {

			pp = this.pools[k];
			objs = new PoolableEntity[pp.size];

			// folder
			g = new GameObject();
			g.name = pp.prefab.gameObject.name;
			folder = g.transform;
			folder.parent = f;

			g = new GameObject();
			g.name = pp.prefab.gameObject.name + " (Active)";
			activeFolder = g.transform;
			activeFolder.parent = folder;

			g = new GameObject();
			g.name = pp.prefab.gameObject.name + " (Inactive)";
			inactiveFolder = g.transform;
			inactiveFolder.parent = folder;

			for(int i = 0; i < pp.size; i++) {
				p = Instantiate(pp.prefab);

				// initialize
				p.transform.parent = inactiveFolder;
				p.Reset();

				objs[i] = p;
			}

			this.pools[k] = new Pool(pp, objs, activeFolder, inactiveFolder);
		}
	}
}