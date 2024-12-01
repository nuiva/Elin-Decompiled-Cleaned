public class TraitToolWaterCan : TraitTool
{
	public int MaxCharge => owner.material.hardness / 5 + 4;

	public override bool HasCharges => true;

	public override void TrySetHeldAct(ActPlan p)
	{
		if (!p.TrySetAct(new TaskWater
		{
			dest = p.pos
		}, owner) && !p.TrySetAct(new ActDrawWater
		{
			waterCan = this
		}, owner))
		{
			p.TrySetAct(new ActWater
			{
				waterCan = this
			}, owner);
		}
	}
}
