using System;
using System.Collections.Generic;
using System.Linq;

public class LayerHome : ELayer
{
	public override bool HeaderIsListOf(int id)
	{
		return id != 0;
	}

	public override void OnInit()
	{
		this.branch = ELayer.Branch;
		LayerHome.Instance = this;
	}

	public override void OnSwitchContent(Window window)
	{
		if (window.setting.tabs.Count == 0)
		{
			return;
		}
		WindowMenu menuLeft = window.menuLeft;
		menuLeft.Clear();
		if (ELayer.player.shippingResults.Count > 0)
		{
			menuLeft.AddButtonSimple(() => "shipping_result".lang(), delegate(UIButton a)
			{
				ELayer.ui.AddLayer<LayerShippingResult>().Show();
			}, null, "Simple");
		}
		string idLang = window.CurrentTab.idLang;
		if (idLang == "top")
		{
			this.info.Refresh();
			return;
		}
		if (!(idLang == "skills"))
		{
			if (!(idLang == "population"))
			{
				if (!(idLang == "chronicle"))
				{
					return;
				}
				this.RefreshChronicle();
			}
			return;
		}
		this.RefreshFeat();
	}

	public void RefreshChronicle()
	{
	}

	public void RefreshTech(bool listComplete = false)
	{
		UIList list = listComplete ? this.listTech2 : this.listTech;
		list.sortMode = ELayer.player.pref.sortResearch;
		BaseList list2 = list;
		UIList.Callback<ResearchPlan, ItemResearch> callback = new UIList.Callback<ResearchPlan, ItemResearch>();
		callback.onInstantiate = delegate(ResearchPlan a, ItemResearch b)
		{
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (ResearchPlan o in (listComplete ? this.branch.researches.finished : this.branch.researches.plans))
			{
				list.Add(o);
			}
		};
		callback.onSort = ((ResearchPlan a, UIList.SortMode m) => a.GetSortVal(m));
		list2.callbacks = callback;
		list.List(false);
		this.RefreshFocus();
	}

	public void RefreshFocus()
	{
		foreach (ItemResearch itemResearch in this.listTech.GetComponentsInChildren<ItemResearch>())
		{
			itemResearch.goFocus.SetActive(itemResearch.plan == this.branch.researches.focused);
		}
	}

