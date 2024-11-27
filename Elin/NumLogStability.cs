using System;

public class NumLogStability : NumLog
{
	public override string Name
	{
		get
		{
			return Lang.Get("stability");
		}
	}

	public override Gross gross
	{
		get
		{
			return EClass.Branch.stability;
		}
	}
}
