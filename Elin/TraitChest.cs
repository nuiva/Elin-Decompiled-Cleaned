public class TraitChest : TraitContainer
{
	public override int ChanceLock => 25;

	public override void Prespawn(int lv)
	{
		ThingGen.CreateTreasureContent(owner.Thing, lv, TreasureType.RandomChest, clearContent: true);
	}
}
