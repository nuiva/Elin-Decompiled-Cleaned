using System;

public class TraitMagicChest : TraitContainer
{
	public override int Electricity
	{
		get
		{
			return base.Electricity + ((this.IsFridge ? 50 : 0) + this.owner.c_containerUpgrade.cap / 5) * -1;
		}
	}

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

	public override bool CanBeOnlyBuiltInHome
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

	public override bool IsFridge
	{
		get
		{
			return this.owner.c_containerUpgrade.cool > 0;
		}
	}

	public override bool UseAltTiles
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public override int DecaySpeedChild
	{
		get
		{
			if (!this.IsFridge || !this.owner.isOn)
			{
				return base.DecaySpeedChild;
			}
			return 0;
		}
	}

	public override bool CanSearchContents
	{
		get
		{
			return EClass.core.IsGameStarted && this.owner.IsInstalled && EClass._zone.IsPCFaction;
		}
	}

	public override void SetName(ref string s)
	{
		if (this.IsFridge)
		{
			s = "chest_fridge".lang(s, null, null, null, null);
		}
	}
}
