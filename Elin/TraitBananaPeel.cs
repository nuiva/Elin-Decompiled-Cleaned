using System;

public class TraitBananaPeel : TraitTrap
{
	public override bool IsNegativeEffect
	{
		get
		{
			return false;
		}
	}

	public override bool StartHidden
	{
		get
		{
			return false;
		}
	}

	public override bool CanDisarmTrap
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return true;
		}
	}
}
