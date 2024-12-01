using UnityEngine;

public class RigidMove : RigidUpdate
{
	public float force;

	private bool flip;

	private float time;

	private Vector3 dir = new Vector3(1f, 0f, 0f);

	public override void OnFixedUpdate()
	{
		if (!(rb.velocity.y > 0.3f) && !(rb.velocity.y < -0.3f))
		{
			time += RigidUpdate.delta;
			dir.x = ((!flip) ? 1 : (-1));
			rb.transform.position = rb.transform.position + dir * force;
			if (rb.transform.position.x > RigidUpdate.rightX)
			{
				flip = true;
			}
			else if (rb.transform.position.x < RigidUpdate.leftX)
			{
				flip = false;
			}
		}
	}
}
