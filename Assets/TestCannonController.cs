using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestCannonController : MonoBehaviour
{
	// private const HitType myHitType = HitType.Explosive;
	// private readonly HitInfo myHitInfo = new HitInfo(myHitType);

	void Update()
	{
		if(Input.GetKeyDown("space"))
		{
			Fire();
		}
	}

	private void Fire()
	{
		RaycastHit[] hits;
		RaycastHit hit;

		hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);
		Debug.DrawRay(transform.position, transform.forward * 100f, Color.red);

		for(int i = 0; i < hits.Length; i++)
		{
			hit = hits[i];
			// hit.transform.GetComponent<PlanetElement>()?.GetHitBy(myHitInfo);
		}
	}
}
