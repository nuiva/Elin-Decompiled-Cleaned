using UnityEngine;

public class RigidLeaf : RigidUpdate
{
	public float floatPower = 0.6f;

	public float slidePower = 15f;

	private Vector2 v;

	private float flip = 1f;

	private float time;

	private float maxTime;

	private float f;

	private float power = 1f;

	private void Awake()
	{
		maxTime = Random.Range(0.5f, 1.5f);
	}

	public override void OnFixedUpdate()
	{
		_ = base.transform.position;
		if (base.transform.position.y < 3f)
		{
			power -= Time.fixedDeltaTime * 0.03f;
		}
		else
		{
			power = 1f;
		}
		if (!(power < 0f))
		{
			time += flip * 0.01f;
			if (time > maxTime)
			{
				maxTime = Random.Range(0.5f, 1.5f);
				time = maxTime;
				flip = -1f;
			}
			else if (time < 0f)
			{
				maxTime = Random.Range(0.5f, 1.5f);
				time = 0f;
				flip = 1f;
			}
			f = time - maxTime * 0.5f;
			v.x = f * slidePower * power;
			v.y = (10f - Mathf.Clamp(((f > 0f) ? f : (0f - f)) * 10f, 0f, 10f)) * floatPower * power;
			rb.AddForce(v);
		}
	}
}
