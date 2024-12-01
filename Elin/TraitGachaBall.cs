public class TraitGachaBall : TraitItem
{
	public override string LangUse => "actContainer";

	public override bool OnUse(Chara c)
	{
		TraitGacha.GachaType refVal = (TraitGacha.GachaType)owner.refVal;
		SpawnList spawnList = refVal switch
		{
			TraitGacha.GachaType.Furniture => SpawnListThing.Get("gacha_furniture", (SourceThing.Row a) => a.value < 5000 && a.Category.IsChildOf("furniture")), 
			TraitGacha.GachaType.Plant => SpawnListThing.Get("gacha_plant", (SourceThing.Row a) => a.Category.id == "plantpot"), 
			_ => SpawnListThing.Get("gacha_junk", (SourceThing.Row a) => a.Category.id == "junk"), 
		};
		Rand.SetSeed(owner.uid + owner.Num * 100 + EClass.world.date.day / 5 * 1000);
		Thing thing = ThingGen.Create(spawnList.Select().id);
		if (refVal != TraitGacha.GachaType.Plant)
		{
			thing.ChangeMaterial(EClass.sources.materials.rows.RandomItemWeighted((SourceMaterial.Row m) => m.chance));
		}
		Rand.SetSeed();
		EClass.player.DropReward(thing);
		owner.ModNum(-1);
		return true;
	}
}
