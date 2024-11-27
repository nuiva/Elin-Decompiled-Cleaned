using System;

public class TraitMerchant : TraitCitizen
{
	public override int GuidePriotiy
	{
		get
		{
			return 10;
		}
	}

	public override string IDRumor
	{
		get
		{
			return "shopkeeper";
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Goods;
		}
	}

	public override bool CanInvest
	{
		get
		{
			return true;
		}
	}

	public override void OnCreate(int lv)
	{
		base.owner.c_invest = EClass.rnd(10);
	}
}
