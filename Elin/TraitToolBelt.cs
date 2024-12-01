public class TraitToolBelt : TraitContainer
{
	public override bool CanOpenContainer => false;

	public override bool CanBeDestroyed => false;

	public override void Prespawn(int lv)
	{
		owner.AddCard(ThingGen.Create("purse"));
	}
}
