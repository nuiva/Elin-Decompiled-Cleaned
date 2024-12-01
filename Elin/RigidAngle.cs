using UnityEngine;

public class RigidAngle : RigidUpdate
{
	public float angle;

	public float force = 0.05f;

	public override void OnFixedUpdate()
	{
		float num = Mathf.Lerp(rb.rotation, angle, force);
		rb.MoveRotation(num);
	}
}
