using System;

public class TraitMineEntrance : TraitStairsDown
{
	public override bool CanOnlyCarry
	{
		get
		{
			return true;
		}
	}

	public override bool IsEntrace
	{
		get
		{
			return true;
		}
	}

	public override bool ForceEnter
	{
		get
		{
			return true;
		}
	}
}
