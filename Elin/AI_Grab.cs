using System.Collections.Generic;

public class AI_Grab : AIAct
{
	public Card target;

	public int num = -1;

	public bool pickHeld;

	public virtual Card GetTarget()
	{
		return target;
	}

	public virtual bool IsValidTarget(Card c)
	{
		if (c != null)
		{
			return c == target;
		}
		return false;
	}

	public override IEnumerable<Status> Run()
	{
		if (IsValidTarget(owner.held))
		{
			yield return Success();
		}
		target = GetTarget();
		if (target == null)
		{
			yield return Cancel();
		}
		Card root = target.GetRootCard();
		if (!root.ExistsOnMap)
		{
			yield return Cancel();
		}
		yield return DoGoto(root, 1);
		if (!owner.TryHoldCard(target, (num == -1) ? target.Num : num))
		{
			yield return Cancel();
		}
	}
}
public class AI_Grab<T> : AI_Grab where T : Trait
{
	public override Card GetTarget()
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<T>().GetRandom();
		if (random == null)
		{
			random = EClass._map.Stocked.traits.GetTraitSet<T>().GetRandom();
		}
		return random?.owner;
	}

	public override bool IsValidTarget(Card c)
	{
		if (c != null)
		{
			return c.trait is T;
		}
		return false;
	}
}
