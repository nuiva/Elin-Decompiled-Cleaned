using System;
using UnityEngine;

public class LayerEquip : ELayer
{
	public static void SetDirty()
	{
		LayerEquip.dirty = true;
	}

	public override void OnInit()
	{
		this.windows[0].layer = this;
		if (!Window.dictData.ContainsKey(this.windows[0].idWindow) && !ELayer.ui.widgets.GetWidget("BottomBar"))
		{
			RectTransform rectTransform = this.windows[0].Rect();
			this.windows[0].Rect().anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 45f);
		}
		this.chara = ELayer.pc;
		LayerEquip.Instance = this;
		this.Rebuild();
	}

	public void Rebuild()
	{
		this.RefreshEquipment(this.listMain, false);
		this.RefreshEquipment(this.listEtc, true);
		this.RefreshToolBelt(false);
		this.windows[0].RebuildLayout(true);
		LayerEquip.dirty = true;
	}

	public void Redraw()
	{
		LayerEquip.dirty = false;
		this.listMain.Redraw();
		this.listEtc.Redraw();
	}

	public void RefreshToolBelt(bool rebuildLayout = false)
	{
	}

	public void RefreshEquipment(UIList list, bool etc = false)
	{
		list.Clear();
		list.callbacks = new UIList.Callback<BodySlot, ButtonGridDrag>
		{
			onInstantiate = delegate(BodySlot a, ButtonGridDrag b)
			{
				b.SetBodySlot(a, new InvOwnerEquip(this.chara, a, null, CurrencyType.None), true);
			},
			onRedraw = delegate(BodySlot a, ButtonGridDrag b, int i)
			{
				b.SetBodySlot(a, new InvOwnerEquip(this.chara, a, null, CurrencyType.None), true);
			},
			onSort = ((BodySlot a, UIList.SortMode b) => this.chara.body.GetSortVal(a)),
			onList = delegate(UIList.SortMode m)
			{
				foreach (BodySlot bodySlot in this.chara.body.slots)
				{
					if (bodySlot.elementId != 44)
					{
						if (bodySlot.elementId == 36 || bodySlot.elementId == 31 || bodySlot.elementId == 37 || bodySlot.elementId == 45)
						{
							if (!etc)
							{
								continue;
							}
						}
						else if (etc)
						{
							continue;
						}
						list.Add(bodySlot);
					}
				}
			}
		};
		list.sortMode = UIList.SortMode.ByNumber;
		list.List(false);
	}

	public static LayerEquip Instance;

	public static bool dirty;

	public UIList listMain;

	public UIList listEtc;

	public Chara chara;
}
