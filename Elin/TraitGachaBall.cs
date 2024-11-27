using System;

public class TraitGachaBall : TraitItem
{
	public override string LangUse
	{
		get
		{
			return "actContainer";
		}
	}

	public override bool OnUse(Chara c)
	{
		TraitGacha.GachaType refVal = (TraitGacha.GachaType)this.owner.refVal;
		SpawnList spawnList;
		if (refVal != TraitGacha.GachaType.Plant)
		{
			if (refVal == TraitGacha.GachaType.Furniture)
			{
				spawnList = SpawnListThing.Get("gacha_furniture", (SourceThing.Row a) => a.value < 5000 && a.Category.IsChildOf("furniture"));
			}
			else
			{
				spawnList = SpawnListThing.Get("gacha_junk", (SourceThing.Row a) => a.Category.id == "junk");
			}
		}
		else
		{
			spawnList = SpawnListThing.Get("gacha_plant", (SourceThing.Row a) => a.Category.id == "plantpot");
		}
		Rand.SetSeed(this.owner.uid + this.owner.Num * 100 + EClass.world.date.day / 5 * 1000);
		Thing thing = ThingGen.Create(spawnList.Select(-1, -1).id, -1, -1);
		if (refVal != TraitGacha.GachaType.Plant)
		{
			thing.ChangeMaterial(EClass.sources.materials.rows.RandomItemWeighted((SourceMaterial.Row m) => (float)m.chance));
		}
		Rand.SetSeed(-1);
		EClass.player.DropReward(thing, false);
		this.owner.ModNum(-1, true);
		return true;
	}
}
