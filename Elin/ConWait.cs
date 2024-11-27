using System;

public class ConWait : Condition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override bool ConsumeTurn
	{
		get
		{
			return true;
		}
	}

	public override bool CancelAI
	{
		get
		{
			return false;
		}
	}

	public override bool WillOverride
	{
		get
		{
			return true;
		}
	}
}
