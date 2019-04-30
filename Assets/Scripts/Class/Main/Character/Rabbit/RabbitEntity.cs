﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactive.Engine;
using Tools;

public abstract class RabbitEntity : CharacterEntity
{
	public RabbitData rabbitData;

	private GameObject death;
	private GameObject fire;
	private IEnumerator fireLightCoroutine = null;
	private Light fireLight;

	protected override void Awake()
	{
		base.Awake();

		// variable
		this.foodChainRank = 20;
		this.foodChainValue = 100f;

		// effect
		fire = myTransform.DeepFind("Fire").gameObject;
		fireLight = fire.GetComponentInChildren<Light>();

		death = myTransform.DeepFind("Death").gameObject;
	}

	public override void InteractWith(InteractiveStatus s, PhysicalInteractionEntity i)
	{
		if(this.currentSetOnElement != null) {
			this.currentSetOnElement(false);
			this.currentSetOnElement = null;
		}
		
		switch(s.element.type) {

			case ChemicalElement.Fire:
				this.currentSetOnElement += this.SetOnFire;
				break;

			default:
				this.currentSetOnElement = null;
				break;
		}

		if(this.currentSetOnElement != null) {
			this.currentSetOnElement(true);
		}
	}

	protected override void Death()
	{
		base.Death();

		// will not interact in future
		myCollider.enabled = false;
		this.behaviour.Remove(this);

		// disappear
		body.gameObject.SetActive(false);
		death.SetActive(true);

		// get destroyed
		Destroy(gameObject, 4f);
	}

	private void SetOnFire(bool active)
	{
		if(fire.activeInHierarchy && active) {
			return;
		}

		fire.SetActive(active);
		if(active) {
			fireLightCoroutine = FireLightCoroutine(fireLight);
			StartCoroutine(fireLightCoroutine);
		} else {
			StopCoroutine(fireLightCoroutine);
			fireLightCoroutine = null;
		}
	}

	private IEnumerator FireLightCoroutine(Light l)
	{
		while(true) {
			l.intensity = Random.Range(2f, 4f);
			yield return Yielders.Wait(0.1f);
		}
	}
}