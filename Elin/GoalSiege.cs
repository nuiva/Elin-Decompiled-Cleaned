using System.Collections.Generic;

public class GoalSiege : Goal
{
	public Card target;

	public override IEnumerable<Status> Run()
	{
		if (target == null || !target.IsAliveInCurrentZone)
		{
			target = GetDestCard();
		}
		if (target != null)
		{
			yield return DoGoto(target);
			owner.DoHostileAction(target.Chara);
		}
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
}
