using System;

public class GrowSystemWeed : GrowSystem
{
	protected override bool UseGenericFirstStageTile
	{
		get
		{
			return false;
		}
	}

	public override bool CanReapSeed()
	{
		return base.stage.idx >= 2;
	}

	public override int HarvestStage
	{
		get
		{
			return 3;
		}
	}

	public override int AutoMineStage
	{
		get
		{
			return 2;
		}
	}

	public override void OnMineObj(Chara c = null)
	{
		base.TryPick(GrowSystem.cell, "grass", EClass.sources.materials.alias["grass"].id, EClass.rnd(5), false);
		base.OnMineObj(c);
	}
}
