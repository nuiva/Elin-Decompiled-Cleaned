using System;

public class ConWeapon : BaseBuff
{
	public override bool IsElemental
	{
		get
		{
			return true;
		}
	}

	public override int P2
	{
		get
		{
			return this.owner.CHA;
		}
	}

	public override void Tick()
	{
	}
}
