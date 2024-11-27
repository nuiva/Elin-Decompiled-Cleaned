using System;

public class ActItem : Act
{
	public override bool CanPerform()
	{
		return this._act != null;
	}

	public int BuildAct(Chara c)
	{
		this._act = null;
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
						return this.ReturnAct(healAction, p);
					}
				}
			}
			return 0;
		}
		return 0;
	}

	public int ReturnAct(Action a, int p)
	{
		this._act = new DynamicAct("ActItem", delegate()
		{
			a();
			return true;
		}, false);
		return p;
	}

	public override bool Perform()
	{
		return this._act.Perform(Act.CC, null, null);
	}

	public Act _act;
}
