public class ConInsane : BadCondition
{
	public override Emo2 EmoIcon => Emo2.insane;

	public override void Tick()
	{
		if (EClass.rnd(5) == 0)
		{
			if (EClass.rnd(5) == 0)
			{
				owner.Say("insane_act", owner);
			}
			else
			{
				owner.Talk("insane");
			}
		}
		if (EClass.rnd(5) == 0)
		{
			owner.AddCondition<ConConfuse>(EClass.rnd(100));
		}
		if (EClass.rnd(5) == 0)
		{
			owner.AddCondition<ConDim>(EClass.rnd(100));
		}
		if (EClass.rnd(5) == 0)
		{
			owner.AddCondition<ConFear>(EClass.rnd(100));
		}
		Mod(-1);
	}
}
