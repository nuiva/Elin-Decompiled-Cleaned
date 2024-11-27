using System;

public class TargetTypeAny : TargetType
{
	public override bool RequireLos
	{
		get
		{
			return false;
		}
	}
}
