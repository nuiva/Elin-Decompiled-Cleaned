public class ConFear : BadCondition
{
	public override Emo2 EmoIcon => Emo2.fear;

	public override bool ConsumeTurn => !owner.IsPC;

	public override int GetPhase()
	{
		return 0;
	}

	public override void Tick()
	{
		Mod(-1);
		foreach (Condition condition in owner.conditions)
		{
			if (condition.ConsumeTurn && condition != this)
			{
				return;
			}
		}
		if (!owner.IsPC && !EClass._zone.IsRegion)
		{
			owner.TryMoveFrom((owner.enemy != null) ? owner.enemy.pos : EClass.pc.pos);
		}
	}
}
