using System;

public class TraitDeliveryChest : TraitContainer
{
	public override int GuidePriotiy
	{
		get
		{
			return 2;
		}
	}

	public override bool IsSpecialContainer
	{
		get
		{
			return false;
		}
	}

	public override bool ShowOpenActAsCrime
	{
		get
		{
			return false;
		}
	}

	public override bool CanOpenContainer
	{
		get
		{
			return this.owner.IsInstalled && (EClass._zone.IsPCFaction || EClass._zone.IsTown || EClass._zone is Zone_MerchantGuild || EClass._zone is Zone_Casino);
		}
	}

	public override bool ShowChildrenNumber
	{
		get
		{
			return false;
		}
	}

	public override bool UseAltTiles
	{
		get
		{
			return EClass.game.cards.container_deliver.things.Count > 0;
		}
	}

	public override void Prespawn(int lv)
	{
	}

	public override bool CanBeShipped
	{
		get
		{
			return false;
		}
	}
}
