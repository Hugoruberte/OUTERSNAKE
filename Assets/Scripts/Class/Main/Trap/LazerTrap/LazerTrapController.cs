using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class LazerTrapController : MovementController
{
	public LazerTrap lazerTrap { get; private set; }
	public Transform target;
	public float lastShootTime = 0f;

	private Transform myTransform;
	private Transform gun;

	public LazerTrapController(LazerTrap l) : base(l)
	{
		this.lazerTrap = l;
		this.myTransform = l.myTransform;
		this.gun = this.myTransform.DeepFind("Gun");
	}

	public override IEnumerator Wander()
	{
		float clock, pause;
		Vector3 axis;

		while(true)
		{
			axis = this.gun.up * (((Random.value >= 0.5f) ? 1 : -1) * this.lazerTrap.lazerTrapData.wanderOmega);
			clock = this.lazerTrap.lazerTrapData.wanderRotationDurationInterval[0] + Random.value * (this.lazerTrap.lazerTrapData.wanderRotationDurationInterval[1] - this.lazerTrap.lazerTrapData.wanderRotationDurationInterval[0]);

			while(clock > 0f) {
				clock -= Time.deltaTime;
				this.gun.Rotate(axis * Time.deltaTime);
				yield return null;
			}

			pause = 0.25f * Random.Range(1, this.lazerTrap.lazerTrapData.wanderMaxPauseQuarter);
			yield return Yielders.Wait(pause);
		}
	}

	public IEnumerator AimAt(Transform target)
	{
		Quaternion to;

		while(true) {
			to = Quaternion.LookRotation(target.position - this.gun.position, this.myTransform.up);
			this.gun.rotation = Quaternion.RotateTowards(this.gun.rotation, to, this.lazerTrap.lazerTrapData.aimOmega * Time.deltaTime);
			yield return null;
		}
	}

	public IEnumerator Shoot()
	{
		this.lazerTrap.Shoot();
		this.lastShootTime = Time.time;

		yield return Yielders.Wait(this.lazerTrap.lazerTrapData.shootDelay);
	}
}
