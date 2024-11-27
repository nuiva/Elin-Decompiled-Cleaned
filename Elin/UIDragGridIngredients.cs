using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDragGridIngredients : EMono
{
	public void Update()
	{
		bool activeSelf = this.goList.activeSelf;
		bool isNoGoal = EMono.pc.ai.IsNoGoal;
		this.goList.SetActive(isNoGoal);
	}

	public void Refresh()
	{
		Debug.Log("Refreshing uiDragGridIngredients");
		List<Thing> list = new List<Thing>();
		bool allowStockIngredients = this.layer.owner.AllowStockIngredients;
		if ((EMono._zone.IsPCFaction || EMono._zone is Zone_Tent) && allowStockIngredients)
		{
			foreach (Thing thing in EMono._map.Stocked.Things)
			{
				if (this.layer.owner.ShouldShowGuide(thing) && !thing.c_isImportant && thing.parentCard != null && thing.parentCard.c_lockLv == 0 && !(thing.parentCard.trait is TraitChestMerchant))
				{
					Window.SaveData windowSaveData = thing.parentCard.GetWindowSaveData();
					if (windowSaveData == null || !windowSaveData.excludeCraft)
					{
						list.Add(thing);
						thing.SetSortVal(UIList.SortMode.ByCategory, CurrencyType.Money);
					}
				}
			}
			list.Sort((Thing a, Thing b) => b.sortVal - a.sortVal);
		}
		this.list.callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onClick = delegate(Thing a, ButtonGrid b)
			{
				int currentIndex = this.layer.currentIndex;
				this.layer.buttons[currentIndex].SetCardGrid(a, this.layer.owner);
				this.layer.owner.OnProcess(a);
			},
			onInstantiate = delegate(Thing a, ButtonGrid b)
			{
				b.SetCard(a, ButtonGrid.Mode.Grid, null);
				b.SetOnClick(delegate
				{
				});
				b.onRightClick = delegate()
				{
					this.list.callbacks.OnClick(a, b);
				};
			}
		};
		this.list.Clear();
		foreach (Thing o in list)
		{
			this.list.Add(o);
		}
		this.list.Refresh(false);
	}

	public LayerDragGrid layer;

	public UIList list;

	public UIScrollView view;

	public GameObject goList;
}
