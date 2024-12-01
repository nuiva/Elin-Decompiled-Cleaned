public class TraitContainerCurrency : TraitContainer
{
	public override InvGridSize InvGridSize => InvGridSize.Purse;

	public override ContainerType ContainerType => ContainerType.Currency;

	public override void Prespawn(int lv)
	{
		owner.AddCard(ThingGen.Create("money").SetNum(3 + EClass.rnd(5)));
	}
}
