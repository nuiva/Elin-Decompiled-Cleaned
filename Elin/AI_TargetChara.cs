using System;

public class AI_TargetChara : AI_TargetCard
{
	public new Chara target
	{
		get
		{
			return this.target as Chara;
		}
		set
		{
			this.target = value;
		}
	}
}
