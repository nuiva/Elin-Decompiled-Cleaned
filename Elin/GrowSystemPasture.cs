using System;
using UnityEngine;

public class GrowSystemPasture : GrowSystemWheat
{
	public override int HarvestStage
	{
		get
		{
			return -1;
		}
	}

	public override bool CanReapSeed()
	{
		return base.stage.idx >= 1;
	}

	public override void OnMineObj(Chara c = null)
	{
		if (base.IsWithered() || base.stage.idx == 0)
		{
			base.OnMineObj(c);
			return;
		}
		int num = 1 + EClass.rnd(base.stage.idx);
		PlantData plantData = EClass._map.TryGetPlant(GrowSystem.cell);
		Thing thing = (plantData != null) ? plantData.seed : null;
		if (thing != null && thing.encLV > 1)
		{
			num += EClass.rndHalf((int)Mathf.Sqrt((float)thing.encLV) + 1);
		}
		base.PopHarvest(c, "pasture", num);
	}
}
