using System;

public class GrowSystemFlower : GrowSystemPlant
{
	public override int HarvestStage
	{
		get
		{
			return 3;
		}
	}

	public override bool CanReapSeed()
	{
		return base.stage.idx >= 2;
	}

	public override void OnMineObj(Chara c = null)
	{
		if (EClass.rnd(2) == 0)
		{
			base.TryPick(GrowSystem.cell, ThingGen.Create("grass", EClass.sources.materials.alias["grass"].id, -1), c, false);
		}
		if (base.Grown || base.Mature)
		{
			base.PopHarvest(c ?? EClass.pc, ThingGen.Create(this.idHarvestThing.IsEmpty("flower"), -1, -1), -1);
		}
	}
}
