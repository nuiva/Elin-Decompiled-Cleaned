using System;

public class AI_Meditate : AI_PassTime
{
	public override AI_PassTime.Type type
	{
		get
		{
			return AI_PassTime.Type.meditate;
		}
	}

	public override int turns
	{
		get
		{
			return 300;
		}
	}

	public override int exp
	{
		get
		{
			return 10;
		}
	}
}
