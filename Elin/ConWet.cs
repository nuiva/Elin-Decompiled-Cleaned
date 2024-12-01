public class ConWet : Condition
{
	public override bool ShouldRefresh => true;

	public override int GetPhase()
	{
		return 0;
	}

	public override void OnRefresh()
	{
		owner.isWet = true;
	}

	public override void Tick()
	{
		if (base.value > 100)
		{
			base.value = 100;
		}
		if ((!owner.Cell.IsTopWaterAndNoSnow && !owner.Cell.HasLiquid) || owner.IsLevitating)
		{
			Mod(-1);
		}
	}
}
