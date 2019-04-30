using UnityEngine;
using System;

public class PoolingManager : Singleton<PoolingManager>
{
	public PoolingData poolingData;

	private Transform folder;

	protected override void Awake()
	{
		base.Awake();

		GameObject g = new GameObject();
		g.name = "Object Pooling Folder";
		this.folder = g.transform;
	}

	void Start()
	{
		this.poolingData.CreatePool(this.folder);
	}

	public T Get<T>(string name) where T : PoolableEntity
	{
		PoolableEntity entity = null;
		PoolingData.Pool pool = this.poolingData.pools.Find(x => x.entity is T && x.prefab.name == name);

		if(pool.prefab == null) {
			Debug.LogError($"ERROR : No matching pool found with name '{name}' and type '{typeof(T)}' in pooling data !");
			return null;
		}

		foreach(PoolableEntity e in pool.entities) {
			if(!e.isActive) {
				entity = e;
				break;
			}
		}

		if(entity == null) {
			Debug.LogWarning($"WARNING : This pool '{name}' and '{typeof(T)}' has no entity available... You should increase pool size.");
			return null;
		}

		entity.isActive = true;
		entity.transform.parent = pool.activeFolder;
		return entity as T;
	}

	public T Get<T>(GameObject prefab) where T : PoolableEntity => this.Get<T>(prefab.name);

	public void Stow(PoolableEntity entity)
	{
		Transform folder;
		Type t = entity.GetType();

		folder = this.poolingData.pools.Find(x => x.entity.GetType() == t && Array.IndexOf(x.entities, entity) > -1).inactiveFolder;
		
		entity.Reset();
		entity.transform.parent = folder;
	}
}
