using System;

public class GoalEndTurn : Goal
{
	public override bool InformCancel
	{
		get
		{
			return false;
		}
	}

	public override bool CancelWhenDamaged
	{
		get
		{
			return false;
		}
	}
}
