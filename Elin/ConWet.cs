using System;

public class ConWet : Condition
{
	public override bool ShouldRefresh
	{
		get
		{
			return true;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override void OnRefresh()
	{
		this.owner.isWet = true;
	}

	public override void Tick()
	{
		if (base.value > 100)
		{
			base.value = 100;
		}
		if ((!this.owner.Cell.IsTopWaterAndNoSnow && !this.owner.Cell.HasLiquid) || this.owner.IsLevitating)
		{
			base.Mod(-1, false);
		}
	}
}
