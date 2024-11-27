using System;

public class TraitKettle : TraitUniqueChara
{
	public override int CostRerollShop
	{
		get
		{
			return 0;
		}
	}

	public override bool CanInvest
	{
		get
		{
			return this.CanJoinParty;
		}
	}

	public override Trait.CopyShopType CopyShop
	{
		get
		{
			if (!this.CanJoinParty)
			{
				return Trait.CopyShopType.None;
			}
			return Trait.CopyShopType.Item;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			if (!this.CanJoinParty)
			{
				return ShopType.None;
			}
			return ShopType.Copy;
		}
	}

	public override PriceType PriceType
	{
		get
		{
			return PriceType.CopyShop;
		}
	}

	public override bool CanJoinParty
	{
		get
		{
			return EClass.game.quests.IsCompleted("vernis_gold") || EClass.debug.enable;
		}
	}

	public override bool CanBeBanished
	{
		get
		{
			return false;
		}
	}

	public override int RestockDay
	{
		get
		{
			return 30;
		}
	}

	public override bool CanCopy(Thing t)
	{
		return !t.noSell && !t.HasElement(1229, 1) && (t.trait is TraitSeed || t.isCrafted);
	}
}
