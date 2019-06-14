using UnityEngine;
using System;

public class PoolingManager : MonoSingleton<PoolingManager>
{
	public PoolingData[] poolingDatas;

	private Transform folder;

	protected override void Awake()
	{
		base.Awake();

		GameObject g = new GameObject();
		g.name = "Object Pooling Folder";
		this.folder = g.transform;
	}

	private void Start()
	{
		foreach(PoolingData d in this.poolingDatas) {
			d.CreatePool(this.folder);
		}
	}

	public T Get<T>(string name) where T : PoolableEntity
	{
		PoolableEntity entity;
		PoolingData.Pool pool;
		int index;

		if(!this.enabled) {
			return null;
		}

		index = 0;
		do {
			pool = this.poolingDatas[index ++].pools.Find(x => x.entity is T && x.prefab.name == name);
		}
		while(!pool.prefab && index < this.poolingDatas.Length);

		if(!pool.prefab) {
			// Debug.LogError($"ERROR : No matching pool found with name '{name}' and type '{typeof(T)}' in pooling data !");
			return null;
		}

		if(pool.entities == null) {
			return null;
		}

		entity = null;
		foreach(PoolableEntity e in pool.entities) {
			if(!e.isActive) {
				entity = e;
				break;
			}
		}

		if(entity == null) {
			Debug.LogWarning($"WARNING : This pool name:'{name}' and type:'{typeof(T)}' has no more entity available... You should increase pool size.");
			return null;
		}

		entity.isActive = true;
		entity.transform.parent = pool.activeFolder;
		return entity as T;
	}

	public T Get<T>(GameObject prefab) where T : PoolableEntity => this.Get<T>(prefab.name);

	public void Stow(PoolableEntity entity)
	{
		PoolingData.Pool pool;
		Transform folder;
		Type t;
		int index;

		t = entity.GetType();
		index = 0;

		do {
			pool = this.poolingDatas[index ++].pools.Find(x => x.entity.GetType() == t && Array.IndexOf(x.entities, entity) > -1);
		}
		while(!pool.prefab && index < this.poolingDatas.Length);

		folder = pool.inactiveFolder;

		entity.Reset();
		entity.transform.parent = folder;
	}
}
