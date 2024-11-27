using System;
using UnityEngine;

public class RigidMove : RigidUpdate
{
	public override void OnFixedUpdate()
	{
		if (this.rb.velocity.y > 0.3f || this.rb.velocity.y < -0.3f)
		{
			return;
		}
		this.time += RigidUpdate.delta;
		this.dir.x = (float)(this.flip ? -1 : 1);
		this.rb.transform.position = this.rb.transform.position + this.dir * this.force;
		if (this.rb.transform.position.x > RigidUpdate.rightX)
		{
			this.flip = true;
			return;
		}
		if (this.rb.transform.position.x < RigidUpdate.leftX)
		{
			this.flip = false;
		}
	}

	public float force;

	private bool flip;

	private float time;

	private Vector3 dir = new Vector3(1f, 0f, 0f);
}
