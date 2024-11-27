using System;
using System.Collections.Generic;
using System.Linq;

public class TraitSet : Dictionary<int, Card>
{
	public void Add(Card c)
	{
		base.Add(c.uid, c);
	}

	public void Remove(Card c)
	{
		base.Remove(c.uid);
	}

	public Trait GetRandom()
	{
		Card card = this.RandomItem<int, Card>();
		if (card == null)
		{
			return null;
		}
		return card.trait;
	}

	public Trait GetRandom(Chara accessChara)
	{
		return this.GetRandom((Card t) => accessChara == null || accessChara.HasAccess(t));
	}

	public Trait GetRandom(Func<Card, bool> func)
	{
		if (func == null)
		{
			return this.GetRandom();
		}
		Card card = base.Values.Where(func).RandomItem<Card>();
		if (card == null)
		{
			return null;
		}
		return card.trait;
	}

	public bool Contains(Card c)
	{
		return base.ContainsKey(c.uid);
	}
}
