using System;

public class TraitLoytelMart : TraitVendingMachine
{
	public int LV
	{
		get
		{
			return EClass.player.flags.loytelMartLv;
		}
	}

	public override int ShopLv
	{
		get
		{
			return this.LV * 10 + ((this.LV > 0) ? 10 : 1);
		}
	}

	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override string IDInvStyle
	{
		get
		{
			return "default";
		}
	}

	public override bool CanBeOnlyBuiltInHome
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
			return ShopType.LoytelMart;
		}
	}

	public override int CostRerollShop
	{
		get
		{
			if (!EClass.debug.enable)
			{
				return 0;
			}
			return 1;
		}
	}

	public override bool CanUse(Chara c)
	{
		return this.owner.IsInstalled && EClass._zone.IsPCFaction;
	}

	public override void SetName(ref string s)
	{
		if (this.LV > 0)
		{
			s = s + Lang.space + "+" + this.LV.ToString();
		}
	}
}
