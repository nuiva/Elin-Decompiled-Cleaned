using UnityEngine;

public class RigidUpdateSound : RigidUpdate
{
	private float timer;

	private void Awake()
	{
		timer = Random.Range(1f, 10f);
	}

	public override void OnFixedUpdate()
	{
		timer -= RigidUpdate.delta;
		if (timer < 0f)
		{
			base.actor.PlaySound();
			timer = Random.Range(1f, 5f);
		}
	}
}
