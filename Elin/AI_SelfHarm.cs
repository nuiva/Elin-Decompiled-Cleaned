using System;

public class AI_SelfHarm : AI_PassTime
{
	public override AI_PassTime.Type type
	{
		get
		{
			return AI_PassTime.Type.selfHarm;
		}
	}

	public override bool CancelWhenDamaged
	{
		get
		{
			return false;
		}
	}

	public override int exp
	{
		get
		{
			return 50;
		}
	}
}
