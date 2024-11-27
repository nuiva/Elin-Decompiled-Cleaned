using System;
using UnityEngine;

public class RigidExplode : RigidUpdate
{
	private void Start()
	{
		this.interval = UnityEngine.Random.Range(this.intervalMin, this.intervalMax);
	}

	public void Explode()
	{
		Vector2 position = this.rb.position;
		if (this.particle)
		{
			ParticleSystem p = UnityEngine.Object.Instantiate<ParticleSystem>(this.particle);
			p.transform.position = position;
			TweenUtil.Delay(5f, delegate
			{
				if (p && p.gameObject)
				{
					UnityEngine.Object.DestroyImmediate(p.gameObject);
				}
			});
		}
		foreach (RaycastHit2D raycastHit2D in Physics2D.CircleCastAll(position, this.radius, Vector3.forward))
		{
			Rigidbody2D component = raycastHit2D.collider.GetComponent<Rigidbody2D>();
			if (component)
			{
				Vector2 a = component.position - position;
				component.AddForce(a * this.force * (1f + component.mass * this.massPower), ForceMode2D.Impulse);
			}
		}
		if (!this.sound.IsEmpty())
		{
			EMono.Sound.Play(this.sound, this.rb.position, 1f);
		}
	}

	public override void OnFixedUpdate()
	{
		this.time += RigidUpdate.delta;
		if (this.time > this.interval)
		{
			if (this.chance >= UnityEngine.Random.Range(0f, 1f))
			{
				this.Explode();
				if (this.destroy)
				{
					this.active = false;
					CollectibleActor component = base.gameObject.GetComponent<CollectibleActor>();
					if (component)
					{
						component.Deactivate();
						return;
					}
					base.gameObject.SetActive(false);
					return;
				}
			}
			if (this.repeat)
			{
				this.time = 0f;
				return;
			}
			this.active = false;
		}
	}

	public ParticleSystem particle;

	public float force = 20f;

	public float radius = 2.5f;

	public float intervalMin = 5f;

	public float intervalMax = 30f;

	public float massPower = 0.1f;

	public float chance = 0.8f;

	public bool repeat;

	public bool destroy = true;

	public string sound = "explode";

	private float time;

	private float interval = 1f;
}
