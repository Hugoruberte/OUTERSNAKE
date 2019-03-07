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
		public Transform folder;

		public Pool(PoolableEntity p, int s) {
			this.prefab = p;
			this.size = s;
			this.objects = null;
			this.folder = null;
		}

		public Pool(Pool p, PoolableEntity[] a, Transform f) {
			this.prefab = p.prefab;
			this.size = p.size;

			this.objects = a;
			this.folder = f;
		}
	}


	public List<Pool> pools = new List<Pool>();


	public void CreatePool(Transform f)
	{
		Pool pp;
		GameObject g;
		Transform folder;
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

			for(int i = 0; i < pp.size; i++) {
				p = Instantiate(pp.prefab);

				// initialize
				p.transform.parent = folder;
				p.Reset();

				objs[i] = p;
			}

			this.pools[k] = new Pool(pp, objs, folder);
		}
	}
}