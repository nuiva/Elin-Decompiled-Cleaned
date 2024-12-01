public class TimeDebuff : BaseDebuff
{
	public override bool TimeBased => true;

	public override bool CanStack(Condition c)
	{
		if (c.givenByPcParty)
		{
			return c.givenByPcParty == base.givenByPcParty;
		}
		return true;
	}
}
