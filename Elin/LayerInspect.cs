using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LayerInspect : ELayer
{
	public void SetItems(List<Thing> things)
	{
		foreach (Thing t in things)
		{
			this.items.Add(LayerInspect.GetItem(t));
		}
		this.Refresh();
	}

	public void SetItemsOnHitPoint()
	{
		this.inspectMap = true;
		this.items = LayerInspect.GetMenuItems();
		this.Refresh();
	}

	public override void OnUpdateInput()
	{
		if (!this.inspectMap)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (!ELayer.ui.isPointerOverUI && Scene.HitPoint.IsValid)
			{
				this.SetItemsOnHitPoint();
				return;
			}
		}
		else
		{
			bool anyKeyDown = Input.anyKeyDown;
		}
	}

	public void Refresh()
	{
		this.list.Clear();
		BaseList baseList = this.list;
		UIList.Callback<LayerInspect.MenuItem, ButtonGeneral> callback = new UIList.Callback<LayerInspect.MenuItem, ButtonGeneral>();
		callback.onClick = delegate(LayerInspect.MenuItem a, ButtonGeneral b)
		{
			if (a.action != null)
			{
				a.action();
			}
		};
		callback.onInstantiate = delegate(LayerInspect.MenuItem a, ButtonGeneral b)
		{
			if (a.card != null)
			{
				b.SetCard(a.card, FontColor.Ignore);
				return;
			}
			b.mainText.text = a.text;
			if (a.renderRow != null)
			{
				a.renderRow.SetImage(b.icon, null, 0, true, 0, 0);
			}
		};
		baseList.callbacks = callback;
		this.list.AddCollection(this.items);
		this.list.Refresh(false);
	}

	public static LayerInspect.MenuItem GetItem(Card t)
	{
		return new LayerInspect.MenuItem
		{
			card = t,
			text = t.Name,
			action = delegate()
			{
				if (t.isChara)
				{
					ELayer.ui.AddLayer<LayerChara>().SetChara(t.Chara);
					return;
				}
				ELayer.ui.AddLayer<LayerInfo>().Set(t, false);
			}
		};
	}

	public static List<LayerInspect.MenuItem> GetMenuItems()
	{
		List<LayerInspect.MenuItem> list = new List<LayerInspect.MenuItem>();
		Point point = Scene.HitPoint.Copy();
		if (!point.IsValid)
		{
			return list;
		}
		foreach (Card card in point.ListCards(false))
		{
			if (card.isSynced)
			{
				list.Add(LayerInspect.GetItem(card));
			}
		}
		if (point.area != null)
		{
			List<LayerInspect.MenuItem> list2 = list;
			LayerInspect.MenuItem menuItem = new LayerInspect.MenuItem();
			menuItem.text = point.area.Name;
			menuItem.action = delegate()
			{
			};
			list2.Add(menuItem);
		}
		if (point.cell.HasLiquid)
		{
			list.Add(new LayerInspect.MenuItem
			{
				text = point.cell.GetLiquidName(),
				action = delegate()
				{
					ELayer.ui.AddLayer<LayerInfo>().SetLiquid(point.cell);
				},
				renderRow = point.cell.sourceEffect
			});
		}
		if (point.matBlock.id != 0)
		{
			list.Add(new LayerInspect.MenuItem
			{
				text = point.cell.GetBlockName(),
				action = delegate()
				{
					ELayer.ui.AddLayer<LayerInfo>().SetBlock(point.cell);
				},
				renderRow = point.cell.sourceBlock
			});
		}
		if (point.matFloor.id != 0)
		{
			list.Add(new LayerInspect.MenuItem
			{
				text = point.cell.GetFloorName(),
				action = delegate()
				{
					ELayer.ui.AddLayer<LayerInfo>().SetFloor(point.cell);
				},
				renderRow = point.cell.sourceFloor
			});
		}
		if (point.cell.obj != 0)
		{
			list.Add(new LayerInspect.MenuItem
			{
				text = point.sourceObj.GetName(),
				action = delegate()
				{
					ELayer.ui.AddLayer<LayerInfo>().SetObj(point.cell);
				},
				renderRow = point.cell.sourceObj
			});
		}
		return list;
	}

	public bool inspectMap;

	public UIList list;

	public List<LayerInspect.MenuItem> items = new List<LayerInspect.MenuItem>();

	public class MenuItem
	{
		public string id;

		public string text;

		public UnityAction action;

		public Card card;

		public RenderRow renderRow;
	}
}
