using System;

public class TraitVendingMachine : TraitItem
{
	public override string IDInvStyle
	{
		get
		{
			return "modern";
		}
	}

	public override bool ShowOrbit
	{
		get
		{
			return true;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.VMachine;
		}
	}

	public override bool AllowSell
	{
		get
		{
			return false;
		}
	}

	public override bool OnUse(Chara c)
	{
		base.OnBarter();
		EClass.ui.AddLayer(LayerInventory.CreateBuy(this.owner, c.trait.CurrencyType, c.trait.PriceType));
		return false;
	}
}
