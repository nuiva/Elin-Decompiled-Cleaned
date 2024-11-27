using System;

public class ConDim : BadCondition
{
	public override bool ConsumeTurn
	{
		get
		{
			return this.GetPhase() >= 1;
		}
	}
}
