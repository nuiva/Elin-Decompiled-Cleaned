using System;

public class GrowSystemBerry : GrowSystemCrop
{
	protected override bool DrawHarvestOnTop
	{
		get
		{
			return true;
		}
	}

	public override void OnMineObj(Chara c = null)
	{
		base.TryPick(GrowSystem.cell, "grass", EClass.sources.materials.alias["grass_forest"].id, EClass.rnd(2), false);
		base.OnMineObj(c);
	}
}
