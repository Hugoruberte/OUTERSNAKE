using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class TurretController : MovementController
{
	public Turret turret { get; private set; }
	public Transform target;
	public float lastShootTime = 0f;
	public float aimStartTime = 0f;

	private Transform myTransform;
	private Transform gun;
	private Transform muzzle;
	private HaloEffect halo;
	private IEnumerator onAimCoroutine = null;
	private TargetEffect targetEffect;

	private const float AIM_TARGET_APPEAR_RADIUS = 5f;
	private const float AIM_TARGET_SMOOTH_LIMIT = 300f;
	private const float AIM_TARGET_HEIGHT = 0.75f;
	private const float AIM_TARGET_FADE_SPEED = 10f;


	public TurretController(Turret t) : base(t)
	{
		this.turret = t;
		this.myTransform = t.myTransform;
		this.gun = this.myTransform.DeepFind("Gun");
		this.muzzle = this.gun.DeepFind("Muzzle");
	}

	public override IEnumerator Wander()
	{
		float clock, pause;
		Vector3 axis;

		while(true)
		{
			axis = this.gun.up * (RandomExtension.randomSign * this.turret.turretData.wanderOmega);
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

	public IEnumerator AimAtTarget()
	{
		Quaternion to;
		Vector3 local, up;

		// Initialize
		up = this.myTransform.up;

		while(this.target)
		{
			// GUN ROTATION
			local = this.myTransform.InverseTransformPoint(this.target.position);
			local.Set(local.x, this.gun.localPosition.y, local.z);
			local = this.myTransform.TransformPoint(local);

			local = local - this.gun.position;
			if(local.magnitude > Mathf.Epsilon) {
				to = Quaternion.LookRotation(local, up);
				this.gun.rotation = Quaternion.RotateTowards(this.gun.rotation, to, this.turret.turretData.aimOmega * Time.deltaTime);
			}
			
			yield return null;
		}
	}

	public IEnumerator Shoot()
	{
		ContinuousParticleEffect load;
		InstantParticleEffect shoot;
		HaloEffect halo;

		Color color = Color.red;

		// Effect : Load
		load = PoolingManager.instance.Get<ContinuousParticleEffect>(this.turret.turretData.loadPrefab);
		if(load) {
			load.SetColor(color);
			load.SetFollow(this.muzzle);
			load.SetPosition(this.muzzle.position);
			load.Launch();
		}

		yield return Yielders.Wait(this.turret.turretData.loadDuration);

		load?.Stop();

		// Effect : Halo
		halo = PoolingManager.instance.Get<HaloEffect>(this.turret.turretData.haloPrefab);
		if(halo) {
			halo.SetColor(color);
			halo.SetDirection(this.myTransform.up);
			halo.SetFollow(this.muzzle, this.myTransform.up * 0.1f);
			halo.SetDuration(0.15f);
			halo.Launch(2.5f, 0.1f);
		}

		yield return Yielders.Wait(0.15f);
		
		// Effect : Shoot
		shoot = PoolingManager.instance.Get<InstantParticleEffect>(this.turret.turretData.shootPrefab);
		if(shoot) {
			shoot.SetColor(color);
			shoot.SetFollow(this.muzzle);
			shoot.SetPosition(this.muzzle.position);
			shoot.Launch();
		}
		
		// Shoot
		this.turret.Shoot();
		this.lastShootTime = Time.time;
	}





	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------- SUB ACTIONS ----------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	/* --------------------------------------------------------------------------------------------*/
	public void OnStartAimAt()
	{
		// Get effect
		this.targetEffect = PoolingManager.instance.Get<TargetEffect>(this.turret.turretData.targetPrefab);

		if(this.targetEffect) {
			this.turret.StartAndStopCoroutine(ref this.onAimCoroutine, this.OnStartAimAtCoroutine(this.targetEffect));
		}
	}

	public void OnEndAimAt()
	{
		this.targetEffect?.HideAway(AIM_TARGET_FADE_SPEED);
		this.targetEffect = null;
	}

	private IEnumerator OnStartAimAtCoroutine(TargetEffect effect)
	{
		Vector3 ps, up, dest, dir;
		float force = 0f;
		Rigidbody rb;

		if(!this.target) {
			yield break;
		}

		// Initialize
		force = 5f;
		up = this.myTransform.up;
		ps = this.target.position + RandomExtension.OnUnitAxisCircle(up) * AIM_TARGET_APPEAR_RADIUS;

		// Set effect
		effect.SetOrientation(ps + up * AIM_TARGET_HEIGHT, up);
		effect.Launch(this.turret.turretData.aimTargetSmooth, AIM_TARGET_FADE_SPEED);
		rb = effect.GetComponent<Rigidbody>();

		while(this.target)
		{
			dest = this.target.position + up * AIM_TARGET_HEIGHT;
			dir = dest - rb.position;
			rb.AddForce(dir * force, ForceMode.Acceleration);

			if(force < AIM_TARGET_SMOOTH_LIMIT) {
				force += 25f * Time.deltaTime;
			}

			yield return Yielders.fixedUpdate;
		}
	}
}
