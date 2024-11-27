using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

public class ThingContainer : List<Thing>
{
	[JsonIgnore]
	public int GridSize
	{
		get
		{
			return this.width * this.height;
		}
	}

	[JsonIgnore]
	public bool HasGrid
	{
		get
		{
			return this.grid != null;
		}
	}

	[JsonIgnore]
	public bool IsMagicChest
	{
		get
		{
			return this.owner.trait is TraitMagicChest;
		}
	}

	[JsonIgnore]
	public int MaxCapacity
	{
		get
		{
			if (!this.IsMagicChest)
			{
				return this.GridSize;
			}
			return 100 + this.owner.c_containerUpgrade.cap;
		}
	}

	public void SetOwner(Card owner)
	{
		this.owner = owner;
		this.width = owner.c_containerSize / 100;
		this.height = owner.c_containerSize % 100;
		if (this.width == 0)
		{
			this.width = 8;
			this.height = 5;
		}
	}

	public void ChangeSize(int w, int h)
	{
		this.width = w;
		this.height = h;
		this.owner.c_containerSize = w * 100 + h;
		this.RefreshGrid();
		Debug.Log(string.Concat(new string[]
		{
			base.Count.ToString(),
			"/",
			this.width.ToString(),
			"/",
			this.height.ToString(),
			"/",
			this.GridSize.ToString()
		}));
	}

	public void RefreshGridRecursive()
	{
		foreach (Thing thing in this)
		{
			if (thing.IsContainer)
			{
				thing.things.RefreshGridRecursive();
			}
		}
		this.RefreshGrid();
	}

	public void RefreshGrid()
	{
		if (this.GridSize == 0)
		{
			return;
		}
		this.grid = new List<Thing>(new Thing[this.GridSize]);
		foreach (Thing thing in this)
		{
			if (this.ShouldShowOnGrid(thing))
			{
				if (thing.invX >= this.GridSize || thing.invX < 0 || this.grid[thing.invX] != null)
				{
					ThingContainer.listUnassigned.Add(thing);
				}
				else
				{
					this.grid[thing.invX] = thing;
				}
			}
		}
		foreach (Thing thing2 in ThingContainer.listUnassigned)
		{
			int freeGridIndex = this.GetFreeGridIndex();
			if (freeGridIndex == -1)
			{
				break;
			}
			this.grid[freeGridIndex] = thing2;
			thing2.invX = freeGridIndex;
		}
		ThingContainer.listUnassigned.Clear();
	}

	public void RefreshGrid(UIMagicChest magic, Window.SaveData data)
	{
		magic.filteredList.Clear();
		magic.cats.Clear();
		magic.catCount.Clear();
		this.grid = new List<Thing>(new Thing[this.GridSize]);
		string lastSearch = magic.lastSearch;
		bool flag = !lastSearch.IsEmpty();
		bool flag2 = !magic.idCat.IsEmpty();
		Window.SaveData.CategoryType category = data.category;
		bool flag3 = category != Window.SaveData.CategoryType.None;
		string text = "";
		foreach (Thing thing in this)
		{
			if (flag3)
			{
				switch (category)
				{
				case Window.SaveData.CategoryType.Main:
					text = thing.category.GetRoot().id;
					break;
				case Window.SaveData.CategoryType.Sub:
					text = thing.category.GetSecondRoot().id;
					break;
				case Window.SaveData.CategoryType.Exact:
					text = thing.category.id;
					break;
				}
				magic.cats.Add(text);
				if (magic.catCount.ContainsKey(text))
				{
					Dictionary<string, int> catCount = magic.catCount;
					string key = text;
					int num = catCount[key];
					catCount[key] = num + 1;
				}
				else
				{
					magic.catCount.Add(text, 1);
				}
			}
			if (flag)
			{
				if (thing.tempName == null)
				{
					thing.tempName = thing.GetName(NameStyle.Full, 1).ToLower();
				}
				if (!thing.tempName.Contains(lastSearch) && !thing.source.GetSearchName(false).Contains(lastSearch) && !thing.source.GetSearchName(true).Contains(lastSearch))
				{
					continue;
				}
			}
			if (!flag2 || !(text != magic.idCat))
			{
				magic.filteredList.Add(thing);
			}
		}
		if (flag2 && !magic.cats.Contains(magic.idCat))
		{
			magic.idCat = "";
			this.RefreshGrid(magic, data);
			return;
		}
		magic.pageMax = (magic.filteredList.Count - 1) / this.GridSize;
		if (magic.page > magic.pageMax)
		{
			magic.page = magic.pageMax;
		}
		for (int i = 0; i < this.GridSize; i++)
		{
			int num2 = magic.page * this.GridSize + i;
			if (num2 >= magic.filteredList.Count)
			{
				break;
			}
			Thing thing2 = magic.filteredList[num2];
			this.grid[i] = thing2;
			thing2.invX = i;
		}
		magic.RefreshCats();
	}

