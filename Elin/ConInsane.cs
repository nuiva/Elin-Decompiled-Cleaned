using System;

public class ConInsane : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.insane;
		}
	}

	public override void Tick()
	{
		if (EClass.rnd(5) == 0)
		{
			if (EClass.rnd(5) == 0)
			{
				this.owner.Say("insane_act", this.owner, null, null);
			}
			else
			{
				this.owner.Talk("insane", null, null, false);
			}
		}
		if (EClass.rnd(5) == 0)
		{
			this.owner.AddCondition<ConConfuse>(EClass.rnd(100), false);
		}
		if (EClass.rnd(5) == 0)
		{
			this.owner.AddCondition<ConDim>(EClass.rnd(100), false);
		}
		if (EClass.rnd(5) == 0)
		{
			this.owner.AddCondition<ConFear>(EClass.rnd(100), false);
		}
		base.Mod(-1, false);
	}
}
