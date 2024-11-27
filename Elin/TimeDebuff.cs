using System;

public class TimeDebuff : BaseDebuff
{
	public override bool TimeBased
	{
		get
		{
			return true;
		}
	}

	public override bool CanStack(Condition c)
	{
		return !c.givenByPcParty || c.givenByPcParty == base.givenByPcParty;
	}
}
