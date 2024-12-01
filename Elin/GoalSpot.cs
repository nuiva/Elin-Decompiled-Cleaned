using System.Collections.Generic;

public class GoalSpot : Goal
{
	public override IEnumerable<Status> Run()
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<TraitSpotSearch>().GetRandom();
		if (random != null)
		{
			yield return DoGoto(random.GetRandomPoint(), 0, ignoreConnection: false, Cancel);
			owner.SetAI(new GoalSearch());
		}
	}
}
