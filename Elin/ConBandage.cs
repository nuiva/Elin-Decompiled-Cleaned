using System;

public class ConBandage : ConHOT
{
	public override void OnStart()
	{
		base.OnStart();
		this.owner.CureCondition<ConBleed>(10);
	}
}
