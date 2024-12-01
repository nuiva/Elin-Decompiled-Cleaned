public class ConNightVision : BaseBuff
{
	public override void OnStartOrStack()
	{
		owner.RecalculateFOV();
	}

	public override void OnCalculateFov(Fov fov, ref int radius, ref float power)
	{
		if (radius < EClass.Colors.pcLights.cateye.radius)
		{
			radius = EClass.Colors.pcLights.cateye.radius;
		}
		float num = 0.01f * EClass.Colors.pcLights.cateye.color.a * 256f;
		if (power < num)
		{
			power = num;
		}
	}

	public override void OnRemoved()
	{
		owner.RecalculateFOV();
	}
}
