using System;
using UnityEngine;

public class RigidLeaf : RigidUpdate
{
	private void Awake()
	{
		this.maxTime = UnityEngine.Random.Range(0.5f, 1.5f);
	}

	public override void OnFixedUpdate()
	{
		Vector3 position = base.transform.position;
		if (base.transform.position.y < 3f)
		{
			this.power -= Time.fixedDeltaTime * 0.03f;
		}
		else
		{
			this.power = 1f;
		}
		if (this.power < 0f)
		{
			return;
		}
		this.time += this.flip * 0.01f;
		if (this.time > this.maxTime)
		{
			this.maxTime = UnityEngine.Random.Range(0.5f, 1.5f);
			this.time = this.maxTime;
			this.flip = -1f;
		}
		else if (this.time < 0f)
		{
			this.maxTime = UnityEngine.Random.Range(0.5f, 1.5f);
			this.time = 0f;
			this.flip = 1f;
		}
		this.f = this.time - this.maxTime * 0.5f;
		this.v.x = this.f * this.slidePower * this.power;
		this.v.y = (10f - Mathf.Clamp(((this.f > 0f) ? this.f : (-this.f)) * 10f, 0f, 10f)) * this.floatPower * this.power;
		this.rb.AddForce(this.v);
	}

	public float floatPower = 0.6f;

	public float slidePower = 15f;

	private Vector2 v;

	private float flip = 1f;

	private float time;

	private float maxTime;

	private float f;

	private float power = 1f;
}
