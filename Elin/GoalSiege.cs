using System;
using System.Collections.Generic;

public class GoalSiege : Goal
{
	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.target == null || !this.target.IsAliveInCurrentZone)
		{
			this.target = this.GetDestCard();
		}
		if (this.target != null)
		{
			yield return base.DoGoto(this.target, null);
			this.owner.DoHostileAction(this.target.Chara, false);
		}
		yield break;
	}

	public Card GetDestCard()
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.faction == EClass.Home && chara.IsAliveInCurrentZone)
			{
				return chara;
			}
		}
		return null;
	}

	public Card target;
}
