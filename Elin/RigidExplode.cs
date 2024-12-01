using UnityEngine;

public class RigidExplode : RigidUpdate
{
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

	private void Start()
	{
		interval = Random.Range(intervalMin, intervalMax);
	}

	public void Explode()
	{
		Vector2 position = rb.position;
		if ((bool)particle)
		{
			ParticleSystem p = Object.Instantiate(particle);
			p.transform.position = position;
			TweenUtil.Delay(5f, delegate
			{
				if ((bool)p && (bool)p.gameObject)
				{
					Object.DestroyImmediate(p.gameObject);
				}
			});
		}
		RaycastHit2D[] array = Physics2D.CircleCastAll(position, radius, Vector3.forward);
		foreach (RaycastHit2D raycastHit2D in array)
		{
			Rigidbody2D component = raycastHit2D.collider.GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				Vector2 vector = component.position - position;
				component.AddForce(vector * force * (1f + component.mass * massPower), ForceMode2D.Impulse);
			}
		}
		if (!sound.IsEmpty())
		{
			EMono.Sound.Play(sound, rb.position);
		}
	}

	public override void OnFixedUpdate()
	{
		time += RigidUpdate.delta;
		if (!(time > interval))
		{
			return;
		}
		if (chance >= Random.Range(0f, 1f))
		{
			Explode();
			if (destroy)
			{
				active = false;
				CollectibleActor component = base.gameObject.GetComponent<CollectibleActor>();
				if ((bool)component)
				{
					component.Deactivate();
				}
				else
				{
					base.gameObject.SetActive(value: false);
				}
				return;
			}
		}
		if (repeat)
		{
			time = 0f;
		}
		else
		{
			active = false;
		}
	}
}
