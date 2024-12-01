using System;
using System.Collections.Generic;
using System.Linq;

public class TraitSet : Dictionary<int, Card>
{
	public void Add(Card c)
	{
		Add(c.uid, c);
	}

	public void Remove(Card c)
	{
		Remove(c.uid);
	}

	public Trait GetRandom()
	{
		return this.RandomItem()?.trait;
	}

	public Trait GetRandom(Chara accessChara)
	{
		return GetRandom((Card t) => accessChara == null || accessChara.HasAccess(t));
	}

	public Trait GetRandom(Func<Card, bool> func)
	{
		if (func == null)
		{
			return GetRandom();
		}
		return base.Values.Where(func).RandomItem()?.trait;
	}

	public bool Contains(Card c)
	{
		return ContainsKey(c.uid);
	}
}
