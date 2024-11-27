using System;

public class BaseBuff : BaseDebuff
{
	public override bool CanManualRemove
	{
		get
		{
			return true;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}
}