	public bool IsOccupied(int x, int y)
	{
		foreach (Thing thing in this)
		{
			if (thing.invY == y && thing.invX == x)
			{
				return true;
			}
		}
		return false;
	}

	public bool ShouldShowOnGrid(Thing t)
	{
		if (!this.owner.IsPC)
		{
			return !(t.trait is TraitChestMerchant);
		}
		return !t.isEquipped && !t.IsHotItem;
	}

	public void OnAdd(Thing t)
	{
		if (!this.HasGrid)
		{
			return;
		}
		if (!this.ShouldShowOnGrid(t))
		{
			return;
		}
		int freeGridIndex = this.GetFreeGridIndex();
		if (freeGridIndex != -1)
		{
			this.grid[freeGridIndex] = t;
		}
		t.pos.x = freeGridIndex;
	}

	public bool IsFull(int y = 0)
	{
		if (this.IsMagicChest)
		{
			return base.Count >= this.MaxCapacity || (this.owner.trait.IsFridge && !this.owner.isOn);
		}
		if (y != 0)
		{
			return false;
		}
		if (this.owner.trait.IsSpecialContainer && this.owner.parent != null)
		{
			return true;
		}
		if (!this.HasGrid)
		{
			return base.Count >= this.GridSize;
		}
		return this.GetFreeGridIndex() == -1;
	}

	public bool IsOverflowing()
	{
		if (!this.HasGrid || this.IsMagicChest)
		{
			return false;
		}
		int num = 0;
		foreach (Thing thing in this)
		{
			if (thing.invY != 1 && !thing.isEquipped)
			{
				num++;
			}
		}
		return num > this.grid.Count;
	}

