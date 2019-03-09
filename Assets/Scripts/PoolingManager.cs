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

		this.poolingData.CreatePool(this.folder);
	}

	public T Get<T>() where T : PoolableEntity
	{
		PoolableEntity entity = null;
		PoolingData.Pool pool = this.poolingData.pools.Find(x => x.prefab is T);

		if(pool.prefab != null) {
			foreach(PoolableEntity e in pool.objects) {
				if(!e.isActive) {

					entity = e;
					break;
				}
			}
		}

		if(entity != null) {
			entity.isActive = true;
			entity.transform.parent = null;
		}
		
		return entity as T;
	}

	public void Stow(PoolableEntity entity)
	{
		Transform folder;

		folder = this.poolingData.pools.Find(x => x.prefab.GetType() == entity.GetType()).folder;
		
		entity.Reset();
		entity.transform.parent = folder;
	}
}
