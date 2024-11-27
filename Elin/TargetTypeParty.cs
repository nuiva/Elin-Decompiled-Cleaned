using System;

public class TargetTypeParty : TargetTypeSelf
{
	public override TargetRange Range
	{
		get
		{
			return TargetRange.Party;
		}
	}

	public override int LimitDist
	{
		get
		{
			return 999;
		}
	}

	public override bool ForceParty
	{
		get
		{
			return true;
		}
	}
}
