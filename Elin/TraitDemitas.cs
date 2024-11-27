using System;
using UnityEngine;

public class TraitDemitas : TraitUniqueChara
{
	public override int CostRerollShop
	{
		get
		{
			return 0;
		}
	}

	public bool CanSpellwrite
	{
		get
		{
			return EClass.game.quests.IsCompleted("demitas_spellwriter");
		}
	}

	public override bool CanInvest
	{
		get
		{
			return this.CanSpellwrite;
		}
	}

	public override Trait.CopyShopType CopyShop
	{
		get
		{
			if (!this.CanSpellwrite)
			{
				return Trait.CopyShopType.None;
			}
			return Trait.CopyShopType.Spellbook;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			if (!this.CanSpellwrite)
			{
				return ShopType.None;
			}
			return ShopType.Copy;
		}
	}

	public override int NumCopyItem
	{
		get
		{
			return 3 + Mathf.Min(base.owner.c_invest / 5, 7);
		}
	}

	public override bool CanJoinParty
	{
		get
		{
			return EClass.debug.enable;
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
		return t.trait is TraitSpellbook;
	}
}
