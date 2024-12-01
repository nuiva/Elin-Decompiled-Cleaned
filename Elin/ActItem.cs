using System;

public class ActItem : Act
{
	public Act _act;

	public override bool CanPerform()
	{
		return _act != null;
	}

	public int BuildAct(Chara c)
	{
		_act = null;
		int p = 110 - 110 * c.hp / c.MaxHP;
		if (c.hp < c.MaxHP * 2 / 3)
		{
			foreach (Thing thing in c.things)
			{
				if (thing.IsIdentified)
				{
					Action healAction = thing.trait.GetHealAction(c);
					if (healAction != null)
					{
						return ReturnAct(healAction, p);
					}
				}
			}
		}
		return 0;
	}

	public int ReturnAct(Action a, int p)
	{
		_act = new DynamicAct("ActItem", delegate
		{
			a();
			return true;
		});
		return p;
	}

	public override bool Perform()
	{
		return _act.Perform(Act.CC);
	}
}
