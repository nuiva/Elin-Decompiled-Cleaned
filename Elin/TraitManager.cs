using System;
using System.Collections.Generic;

public class TraitManager : EClass
{
	public void OnAddCard(Card c)
	{
		Trait trait = c.trait;
		this.typeMap.GetOrCreate(trait.GetType(), null).Add(c);
		if (trait.IsAltar)
		{
			this.altars.Add(c);
		}
		if (trait.IsRestSpot)
		{
			this.restSpots.Add(c);
		}
		if (trait is TraitChair)
		{
			this.chairs.Add(c);
		}
		if (trait is TraitLightSun)
		{
			this.suns.Add(c);
		}
	}

	public void OnRemoveCard(Card c)
	{
		Trait trait = c.trait;
		this.typeMap[trait.GetType()].Remove(c);
		if (trait.IsAltar)
		{
			this.altars.Remove(c);
		}
		if (trait.IsRestSpot)
		{
			this.restSpots.Remove(c);
		}
		if (trait is TraitChair)
		{
			this.chairs.Remove(c);
		}
		if (trait is TraitLightSun)
		{
			this.suns.Remove(c);
		}
	}

	public Thing GetRandomThing<T>() where T : Trait
	{
		Card card = this.GetTraitSet<T>().RandomItem<int, Card>();
		if (card == null)
		{
			return null;
		}
		return card.Thing;
	}

	public List<T> List<T>(Func<T, bool> func = null) where T : Trait
	{
		TraitSet traitSet = this.GetTraitSet<T>();
		List<T> list = new List<T>();
		if (func == null)
		{
			using (Dictionary<int, Card>.ValueCollection.Enumerator enumerator = traitSet.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Card card = enumerator.Current;
					list.Add(card.trait as T);
				}
				return list;
			}
		}
		foreach (Card card2 in traitSet.Values)
		{
			if (func(card2.trait as T))
			{
				list.Add(card2.trait as T);
			}
		}
		return list;
	}

	public TraitSet GetTraitSet<T>() where T : Trait
	{
		return this.typeMap.GetOrCreate(typeof(T), null);
	}

	public TraitSet GetTraitSet(Type t)
	{
		return this.typeMap.GetOrCreate(t, null);
	}

	public Dictionary<Type, TraitSet> typeMap = new Dictionary<Type, TraitSet>();

	public TraitMap altars = new TraitMap();

	public TraitMap restSpots = new TraitMap();

	public TraitMap chairs = new TraitMap();

	public TraitMap suns = new TraitMap();
}
