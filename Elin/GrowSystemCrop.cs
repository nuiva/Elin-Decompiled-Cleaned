using System;

public class GrowSystemCrop : GrowSystem
{
	public override bool IsCrimeToHarvest
	{
		get
		{
			return true;
		}
	}

	public override int HarvestStage
	{
		get
		{
			return 3;
		}
	}

	public override void OnMineObj(Chara c = null)
	{
		if (this.source.alias == "grape" && (base.stage.idx == 2 || base.stage.idx == 3))
		{
			base.TryPick(GrowSystem.cell, "vine", -1, 1, false);
		}
		base.OnMineObj(c);
	}
}
