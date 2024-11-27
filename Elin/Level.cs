using System;

public class Level : Element
{
	public override bool CanGainExp
	{
		get
		{
			return true;
		}
	}

	public override bool UsePotential
	{
		get
		{
			return false;
		}
	}

	public override bool UseExpMod
	{
		get
		{
			return false;
		}
	}

	public override int ExpToNext
	{
		get
		{
			return (100 + base.Value * 10) * (100 - this.owner.Value(403)) / 100;
		}
	}
}
