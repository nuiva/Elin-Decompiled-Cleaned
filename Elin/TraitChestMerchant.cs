using System;

public class TraitChestMerchant : TraitContainer
{
	public override string IDInvStyle
	{
		get
		{
			if (EClass._zone is Zone_Casino)
			{
				return "casino";
			}
			if (EClass._zone.source.tag.Contains("tech"))
			{
				return "modern";
			}
			if (this.owner.GetRootCard().trait is TraitVendingMachine)
			{
				return this.owner.GetRootCard().trait.IDInvStyle;
			}
			return base.idContainer;
		}
	}

	public override void Prespawn(int lv)
	{
	}
}
