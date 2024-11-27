using System;

public class ConFear : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.fear;
		}
	}

	public override bool ConsumeTurn
	{
		get
		{
			return !this.owner.IsPC;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override void Tick()
	{
		base.Mod(-1, false);
		foreach (Condition condition in this.owner.conditions)
		{
			if (condition.ConsumeTurn && condition != this)
			{
				return;
			}
		}
		if (!this.owner.IsPC && !EClass._zone.IsRegion)
		{
			this.owner.TryMoveFrom((this.owner.enemy != null) ? this.owner.enemy.pos : EClass.pc.pos);
		}
	}
}
