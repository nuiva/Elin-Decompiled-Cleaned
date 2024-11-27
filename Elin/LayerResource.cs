using System;
using System.Collections.Generic;

public class LayerResource : ELayer
{
	public override void OnInit()
	{
		this.list2.sortMode = ELayer.player.pref.sortResources;
		this.RefreshCategory();
		if (!LayerResource.selectCat.IsEmpty())
		{
			this.list.Select<SourceCategory.Row>((SourceCategory.Row a) => a.id == LayerResource.selectCat, true);
		}
		else
		{
			this.list.Select(0, true);
		}
		LayerResource.selectCat = null;
		WindowMenu menuRight = this.windows[0].menuRight;
		menuRight.AddButton2Line("sort", () => this.list2.sortMode.ToString(), delegate(UIButton b)
		{
			this.list2.NextSort();
		}, null, "2line");
		menuRight.AddButton2Line("stockCapacity", () => ELayer._map.Stocked.weight.ToString() + "/" + ELayer._map.Stocked.maxWeight.ToString(), null, null, "2line");
	}

	public override void OnKill()
	{
		ELayer.player.pref.sortResources = this.list2.sortMode;
	}

	public void RefreshCategory()
	{
		HashSet<string> track = ELayer.player.trackedCategories;
		this.list.Clear();
		this.list.callbacks = new UIList.Callback<SourceCategory.Row, ButtonCategory>
		{
			onClick = delegate(SourceCategory.Row a, ButtonCategory b)
			{
				this.ShowThings(a);
			},
			onInstantiate = delegate(SourceCategory.Row cat, ButtonCategory b)
			{
				b.toggle.SetActive(true);
				b.toggle.isOn = track.Contains(cat.id);
				b.toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					SE.ClickGeneral();
					if (isOn)
					{
						track.Add(cat.id);
					}
					else
					{
						track.Remove(cat.id);
					}
					UIResourceTrack.Refresh();
				});
				int sum = ELayer._map.Stocked.categoryMap[cat.id].sum;
				b.mainText.text = cat.GetText("name", false).ToTitleCase(false) + " (" + sum.ToString() + ")";
				b.SetFold(cat.children.Count > 0, true, delegate(UIList l)
				{
					foreach (SourceCategory.Row row2 in ELayer.sources.categories.rows)
					{
						if (row2.parent == cat)
						{
							l.Add(row2);
						}
					}
				});
			},
			onRefresh = null
		};
		foreach (SourceCategory.Row row in ELayer.sources.categories.rows)
		{
			if (!(row.id == "new") && !(row.id == "stash") && row.parent == null)
			{
				this.list.Add(row);
			}
		}
		this.list.Refresh(false);
		UIResourceTrack.Refresh();
	}

	public void ShowThings(SourceCategory.Row cat)
	{
		HashSet<string> track = ELayer.player.trackedCards;
		this.list2.callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onRedraw = delegate(Thing a, ButtonGrid b, int i)
			{
				b.SetCard(a, ButtonGrid.Mode.Default, null);
				bool check = track.Contains(a.source.id);
				b.SetCheck(check);
			},
			onClick = delegate(Thing a, ButtonGrid b)
			{
				if (!track.Contains(a.source.id))
				{
					track.Add(a.source.id);
				}
				else
				{
					track.Remove(a.source.id);
				}
				UIResourceTrack.Refresh();
				this.list2.dsv.refresh();
				SE.Tab();
			},
			onList = delegate(UIList.SortMode m)
			{
				List<Thing> list = new List<Thing>();
				list = ELayer._map.Stocked.ListThingsInCategory(cat);
				for (int i = 0; i < list.Count; i++)
				{
					list[i].SetSortVal(m, CurrencyType.Money);
				}
				list.Sort((Thing a, Thing b) => a.sortVal - b.sortVal);
				int num = 0;
				int num2 = 0;
				foreach (Thing thing in list)
				{
					this.list2.Add(thing);
					num += thing.Num;
					num2 += thing.Num;
				}
				this.windows[0].textStats.SetText("statsTotal".lang(list.Count.ToString() ?? "", num.ToString() ?? "", num2.ToString() ?? "", null, null));
			}
		};
		this.list2.List();
	}

	public UIList list;

	public UIDynamicList list2;

	public UICardInfo info;

	public UINote noteOverview;

	public static string selectCat;
}
