using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LayerInspect : ELayer
{
	public class MenuItem
	{
		public string id;

		public string text;

		public UnityAction action;

		public Card card;

		public RenderRow renderRow;
	}

	public bool inspectMap;

	public UIList list;

	public List<MenuItem> items = new List<MenuItem>();

	public void SetItems(List<Thing> things)
	{
		foreach (Thing thing in things)
		{
			items.Add(GetItem(thing));
		}
		Refresh();
	}

	public void SetItemsOnHitPoint()
	{
		inspectMap = true;
		items = GetMenuItems();
		Refresh();
	}

	public override void OnUpdateInput()
	{
		if (!inspectMap)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (!ELayer.ui.isPointerOverUI && Scene.HitPoint.IsValid)
			{
				SetItemsOnHitPoint();
			}
		}
		else
		{
			_ = Input.anyKeyDown;
		}
	}

	public void Refresh()
	{
		list.Clear();
		list.callbacks = new UIList.Callback<MenuItem, ButtonGeneral>
		{
			onClick = delegate(MenuItem a, ButtonGeneral b)
			{
				if (a.action != null)
				{
					a.action();
				}
			},
			onInstantiate = delegate(MenuItem a, ButtonGeneral b)
			{
				if (a.card != null)
				{
					b.SetCard(a.card);
				}
				else
				{
					b.mainText.text = a.text;
					if (a.renderRow != null)
					{
						a.renderRow.SetImage(b.icon);
					}
				}
			}
		};
		list.AddCollection(items);
		list.Refresh();
	}

	public static MenuItem GetItem(Card t)
	{
		return new MenuItem
		{
			card = t,
			text = t.Name,
			action = delegate
			{
				if (t.isChara)
				{
					ELayer.ui.AddLayer<LayerChara>().SetChara(t.Chara);
				}
				else
				{
					ELayer.ui.AddLayer<LayerInfo>().Set(t);
				}
			}
		};
	}

	public static List<MenuItem> GetMenuItems()
	{
		List<MenuItem> list = new List<MenuItem>();
		Point point = Scene.HitPoint.Copy();
		if (!point.IsValid)
		{
			return list;
		}
		foreach (Card item in point.ListCards())
		{
			if (item.isSynced)
			{
				list.Add(GetItem(item));
			}
		}
		if (point.area != null)
		{
			list.Add(new MenuItem
			{
				text = point.area.Name,
				action = delegate
				{
				}
			});
		}
		if (point.cell.HasLiquid)
		{
			list.Add(new MenuItem
			{
				text = point.cell.GetLiquidName(),
				action = delegate
				{
					ELayer.ui.AddLayer<LayerInfo>().SetLiquid(point.cell);
				},
				renderRow = point.cell.sourceEffect
			});
		}
		if (point.matBlock.id != 0)
		{
			list.Add(new MenuItem
			{
				text = point.cell.GetBlockName(),
				action = delegate
				{
					ELayer.ui.AddLayer<LayerInfo>().SetBlock(point.cell);
				},
				renderRow = point.cell.sourceBlock
			});
		}
		if (point.matFloor.id != 0)
		{
			list.Add(new MenuItem
			{
				text = point.cell.GetFloorName(),
				action = delegate
				{
					ELayer.ui.AddLayer<LayerInfo>().SetFloor(point.cell);
				},
				renderRow = point.cell.sourceFloor
			});
		}
		if (point.cell.obj != 0)
		{
			list.Add(new MenuItem
			{
				text = point.sourceObj.GetName(),
				action = delegate
				{
					ELayer.ui.AddLayer<LayerInfo>().SetObj(point.cell);
				},
				renderRow = point.cell.sourceObj
			});
		}
		return list;
	}
}
