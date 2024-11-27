using System;

public class GrowSystemCactus : GrowSystemCrop
{
	protected override bool DrawHarvestOnTop
	{
		get
		{
			return true;
		}
	}

	protected override bool UseGenericFirstStageTile
	{
		get
		{
			return false;
		}
	}

	public override void OnMineObj(Chara c = null)
	{
		base.TryPick(GrowSystem.cell, "needle", -1, 1, false);
	}
}
