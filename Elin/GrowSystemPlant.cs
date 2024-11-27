using System;

public class GrowSystemPlant : GrowSystem
{
	public bool Bud
	{
		get
		{
			return base.stage.idx == 0;
		}
	}

	public bool Young
	{
		get
		{
			return base.stage.idx == 1;
		}
	}

	public bool Grown
	{
		get
		{
			return base.stage.idx == 2;
		}
	}

	public bool Mature
	{
		get
		{
			return base.stage.idx == 3;
		}
	}

	public bool Withered
	{
		get
		{
			return base.stage.idx == 4;
		}
	}

	public override void OnMineObj(Chara c = null)
	{
		base.TryPick(GrowSystem.cell, "grass", EClass.sources.materials.alias["grass"].id, EClass.rnd(2), false);
		base.OnMineObj(c);
	}
}
