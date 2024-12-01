public class TraitShippingChest : TraitContainer
{
	public override bool IsHomeItem => true;

	public override bool IsSpecialContainer => true;

	public override bool CanOpenContainer
	{
		get
		{
			if (EClass._zone.IsPCFaction)
			{
				return owner.IsInstalled;
			}
			return false;
		}
	}

	public override bool ShowOpenActAsCrime => false;

	public override bool ShowChildrenNumber => false;

	public override bool UseAltTiles => EClass.game.cards.container_shipping.things.Count > 0;

	public override bool CanBeShipped => false;

	public override void Prespawn(int lv)
	{
	}
}
