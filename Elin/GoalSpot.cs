using System;
using System.Collections.Generic;

public class GoalSpot : Goal
{
	public override IEnumerable<AIAct.Status> Run()
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<TraitSpotSearch>().GetRandom();
		if (random != null)
		{
			yield return base.DoGoto(random.GetRandomPoint(null, null), 0, false, new Func<AIAct.Status>(this.Cancel));
			this.owner.SetAI(new GoalSearch());
		}
		yield break;
	}
}
