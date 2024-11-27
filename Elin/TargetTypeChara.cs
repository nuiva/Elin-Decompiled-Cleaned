using System;

public class TargetTypeChara : TargetType
{
	public override TargetRange Range
	{
		get
		{
			return TargetRange.Chara;
		}
	}

	public override bool RequireChara
	{
		get
		{
			return true;
		}
	}
}
