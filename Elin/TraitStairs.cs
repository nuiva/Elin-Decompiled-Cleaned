using System;

public class TraitStairs : TraitNewZone
{
	public override bool CanUseInTempDungeon
	{
		get
		{
			return true;
		}
	}

	public override bool CanToggleAutoEnter
	{
		get
		{
			return true;
		}
	}

	public override Point GetExitPos()
	{
		return new Point(this.owner.pos);
	}
}
