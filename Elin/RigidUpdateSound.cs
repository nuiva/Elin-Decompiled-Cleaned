using System;
using UnityEngine;

public class RigidUpdateSound : RigidUpdate
{
	private void Awake()
	{
		this.timer = UnityEngine.Random.Range(1f, 10f);
	}

	public override void OnFixedUpdate()
	{
		this.timer -= RigidUpdate.delta;
		if (this.timer < 0f)
		{
			base.actor.PlaySound(null);
			this.timer = UnityEngine.Random.Range(1f, 5f);
		}
	}

	private float timer;
}
