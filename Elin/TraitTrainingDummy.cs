using System;

public class TraitTrainingDummy : Trait
{
	public override bool CanBeAttacked
	{
		get
		{
			return true;
		}
	}

	public override bool IdleUse(Chara c, int dist)
	{
		return (dist <= 1 && ACT.Melee.Perform(c, this.owner, null)) || (c.TryEquipRanged() && dist < c.ranged.range && ACT.Ranged.Perform(c, this.owner, null));
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!this.owner.IsInstalled)
		{
			return;
		}
		p.TrySetAct(new AI_PracticeDummy
		{
			target = this.owner.Thing
		}, this.owner);
	}
}
