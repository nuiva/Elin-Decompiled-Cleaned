using System;

public class AI_Grab<T> : AI_Grab where T : Trait
{
	public override Card GetTarget()
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<T>().GetRandom();
		if (random == null)
		{
			random = EClass._map.Stocked.traits.GetTraitSet<T>().GetRandom();
		}
		if (random == null)
		{
			return null;
		}
		return random.owner;
	}

	public override bool IsValidTarget(Card c)
	{
		return c != null && c.trait is T;
	}
}
