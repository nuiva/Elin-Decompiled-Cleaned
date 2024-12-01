using System;
using System.Collections.Generic;
using System.Linq;

public class TraitMap : Dictionary<int, Trait>
{
	public void Add(Card c)
	{
		Add(c.uid, c.trait);
	}

	public void Remove(Card c)
	{
		Remove(c.uid);
	}

	public Card GetRandom()
	{
		return this.RandomItem()?.owner;
	}

	public Card GetRandom(Func<Trait, bool> func)
	{
		if (func == null)
		{
			return GetRandom();
		}
		return base.Values.Where(func).RandomItem()?.owner;
	}

	public Card GetRandomInstalled()
	{
		if (base.Count == 0)
		{
			return null;
		}
		return base.Values.Where((Trait a) => a.owner.placeState == PlaceState.installed).RandomItem()?.owner;
	}

	public bool Contains(Card c)
	{
		return ContainsKey(c.uid);
	}
}
