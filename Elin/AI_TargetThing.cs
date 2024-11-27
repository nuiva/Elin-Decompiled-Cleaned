using System;

public class AI_TargetThing : AI_TargetCard
{
	public new Thing target
	{
		get
		{
			return this.target as Thing;
		}
		set
		{
			this.target = value;
		}
	}
}
