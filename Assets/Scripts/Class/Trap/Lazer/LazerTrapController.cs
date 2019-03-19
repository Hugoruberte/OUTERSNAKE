using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerTrapController : MovementController
{
	private LazerTrap lazerTrap;

	private Transform myTransform;
	private Transform gun;

	public LazerTrapController(LazerTrap l) : base(l)
	{
		this.lazerTrap = l;
		this.myTransform = l.myTransform;
		this.gun = this.myTransform.DeepFind("Gun");
	}

	public IEnumerator AimAt(Transform target)
	{
		Quaternion to;

		while(true) {
			to = Quaternion.LookRotation(target.position - this.gun.position, this.myTransform.up);
			this.gun.rotation = Quaternion.RotateTowards(this.gun.rotation, to, this.lazerTrap.lazerTrapData.omega * Time.deltaTime);
			yield return null;
		}
	}
}
