using System;

public class TraitStairsUp : TraitStairs
{
	public override ZoneTransition.EnterState enterState
	{
		get
		{
			return ZoneTransition.EnterState.Up;
		}
	}

	public override string langOnUse
	{
		get
		{
			return "stairsUp";
		}
	}

	public override bool IsUpstairs
	{
		get
		{
			return true;
		}
	}
}
