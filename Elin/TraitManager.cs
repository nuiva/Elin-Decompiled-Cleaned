using System;
using System.Collections.Generic;

public class TraitManager : EClass
{
	public Dictionary<Type, TraitSet> typeMap = new Dictionary<Type, TraitSet>();

	public TraitMap altars = new TraitMap();

	public TraitMap restSpots = new TraitMap();

	public TraitMap chairs = new TraitMap();

	public TraitMap suns = new TraitMap();

	public void OnAddCard(Card c)
	{
		Trait trait = c.trait;
		typeMap.GetOrCreate(trait.GetType()).Add(c);
		if (trait.IsAltar)
		{
			altars.Add(c);
		}
		if (trait.IsRestSpot)
		{
			restSpots.Add(c);
		}
		if (trait is TraitChair)
		{
			chairs.Add(c);
		}
		if (trait is TraitLightSun)
		{
			suns.Add(c);
		}
	}

	public void OnRemoveCard(Card c)
	{
		Trait trait = c.trait;
		typeMap[trait.GetType()].Remove(c);
		if (trait.IsAltar)
		{
			altars.Remove(c);
		}
		if (trait.IsRestSpot)
		{
			restSpots.Remove(c);
		}
		if (trait is TraitChair)
		{
			chairs.Remove(c);
		}
		if (trait is TraitLightSun)
		{
			suns.Remove(c);
		}
	}

	public Thing GetRandomThing<T>() where T : Trait
	{
		return GetTraitSet<T>().RandomItem()?.Thing;
	}

	public List<T> List<T>(Func<T, bool> func = null) where T : Trait
	{
		TraitSet traitSet = GetTraitSet<T>();
		List<T> list = new List<T>();
		if (func == null)
		{
			foreach (Card value in traitSet.Values)
			{
				list.Add(value.trait as T);
			}
		}
		else
		{
			foreach (Card value2 in traitSet.Values)
			{
				if (func(value2.trait as T))
				{
					list.Add(value2.trait as T);
				}
			}
		}
		return list;
	}

	public TraitSet GetTraitSet<T>() where T : Trait
	{
		return typeMap.GetOrCreate(typeof(T));
	}

	public TraitSet GetTraitSet(Type t)
	{
		return typeMap.GetOrCreate(t);
	}
}
