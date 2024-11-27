using System;
using UnityEngine;

public class RigidFloat : RigidUpdate
{
	private void Awake()
	{
		this._airHeight = this.airHeight + UnityEngine.Random.Range(-2f, 2f);
		this.time = UnityEngine.Random.Range(0f, this.pingpongTime);
	}

	public override void OnFixedUpdate()
	{
		this.time += Time.fixedDeltaTime * this.flip;
		if (this.time > this.pingpongTime)
		{
			this.flip = -1f;
			this.time = this.pingpongTime;
		}
		else if (this.time < 0f)
		{
			this.flip = 1f;
			this.time = 0f;
		}
		float num = this.time - this.pingpongTime * 0.5f;
		if (num > 0f)
		{
			num = Mathf.Sqrt(num);
		}
		else
		{
			num = Mathf.Sqrt(-num) * -1f;
		}
		float num2 = this._airHeight + num * this.pingpongPower;
		float num3 = base.transform.position.y;
		if (num3 < num2)
		{
			if (num3 < 1f)
			{
				num3 = 1f;
			}
			this.rb.AddForce(new Vector2(0f, this.floatPower * (1f - num3 / num2)));
		}
	}

	public float airHeight = 3.5f;

	public float floatPower = 30f;

	public float pingpongTime = 1f;

	public float pingpongPower = 0.15f;

	private float _airHeight;

	private float time;

	private float flip = 1f;
}
