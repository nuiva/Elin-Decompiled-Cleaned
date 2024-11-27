using System;

public class TraitStairsDown : TraitStairs
{
	public override ZoneTransition.EnterState enterState
	{
		get
		{
			return ZoneTransition.EnterState.Down;
		}
	}

	public override string langOnUse
	{
		get
		{
			return "stairsDown";
		}
	}

	public override bool IsDownstairs
	{
		get
		{
			return true;
		}
	}
}
