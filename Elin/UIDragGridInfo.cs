using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIDragGridInfo : EMono
{
	private void Awake()
	{
		this.window.SetActive(false);
		this.transMold.SetActive(false);
	}

	public void Refresh()
	{
		this.Init(this.owner);
	}

	public void Init(Card _owner)
	{
		this.owner = _owner;
		TraitCrafter crafter = this.owner.trait as TraitCrafter;
		if (crafter == null)
		{
			return;
		}
		this.textHeader.text = "knownRecipe".lang();
		List<SourceRecipe.Row> recipes = (from r in EMono.sources.recipes.rows
		where r.factory == crafter.IdSource
		select r).ToList<SourceRecipe.Row>();
		if (recipes.Count == 0)
		{
			return;
		}
		BaseList baseList = this.list;
		UIList.Callback<SourceRecipe.Row, LayoutGroup> callback = new UIList.Callback<SourceRecipe.Row, LayoutGroup>();
		callback.onClick = delegate(SourceRecipe.Row a, LayoutGroup b)
		{
		};
		callback.onInstantiate = delegate(SourceRecipe.Row a, LayoutGroup b)
		{
			UIDragGridInfo.<>c__DisplayClass13_1 CS$<>8__locals2;
			CS$<>8__locals2.b = b;
			for (int i = 0; i < crafter.numIng; i++)
			{
				if (i != 0)
				{
					Util.Instantiate<Transform>(this.moldPlus, CS$<>8__locals2.b);
				}
				string[] array = (i == 0) ? a.ing1 : ((i == 1) ? a.ing2 : a.ing3);
				if (array.IsEmpty())
				{
					break;
				}
				foreach (string text in array)
				{
					if (text != array[0])
					{
						Util.Instantiate<Transform>(this.moldOr, CS$<>8__locals2.b);
					}
					base.<Init>g__AddThing|4(text, ref CS$<>8__locals2);
				}
			}
			Util.Instantiate<Transform>(this.moldEqual, CS$<>8__locals2.b);
			base.<Init>g__AddThing|4(a.thing, ref CS$<>8__locals2);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (SourceRecipe.Row row in recipes)
			{
				if (row.tag.Contains("known") || EMono.player.knownCraft.Contains(row.id) || EMono.debug.godCraft)
				{
					this.list.Add(row);
				}
			}
		};
		baseList.callbacks = callback;
		this.list.List(false);
		this.window.SetActive(true);
		this.window.RebuildLayout(true);
	}

	public void InitFuel(Card _owner)
	{
		this.owner = _owner;
		this.textHeader.text = "knownFuel".lang();
		List<SourceThing.Row> fuels = new List<SourceThing.Row>();
		foreach (SourceThing.Row row in EMono.sources.things.rows)
		{
			if (this.owner.trait.IsFuel(row.id))
			{
				fuels.Add(row);
			}
		}
		BaseList baseList = this.list;
		UIList.Callback<SourceThing.Row, LayoutGroup> callback = new UIList.Callback<SourceThing.Row, LayoutGroup>();
		callback.onClick = delegate(SourceThing.Row a, LayoutGroup b)
		{
		};
		callback.onInstantiate = delegate(SourceThing.Row a, LayoutGroup b)
		{
			UIDragGridInfo.<>c__DisplayClass14_1 CS$<>8__locals2;
			CS$<>8__locals2.b = b;
			base.<InitFuel>g__AddThing|3(a.id, ref CS$<>8__locals2);
			Util.Instantiate<Transform>(this.moldEqual, CS$<>8__locals2.b);
			base.<InitFuel>g__AddThing|3(this.owner.id, ref CS$<>8__locals2);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (SourceThing.Row o in fuels)
			{
				this.list.Add(o);
			}
		};
		baseList.callbacks = callback;
		this.list.List(false);
		this.window.SetActive(true);
		this.window.RebuildLayout(true);
	}

	public Window window;

	public UIText textHeader;

	public Transform transMold;

	public Transform moldThing;

	public Transform moldPlus;

	public Transform moldEqual;

	public Transform moldOr;

	public Transform moldCat;

	public Transform moldUnknown;

	public UIList list;

	public Card owner;
}
