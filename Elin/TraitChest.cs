using System;

public class TraitChest : TraitContainer
{
	public override int ChanceLock
	{
		get
		{
			return 25;
		}
	}

	public override void Prespawn(int lv)
	{
		ThingGen.CreateTreasureContent(this.owner.Thing, lv, TreasureType.RandomChest, true);
	}
}