	public int GetFreeGridIndex()
	{
		for (int i = 0; i < this.grid.Count; i++)
		{
			if (this.grid[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	public void OnRemove(Thing t)
	{
		if (!this.HasGrid || t.invY != 0)
		{
			return;
		}
		int num = this.grid.IndexOf(t);
		if (num != -1)
		{
			this.grid[num] = null;
		}
	}

	public void SetSize(int w, int h)
	{
		this.owner.c_containerSize = w * 100 + h;
		this.SetOwner(this.owner);
	}

	public Thing TryStack(Thing target, int destInvX = -1, int destInvY = -1)
	{
		foreach (Thing thing in this)
		{
			if (destInvX == -1 && thing.CanSearchContents && this.owner.GetRootCard().IsPC)
			{
				Thing thing2 = thing.things.TryStack(target, destInvX, destInvY);
				if (thing2 != target)
				{
					return thing2;
				}
			}
			if ((destInvX == -1 || (thing.invX == destInvX && thing.invY == destInvY)) && thing != target && target.TryStackTo(thing))
			{
				return thing;
			}
		}
		return target;
	}

	public Thing CanStack(Thing target, int destInvX = -1, int destInvY = -1)
	{
		foreach (Thing thing in this)
		{
			if (thing != target && target.CanStackTo(thing))
			{
				return thing;
			}
		}
		return target;
	}

	public ThingContainer.DestData GetDest(Thing t, bool tryStack = true)
	{
		ThingContainer.<>c__DisplayClass31_0 CS$<>8__locals1;
		CS$<>8__locals1.t = t;
		CS$<>8__locals1.tryStack = tryStack;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.d = default(ThingContainer.DestData);
		if (!this.owner.IsPC)
		{
			this.<GetDest>g__SearchDest|31_0(this, true, true, ref CS$<>8__locals1);
			return CS$<>8__locals1.d;
		}
		CS$<>8__locals1.flag = CS$<>8__locals1.t.category.GetRoot().id.ToEnum(true);
		if (CS$<>8__locals1.flag == ContainerFlag.none)
		{
			CS$<>8__locals1.flag = ContainerFlag.other;
		}
		ThingContainer._listContainers.Clear();
		ThingContainer._listContainers.Add(this);
		this.<GetDest>g__TrySearchContainer|31_1(this.owner, ref CS$<>8__locals1);
		ThingContainer._listContainers.Sort(delegate(ThingContainer a, ThingContainer b)
		{
			Window.SaveData windowSaveData = b.owner.GetWindowSaveData();
			int num = ((windowSaveData != null) ? windowSaveData.priority : 0) * 10 + (b.owner.IsPC ? 1 : 0);
			Window.SaveData windowSaveData2 = a.owner.GetWindowSaveData();
			return num - (((windowSaveData2 != null) ? windowSaveData2.priority : 0) * 10 + (a.owner.IsPC ? 1 : 0));
		});
		foreach (ThingContainer things in ThingContainer._listContainers)
		{
			this.<GetDest>g__SearchDest|31_0(things, false, true, ref CS$<>8__locals1);
			if (CS$<>8__locals1.d.IsValid)
			{
				return CS$<>8__locals1.d;
			}
		}
		foreach (ThingContainer things2 in ThingContainer._listContainers)
		{
			this.<GetDest>g__SearchDest|31_0(things2, true, false, ref CS$<>8__locals1);
			if (CS$<>8__locals1.d.IsValid)
			{
				return CS$<>8__locals1.d;
			}
		}
		return CS$<>8__locals1.d;
	}

	public bool IsFull(Thing t, bool recursive = true, bool tryStack = true)
	{
		return this.IsFull(0) && (!tryStack || this.CanStack(t, -1, -1) == t) && (!recursive || !this.GetDest(t, tryStack).IsValid);
	}

	public void AddCurrency(Card owner, string id, int a, SourceMaterial.Row mat = null)
	{
		int num = a;
		this.ListCurrency(id);
		foreach (Thing thing in ThingContainer.tempList)
		{
			if (!(thing.id != id) && (mat == null || thing.material == mat))
			{
				if (num > 0)
				{
					thing.ModNum(num, true);
					return;
				}
				if (thing.Num + num >= 0)
				{
					thing.ModNum(num, true);
					return;
				}
				num += thing.Num;
				thing.ModNum(-thing.Num, true);
			}
		}
		if (num == 0)
		{
			return;
		}
		if (num > 0)
		{
			Thing thing2 = ThingGen.Create(id, -1, -1);
			if (mat != null)
			{
				thing2.ChangeMaterial(mat);
			}
			owner.AddThing(thing2, true, -1, -1).SetNum(num);
			return;
		}
	}

	public void DestroyAll(Func<Thing, bool> funcExclude = null)
	{
		this.ForeachReverse(delegate(Thing t)
		{
			if (funcExclude != null && funcExclude(t))
			{
				return;
			}
			t.Destroy();
			this.Remove(t);
		});
		if (this.grid != null)
		{
			for (int i = 0; i < this.grid.Count; i++)
			{
				this.grid[i] = null;
			}
		}
	}

	public Thing Find(int uid)
	{
		foreach (Thing thing in this)
		{
			if (thing.CanSearchContents)
			{
				Thing thing2 = thing.things.Find(uid);
				if (thing2 != null)
				{
					return thing2;
				}
			}
			if (thing.uid == uid)
			{
				return thing;
			}
		}
		return null;
	}

	public Thing Find<T>() where T : Trait
	{
		foreach (Thing thing in this)
		{
			if (thing.CanSearchContents)
			{
				Thing thing2 = thing.things.Find<T>();
				if (thing2 != null)
				{
					return thing2;
				}
			}
			if (thing.trait is T)
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindBest<T>(Func<Thing, int> func) where T : Trait
	{
		this.List((Thing t) => t.trait is T, true);
		if (ThingContainer.tempList.Count == 0)
		{
			return null;
		}
		ThingContainer.tempList.Sort((Thing a, Thing b) => func(b) - func(a));
		return ThingContainer.tempList[0];
	}

	public Thing Find(Func<Thing, bool> func, bool recursive = true)
	{
		foreach (Thing thing in this)
		{
			if (recursive && thing.CanSearchContents)
			{
				Thing thing2 = thing.things.Find(func, true);
				if (thing2 != null)
				{
					return thing2;
				}
			}
			if (func(thing))
			{
				return thing;
			}
		}
		return null;
	}

	public Thing Find(string id, string idMat)
	{
		return this.Find(id, idMat.IsEmpty() ? -1 : EClass.sources.materials.alias[idMat].id, -1);
	}

	public Thing Find(string id, int idMat = -1, int refVal = -1)
	{
		foreach (Thing thing in this)
		{
			if (thing.CanSearchContents)
			{
				Thing thing2 = thing.things.Find(id, idMat, refVal);
				if (thing2 != null)
				{
					return thing2;
				}
			}
			if (thing.id == id && (idMat == -1 || thing.material.id == idMat) && (refVal == -1 || thing.refVal == refVal))
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindStealable()
	{
		List<Thing> list = new List<Thing>();
		foreach (Thing thing in this)
		{
			if (!thing.IsContainer && thing.trait.CanBeStolen && !thing.HasTag(CTAG.gift))
			{
				list.Add(thing);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.Sort((Thing a, Thing b) => ThingContainer.<FindStealable>g__Compare|41_0(a) - ThingContainer.<FindStealable>g__Compare|41_0(b));
		return list[0];
	}

	public ThingStack GetThingStack(string id, int refVal = -1)
	{
		ThingStack s = new ThingStack();
		bool isOrigin = EClass.sources.cards.map[id].isOrigin;
		return this.GetThingStack(id, s, isOrigin, refVal);
	}

	public ThingStack GetThingStack(string id, ThingStack s, bool isOrigin, int refVal = -1)
	{
		foreach (Thing thing in this)
		{
			if (thing.CanSearchContents)
			{
				thing.things.GetThingStack(id, s, isOrigin, refVal);
			}
			if ((refVal == -1 || thing.refVal == refVal) && thing.IsIdentified && (thing.id == id || (isOrigin && thing.source._origin == id)))
			{
				s.Add(thing);
			}
		}
		return s;
	}

	public ThingStack GetThingStack(string id)
	{
		ThingStack s = new ThingStack();
		bool isOrigin = EClass.sources.cards.map[id].isOrigin;
		return this.GetThingStack(id, s, isOrigin);
	}

	public ThingStack GetThingStack(string id, ThingStack s, bool isOrigin)
	{
		foreach (Thing thing in this)
		{
			if (thing.CanSearchContents)
			{
				thing.things.GetThingStack(id, s, isOrigin);
			}
			if (thing.id == id || (isOrigin && thing.source._origin == id))
			{
				s.Add(thing);
			}
		}
		return s;
	}

	public int GetCurrency(string id, ref int sum, SourceMaterial.Row mat = null)
	{
		foreach (Thing thing in this)
		{
			if (thing.CanSearchContents)
			{
				thing.things.GetCurrency(id, ref sum, mat);
			}
			if (thing.id == id && (mat == null || thing.material == mat))
			{
				sum += thing.Num;
			}
		}
		return sum;
	}

	public List<Thing> ListCurrency(string id)
	{
		ThingContainer.tempList.Clear();
		this._ListCurrency(id);
		return ThingContainer.tempList;
	}

	private void _ListCurrency(string id)
	{
		foreach (Thing thing in this)
		{
			if (thing.CanSearchContents)
			{
				thing.things._ListCurrency(id);
			}
			if (thing.id == id)
			{
				ThingContainer.tempList.Add(thing);
			}
		}
	}

	public List<Thing> List(Func<Thing, bool> func, bool onlyAccessible = false)
	{
		ThingContainer.tempList.Clear();
		this._List(func, onlyAccessible);
		return ThingContainer.tempList;
	}

	public void _List(Func<Thing, bool> func, bool onlyAccessible = false)
	{
		foreach (Thing thing in this)
		{
			if (!onlyAccessible || !(thing.parent is Card) || (thing.parent as Card).c_lockLv <= 0)
			{
				thing.things._List(func, onlyAccessible);
				if (func(thing))
				{
					ThingContainer.tempList.Add(thing);
				}
			}
		}
	}

	public void AddFactory(HashSet<string> hash)
	{
		foreach (Thing thing in this)
		{
			if (thing.CanSearchContents)
			{
				thing.things.AddFactory(hash);
			}
			if (thing.trait.IsFactory)
			{
				hash.Add(thing.id);
			}
			if (thing.trait.ToggleType == ToggleType.Fire && thing.isOn)
			{
				hash.Add("fire");
			}
		}
	}

	public void Foreach(Action<Thing> action, bool onlyAccessible = true)
	{
		foreach (Thing thing in this)
		{
			if (!onlyAccessible || thing.CanSearchContents)
			{
				thing.things.Foreach(action, true);
			}
			action(thing);
		}
	}

	public void Foreach(Func<Thing, bool> action, bool onlyAccessible = true)
	{
		foreach (Thing thing in this)
		{
			if (!onlyAccessible || thing.CanSearchContents)
			{
				thing.things.Foreach(action, true);
			}
			if (action(thing))
			{
				break;
			}
		}
	}

	[CompilerGenerated]
	private void <GetDest>g__SearchDest|31_0(ThingContainer things, bool searchEmpty, bool searchStack, ref ThingContainer.<>c__DisplayClass31_0 A_4)
	{
		if (A_4.t.IsContainer && A_4.t.things.Count > 0 && things.owner.IsContainer && !(things.owner.trait is TraitToolBelt))
		{
			return;
		}
		if (searchStack && A_4.tryStack)
		{
			Thing thing = things.CanStack(A_4.t, -1, -1);
			if (thing != A_4.t)
			{
				A_4.d.stack = thing;
				return;
			}
		}
		if (searchEmpty && !things.IsFull(0))
		{
			if (things.owner.trait is TraitToolBelt)
			{
				return;
			}
			if (!things.owner.isChara && !(things.owner.parent is Chara))
			{
				Thing thing2 = things.owner.parent as Thing;
				if (!(((thing2 != null) ? thing2.trait : null) is TraitToolBelt))
				{
					return;
				}
			}
			A_4.d.container = things.owner;
		}
	}

	[CompilerGenerated]
	private void <GetDest>g__TrySearchContainer|31_1(Card c, ref ThingContainer.<>c__DisplayClass31_0 A_2)
	{
		foreach (Thing thing in c.things)
		{
			if (thing.CanSearchContents)
			{
				this.<GetDest>g__TrySearchContainer|31_1(thing, ref A_2);
			}
		}
		if (c.things == this)
		{
			return;
		}
		Window.SaveData windowSaveData = c.GetWindowSaveData();
		if (windowSaveData != null)
		{
			if (windowSaveData.noRotten && A_2.t.IsDecayed)
			{
				return;
			}
			if (windowSaveData.onlyRottable && A_2.t.trait.Decay == 0)
			{
				return;
			}
			if (windowSaveData.userFilter && !windowSaveData.IsFilterPass(A_2.t.GetName(NameStyle.Full, 1)))
			{
				return;
			}
			if (windowSaveData.advDistribution)
			{
				using (HashSet<int>.Enumerator enumerator2 = windowSaveData.cats.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						int num = enumerator2.Current;
						if (A_2.t.category.uid == num)
						{
							ThingContainer._listContainers.Add(c.things);
							break;
						}
					}
					return;
				}
			}
			if (!windowSaveData.flag.HasFlag(A_2.flag))
			{
				ThingContainer._listContainers.Add(c.things);
			}
		}
	}

	[CompilerGenerated]
	internal static int <FindStealable>g__Compare|41_0(Thing a)
	{
		return a.SelfWeight + (a.isEquipped ? 10000 : 0);
	}

	[JsonIgnore]
	public static List<Thing> listUnassigned = new List<Thing>();

	[JsonIgnore]
	public const int InvYHotbar = 1;

	[JsonIgnore]
	public int width;

	[JsonIgnore]
	public int height;

	[JsonIgnore]
	public Card owner;

	[JsonIgnore]
	public List<Thing> grid;

	private static List<ThingContainer> _listContainers = new List<ThingContainer>();

	private static List<Thing> tempList = new List<Thing>();

	public struct DestData
	{
		public bool IsValid
		{
			get
			{
				return this.stack != null || this.container != null;
			}
		}

		public Card container;

		public Thing stack;
	}
}
