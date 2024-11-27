using System;
using System.Collections.Generic;
using System.Linq;

public class TraitMap : Dictionary<int, Trait>
{
	public void Add(Card c)
	{
		base.Add(c.uid, c.trait);
	}

	public void Remove(Card c)
	{
		base.Remove(c.uid);
	}

	public Card GetRandom()
	{
		Trait trait = this.RandomItem<int, Trait>();
		if (trait == null)
		{
			return null;
		}
		return trait.owner;
	}

	public Card GetRandom(Func<Trait, bool> func)
	{
		if (func == null)
		{
			return this.GetRandom();
		}
		Trait trait = base.Values.Where(func).RandomItem<Trait>();
		if (trait == null)
		{
			return null;
		}
		return trait.owner;
	}

	public Card GetRandomInstalled()
	{
		if (base.Count == 0)
		{
			return null;
		}
		Trait trait = (from a in base.Values
		where a.owner.placeState == PlaceState.installed
		select a).RandomItem<Trait>();
		if (trait == null)
		{
			return null;
		}
		return trait.owner;
	}

	public bool Contains(Card c)
	{
		return base.ContainsKey(c.uid);
	}
}
