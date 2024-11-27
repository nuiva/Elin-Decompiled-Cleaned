using System;
using UnityEngine;

public class RigidAngle : RigidUpdate
{
	public override void OnFixedUpdate()
	{
		float num = Mathf.Lerp(this.rb.rotation, this.angle, this.force);
		this.rb.MoveRotation(num);
	}

	public float angle;

	public float force = 0.05f;
}
