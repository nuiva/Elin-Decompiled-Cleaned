using System;

public class Goal : AIAct
{
	public override bool InformCancel
	{
		get
		{
			return false;
		}
	}

	public Goal Duplicate()
	{
		return Activator.CreateInstance(base.GetType()) as Goal;
	}
}
