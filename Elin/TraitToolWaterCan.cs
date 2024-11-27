using System;

public class TraitToolWaterCan : TraitTool
{
	public int MaxCharge
	{
		get
		{
			return this.owner.material.hardness / 5 + 4;
		}
	}

	public override bool HasCharges
	{
		get
		{
			return true;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.TrySetAct(new TaskWater
		{
			dest = p.pos
		}, this.owner))
		{
			return;
		}
		if (p.TrySetAct(new ActDrawWater
		{
			waterCan = this
		}, this.owner))
		{
			return;
		}
		p.TrySetAct(new ActWater
		{
			waterCan = this
		}, this.owner);
	}
}
