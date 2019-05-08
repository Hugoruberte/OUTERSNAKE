using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class TurretController : MovementController
{
	public Turret turret { get; private set; }
	public Transform target;
	public float lastShootTime = 0f;

	private Transform myTransform;
	private Transform gun;
	private Transform muzzle;
	private ContinuousParticleEffect load;
	private InstantParticleEffect shoot;
	private HaloEffect halo;

	public TurretController(Turret l) : base(l)
	{
		this.turret = l;
		this.myTransform = l.myTransform;
		this.gun = this.myTransform.DeepFind("Gun");
		this.muzzle = this.gun.DeepFind("Muzzle");
	}

	public override IEnumerator Wander()
	{
		float clock, pause;
		Vector3 axis;

		while(true)
		{
			axis = this.gun.up * (((Random.value >= 0.5f) ? 1 : -1) * this.turret.turretData.wanderOmega);
			clock = this.turret.turretData.wanderRotationDurationInterval[0] + Random.value * (this.turret.turretData.wanderRotationDurationInterval[1] - this.turret.turretData.wanderRotationDurationInterval[0]);

			while(clock > 0f) {
				clock -= Time.deltaTime;
				this.gun.Rotate(axis * Time.deltaTime);
				yield return null;
			}

			pause = 0.25f * Random.Range(1, this.turret.turretData.wanderMaxPauseQuarter);
			yield return Yielders.Wait(pause);
		}
	}

	public IEnumerator AimAt(Transform target)
	{
		Quaternion to;

		while(true) {
			to = Quaternion.LookRotation(target.position - this.gun.position, this.myTransform.up);
			this.gun.rotation = Quaternion.RotateTowards(this.gun.rotation, to, this.turret.turretData.aimOmega * Time.deltaTime);
			yield return null;
		}
	}

	public IEnumerator Shoot()
	{
		Color color = Color.red;

		// Effect : Load
		this.load = PoolingManager.instance.Get<ContinuousParticleEffect>(this.turret.turretData.loadPrefab);
		if(this.load) {
			this.load.SetColor(color);
			this.load.SetFollow(this.muzzle);
			this.load.Launch(this.muzzle.position);
		}

		yield return Yielders.Wait(this.turret.turretData.loadDuration);

		this.load?.Stop();

		// Effect : Halo
		this.halo = PoolingManager.instance.Get<HaloEffect>(this.turret.turretData.haloPrefab);
		if(this.halo) {
			this.halo.SetColor(color);
			this.halo.SetDirection(this.myTransform.up);
			this.halo.SetFollow(this.muzzle, this.myTransform.up * 0.1f);
			this.halo.SetDuration(0.15f);
			this.halo.Launch(2.5f, 0.1f);
		}

		yield return Yielders.Wait(0.15f);
		
		// Effect : Shoot
		this.shoot = PoolingManager.instance.Get<InstantParticleEffect>(this.turret.turretData.shootPrefab);
		if(this.shoot) {
			this.shoot.SetColor(color);
			this.shoot.SetFollow(this.muzzle);
			this.shoot.Launch(this.muzzle.position);
		}
		
		// Shoot
		this.turret.Shoot();
		this.lastShootTime = Time.time;
	}
}
