using System;

public class TraitBath : Trait
{
	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct(new AI_Bladder
		{
			toilet = this
		}, this.owner);
	}
}
