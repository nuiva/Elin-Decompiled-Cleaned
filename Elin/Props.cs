using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Props : EClass
{
	public virtual bool IsStocked
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsRoaming
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsInstalled
	{
		get
		{
			return false;
		}
	}

	public virtual PlaceState state
	{
		get
		{
			return PlaceState.roaming;
		}
	}

	public List<Thing> Things
	{
		get
		{
			return this.things;
		}
	}

	public int Count
	{
		get
		{
			return this.all.Count;
		}
	}

	public void Init()
	{
		if (this.categoryMap.Count > 0)
		{
			return;
		}
		foreach (SourceCategory.Row row in EClass.sources.categories.rows)
		{
			this.categoryMap.Add(row.id, new PropSetCategory
			{
				source = row
			});
		}
		foreach (PropSetCategory propSetCategory in this.categoryMap.Values)
		{
			if (propSetCategory.source.parent != null)
			{
				propSetCategory.parent = this.categoryMap[propSetCategory.source.parent.id];
			}
		}
	}

	public void Add(Card t)
	{
		if (t.props != null)
		{
			t.props.Remove(t);
		}
		t.props = this;
		t.placeState = this.state;
		if (t.isChara)
		{
			this.raceMap.GetOrCreate(t.Chara.race.id, null).Add(t);
			return;
		}
		foreach (Thing t2 in t.things)
		{
			if (t.placeState != PlaceState.roaming)
			{
				EClass._map.Stocked.Add(t2);
			}
		}
		if (this.all.Contains(t))
		{
			Debug.LogError(((t != null) ? t.ToString() : null) + " alreadin in " + ((this != null) ? this.ToString() : null));
			return;
		}
		this.weight += t.Num;
		this.all.Add(t);
		this.things.Add(t.Thing);
		this.cardMap.GetOrCreate(t.id, null).Add(t);
		if (t.sourceCard.origin != null)
		{
			this.cardMap.GetOrCreate(t.sourceCard.origin.id, null).Add(t);
		}
		this.categoryMap[t.category.id].Add(t);
		if (!t.Thing.source.workTag.IsEmpty())
		{
			this.workMap.GetOrCreate(t.Thing.source.workTag, null).Add(t);
		}
		if (t.IsContainer)
		{
			this.containers.Add(t.Thing);
		}
		this.traits.OnAddCard(t);
		if (t.isDeconstructing)
		{
			EClass._map.props.deconstructing.Add(t);
		}
	}

	public void Remove(Card t)
	{
		t.props = null;
		t.placeState = PlaceState.roaming;
		if (t.isChara)
		{
			this.raceMap[t.Chara.race.id].Remove(t);
			return;
		}
		foreach (Thing thing in t.things)
		{
			if (thing.props != null)
			{
				thing.props.Remove(thing);
			}
		}
		if (!this.all.Contains(t))
		{
			Debug.LogError(((t != null) ? t.ToString() : null) + " isn't in " + ((this != null) ? this.ToString() : null));
			return;
		}
		this.weight -= t.Num;
		this.all.Remove(t);
		this.things.Remove(t.Thing);
		this.cardMap[t.id].Remove(t);
		if (t.sourceCard.origin != null)
		{
			this.cardMap[t.sourceCard.origin.id].Remove(t);
		}
		this.categoryMap[t.category.id].Remove(t);
		if (!t.Thing.source.workTag.IsEmpty())
		{
			this.workMap[t.Thing.source.workTag].Remove(t);
		}
		if (t.IsContainer)
		{
			this.containers.Remove(t.Thing);
		}
		this.traits.OnRemoveCard(t);
		if (t.isDeconstructing)
		{
			EClass._map.props.deconstructing.Remove(t);
		}
	}

	public void OnNumChange(Card c, int a)
	{
		if (c.isChara)
		{
			return;
		}
		this.weight += a;
		this.categoryMap[c.category.id].OnChangeNum(a);
		this.cardMap[c.id].OnChangeNum(a);
		if (!c.Thing.source.workTag.IsEmpty())
		{
			this.workMap[c.Thing.source.workTag].OnChangeNum(a);
		}
	}

	public bool ShouldListAsResource(Thing t)
	{
		Card card = t.parent as Card;
		return card != null && !card.isSale && card.IsContainer && card.c_lockLv == 0 && !(card.trait is TraitChestMerchant) && !t.c_isImportant;
	}

	public Thing GetAvailableThing(string id, int idMat)
	{
		PropSet propSet = this.cardMap.TryGetValue(id, null);
		if (propSet == null)
		{
			return null;
		}
		foreach (Card card in propSet.Values)
		{
			if (card.idMaterial == idMat)
			{
				return card as Thing;
			}
		}
		return null;
	}

	public ThingStack ListThingStack(Recipe.Ingredient ing, StockSearchMode searchMode)
	{
		Props.<>c__DisplayClass28_0 CS$<>8__locals1 = new Props.<>c__DisplayClass28_0();
		CS$<>8__locals1.<>4__this = this;
		string id = ing.id;
		CS$<>8__locals1.idMat = -1;
		CS$<>8__locals1.tag = (ing.tag.IsEmpty() ? null : ing.tag);
		CS$<>8__locals1.stack = new ThingStack
		{
			val = CS$<>8__locals1.idMat
		};
		Room room = EClass.pc.pos.cell.room;
		if (ing.useCat)
		{
			CS$<>8__locals1.<ListThingStack>g__FindCat|0(id);
			foreach (string id2 in ing.idOther)
			{
				CS$<>8__locals1.<ListThingStack>g__FindCat|0(id2);
			}
			return CS$<>8__locals1.stack;
		}
		CS$<>8__locals1.<ListThingStack>g__Find|1(id);
		foreach (string id3 in ing.idOther)
		{
			CS$<>8__locals1.<ListThingStack>g__Find|1(id3);
		}
		return CS$<>8__locals1.stack;
	}

	public List<Thing> ListThingsInCategory(SourceCategory.Row cat)
	{
		List<Thing> list = new List<Thing>();
		foreach (Thing thing in this.Things)
		{
			if (thing.category.IsChildOf(cat))
			{
				list.Add(thing);
			}
		}
		return list;
	}

	public Dictionary<string, ThingStack> ListThingStacksInCategory(SourceCategory.Row cat)
	{
		Dictionary<string, ThingStack> dictionary = new Dictionary<string, ThingStack>();
		foreach (Thing t in this.Things)
		{
			this.ListThingStacksInCategory(cat, dictionary, t);
		}
		return dictionary;
	}

	private void ListThingStacksInCategory(SourceCategory.Row cat, Dictionary<string, ThingStack> stacks, Thing t)
	{
		if (EClass.sources.categories.map[t.source.category].IsChildOf(cat))
		{
			ThingStack thingStack = stacks.TryGetValue(t.id, null);
			if (thingStack == null)
			{
				thingStack = new ThingStack();
				stacks.Add(t.id, thingStack);
			}
			thingStack.count += t.Num;
			thingStack.list.Add(t);
		}
	}

	public Thing Find<T>() where T : Trait
	{
		foreach (Thing thing in this.Things)
		{
			if (thing.trait is T)
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindEmptyContainer<T>() where T : Trait
	{
		foreach (Thing thing in this.Things)
		{
			if (thing.trait is T && !thing.things.IsFull(0))
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindEmptyContainer<T>(Thing target) where T : Trait
	{
		foreach (Thing thing in this.Things)
		{
			if (thing.trait is T && !thing.things.IsFull(target, true, true))
			{
				return thing;
			}
		}
		return null;
	}

	public Thing Find(int uid)
	{
		foreach (Thing thing in this.Things)
		{
			if (thing.uid == uid)
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindShared(string id)
	{
		return this.Find(id, -1, -1, true);
	}

	public Thing Find(string id, string idMat)
	{
		return this.Find(id, idMat.IsEmpty() ? -1 : EClass.sources.materials.alias[idMat].id, -1, false);
	}

	public Thing Find(string id, int idMat = -1, int refVal = -1, bool shared = false)
	{
		PropSet propSet = this.cardMap.TryGetValue(id, null);
		Dictionary<int, Card>.ValueCollection valueCollection = (propSet != null) ? propSet.Values : null;
		if (valueCollection != null)
		{
			foreach (Card card in valueCollection)
			{
				if (shared)
				{
					Thing thing = card.parent as Thing;
					if (thing == null || !thing.IsSharedContainer)
					{
						continue;
					}
				}
				if ((idMat == -1 || card.material.id == idMat) && (refVal == -1 || card.refVal == refVal))
				{
					return card as Thing;
				}
			}
		}
		return null;
	}

	public int GetNum(string id, bool onlyShared = false)
	{
		int num = 0;
		foreach (Card card in this.cardMap.GetOrCreate(id, null).Values)
		{
			if (!onlyShared || (card.parentThing != null && card.parentThing.IsSharedContainer))
			{
				num += card.Num;
			}
		}
		return num;
	}

	public static int GetNumStockedAndRoaming(string id)
	{
		return EClass._map.Stocked.cardMap.GetOrCreate(id, null).num + EClass._map.Roaming.GetNum(id, false);
	}

	public void Validate()
	{
		foreach (KeyValuePair<string, PropSet> keyValuePair in this.cardMap)
		{
			int num = 0;
			foreach (Card card in keyValuePair.Value.Values)
			{
				num += card.Num;
			}
			if (num != keyValuePair.Value.num)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"prop num:",
					keyValuePair.Key,
					" ",
					keyValuePair.Value.num.ToString(),
					"/",
					num.ToString()
				}));
			}
		}
	}

	public PropSet all = new PropSet();

	public Dictionary<string, PropSet> cardMap = new Dictionary<string, PropSet>();

	public Dictionary<string, PropSetCategory> categoryMap = new Dictionary<string, PropSetCategory>();

	public Dictionary<string, PropSet> raceMap = new Dictionary<string, PropSet>();

	public Dictionary<string, PropSet> workMap = new Dictionary<string, PropSet>();

	public TraitManager traits = new TraitManager();

	public List<Thing> things = new List<Thing>();

	public List<Thing> containers = new List<Thing>();

	[JsonProperty]
	public int maxWeight = 100;

	public int weight;
}
