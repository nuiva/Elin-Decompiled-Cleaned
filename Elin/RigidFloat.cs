using UnityEngine;

public class RigidFloat : RigidUpdate
{
	public float airHeight = 3.5f;

	public float floatPower = 30f;

	public float pingpongTime = 1f;

	public float pingpongPower = 0.15f;

	private float _airHeight;

	private float time;

	private float flip = 1f;

	private void Awake()
	{
		_airHeight = airHeight + Random.Range(-2f, 2f);
		time = Random.Range(0f, pingpongTime);
	}

	public override void OnFixedUpdate()
	{
		time += Time.fixedDeltaTime * flip;
		if (time > pingpongTime)
		{
			flip = -1f;
			time = pingpongTime;
		}
		else if (time < 0f)
		{
			flip = 1f;
			time = 0f;
		}
		float num = time - pingpongTime * 0.5f;
		num = ((!(num > 0f)) ? (Mathf.Sqrt(0f - num) * -1f) : Mathf.Sqrt(num));
		float num2 = _airHeight + num * pingpongPower;
		float num3 = base.transform.position.y;
		if (num3 < num2)
		{
			if (num3 < 1f)
			{
				num3 = 1f;
			}
			rb.AddForce(new Vector2(0f, floatPower * (1f - num3 / num2)));
		}
	}
}
