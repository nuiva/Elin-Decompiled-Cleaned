using System;

public class TraitShippingChest : TraitContainer
{
	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override bool IsSpecialContainer
	{
		get
		{
			return true;
		}
	}

	public override bool CanOpenContainer
	{
		get
		{
			return EClass._zone.IsPCFaction && this.owner.IsInstalled;
		}
	}

	public override bool ShowOpenActAsCrime
	{
		get
		{
			return false;
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
			return EClass.game.cards.container_shipping.things.Count > 0;
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