	public void RefreshFeat()
	{
		BaseList baseList = this.listFeat;
		UIList.Callback<Element, ButtonElement> callback = new UIList.Callback<Element, ButtonElement>();
		callback.onClick = delegate(Element a, ButtonElement b)
		{
			int p = ELayer.Branch.GetTechUpgradeCost(a);
			int currency = ELayer.pc.GetCurrency("money2");
			if (p == 0 || p > currency || a.ValueWithoutLink == 0 || a.source.cost[0] == 0)
			{
				SE.Beep();
				return;
			}
			Dialog.YesNo("dialogUpgradeTech".lang(a.Name, null, null, null, null), delegate
			{
				SE.Pay();
				ELayer.pc.ModCurrency(-p, "money2");
				ELayer.Branch.elements.ModBase(a.id, 1);
				this.listFeat.List(false);
				ELayer.Branch.resources.SetDirty();
				ELayer.core.game.player.hotbars.ResetHotbar(2);
				if (WidgetHotbar.HotBarMainMenu)
				{
					WidgetHotbar.HotBarMainMenu.RebuildPage(-1);
				}
				if (WidgetMenuPanel.Instance)
				{
					WidgetMenuPanel.Instance.OnChangeActionMode();
				}
			}, null, "yes", "no");
		};
		callback.onInstantiate = delegate(Element a, ButtonElement b)
		{
			b.SetElement(a, ELayer._zone.elements, ButtonElement.Mode.Tech);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Element o in from b in this.branch.elements.dict.Values
			where (b.Value > 0 || b.vBase > 0) && b.source.category != "policy" && b.source.category != "landfeat" && !b.HasTag("hidden")
			select b)
			{
				this.listFeat.Add(o);
			}
		};
		callback.onSort = ((Element a, UIList.SortMode m) => a.GetSortVal(m));
		callback.onRefresh = null;
		baseList.callbacks = callback;
		this.listFeat.List(false);
		BaseList baseList2 = this.listFeat2;
		UIList.Callback<Element, ButtonElement> callback2 = new UIList.Callback<Element, ButtonElement>();
		callback2.onClick = delegate(Element a, ButtonElement b)
		{
		};
		callback2.onInstantiate = delegate(Element a, ButtonElement b)
		{
			b.SetElement(a, ELayer._zone.elements, ButtonElement.Mode.Policy);
		};
		callback2.onList = delegate(UIList.SortMode m)
		{
			foreach (Element o in from b in this.branch.elements.dict.Values
			where b.source.category == "policy"
			select b)
			{
				this.listFeat2.Add(o);
			}
		};
		callback2.onSort = ((Element a, UIList.SortMode m) => a.GetSortVal(m));
		callback2.onRefresh = null;
		baseList2.callbacks = callback2;
		this.listFeat2.List(false);
		BaseList baseList3 = this.listFeat3;
		UIList.Callback<Element, ButtonElement> callback3 = new UIList.Callback<Element, ButtonElement>();
		callback3.onClick = delegate(Element a, ButtonElement b)
		{
		};
		callback3.onInstantiate = delegate(Element a, ButtonElement b)
		{
			b.SetElement(a, ELayer._zone.elements, ButtonElement.Mode.LandFeat);
		};
		callback3.onList = delegate(UIList.SortMode m)
		{
			IEnumerable<Element> enumerable = this.branch.elements.dict.Values;
			if (this.branch.HasNetwork)
			{
				enumerable = enumerable.Concat(ELayer.pc.faction.elements.dict.Values);
			}
			foreach (Element o in from b in enumerable
			where (b.Value > 0 || b.vBase > 0) && b.source.category == "landfeat" && !b.HasTag("hidden")
			select b)
			{
				this.listFeat3.Add(o);
			}
		};
		callback3.onSort = ((Element a, UIList.SortMode m) => a.GetSortVal(m));
		callback3.onRefresh = null;
		baseList3.callbacks = callback3;
		this.listFeat3.List(false);
	}

	public void RefreshAreas()
	{
		BaseList baseList = this.listArea;
		UIList.Callback<Area, ItemGeneral> callback = new UIList.Callback<Area, ItemGeneral>();
		callback.onClick = delegate(Area a, ItemGeneral b)
		{
		};
		callback.onInstantiate = delegate(Area a, ItemGeneral b)
		{
			UIButton button = b.button1;
			b.SetMainText(a.Name, null, true);
			b.AddSubButton(ELayer.core.refs.icons.go, delegate
			{
				ELayer.screen.Focus(a.points[0]);
			}, "tFocus", null);
			b.Build();
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Area o in ELayer._map.rooms.listArea)
			{
				this.listArea.Add(o);
			}
		};
		callback.onSort = ((Area a, UIList.SortMode m) => a.GetSortVal(m));
		callback.onRefresh = null;
		baseList.callbacks = callback;
		this.listArea.List(false);
	}

	public void RefreshSpots()
	{
		this.listArea.Clear();
		BaseList baseList = this.listArea;
		UIList.Callback<Trait, ItemGeneral> callback = new UIList.Callback<Trait, ItemGeneral>();
		callback.onClick = delegate(Trait a, ItemGeneral b)
		{
		};
		callback.onInstantiate = delegate(Trait a, ItemGeneral b)
		{
			UIButton button = b.button1;
			b.SetMainText(a.Name, null, true);
			b.AddSubButton(ELayer.core.refs.icons.go, delegate
			{
				ELayer.screen.Focus(a.GetPoint());
			}, "tFocus", null);
			b.Build();
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Card card in ELayer._map.Installed.all.Values)
			{
				if (card.trait.IsSpot)
				{
					this.listArea.Add(card.trait);
				}
			}
		};
		callback.onSort = ((Trait a, UIList.SortMode m) => a.GetSortVal(m));
		callback.onRefresh = null;
		baseList.callbacks = callback;
		this.listArea.List(false);
	}

	public static LayerHome Instance;

	public UIList listArea;

	public UIList listTech;

	public UIList listTech2;

	public UIList listFeat;

	public UIList listFeat2;

	public UIList listFeat3;

	public FactionBranch branch;

	public UIHomeInfo info;

	public const int TabResearch = 1;

	public const int TabPolicy = 2;

	public const int TabPopulation = 3;

	public const int TabFeat = 4;
}
