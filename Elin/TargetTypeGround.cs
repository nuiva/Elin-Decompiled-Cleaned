using System;

public class TargetTypeGround : TargetType
{
	public override TargetRange Range
	{
		get
		{
			return TargetRange.Ground;
		}
	}

	public override bool CanTargetGround
	{
		get
		{
			return true;
		}
	}
}
