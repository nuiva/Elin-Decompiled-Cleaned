using UnityEngine;

public class LayerEquip : ELayer
{
	public static LayerEquip Instance;

	public static bool dirty;

	public UIList listMain;

	public UIList listEtc;

	public Chara chara;

	public static void SetDirty()
	{
		dirty = true;
	}

	public override void OnInit()
	{
		windows[0].layer = this;
		if (!Window.dictData.ContainsKey(windows[0].idWindow) && !ELayer.ui.widgets.GetWidget("BottomBar"))
		{
			RectTransform rectTransform = windows[0].Rect();
			windows[0].Rect().anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 45f);
		}
		chara = ELayer.pc;
		Instance = this;
		Rebuild();
	}

	public void Rebuild()
	{
		RefreshEquipment(listMain);
		RefreshEquipment(listEtc, etc: true);
		RefreshToolBelt();
		windows[0].RebuildLayout(recursive: true);
		dirty = true;
	}

	public void Redraw()
	{
		dirty = false;
		listMain.Redraw();
		listEtc.Redraw();
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
				b.SetBodySlot(a, new InvOwnerEquip(chara, a), showIndex: true);
			},
			onRedraw = delegate(BodySlot a, ButtonGridDrag b, int i)
			{
				b.SetBodySlot(a, new InvOwnerEquip(chara, a), showIndex: true);
			},
			onSort = (BodySlot a, UIList.SortMode b) => chara.body.GetSortVal(a),
			onList = delegate
			{
				foreach (BodySlot slot in chara.body.slots)
				{
					if (slot.elementId != 44)
					{
						if (slot.elementId == 36 || slot.elementId == 31 || slot.elementId == 37 || slot.elementId == 45)
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
						list.Add(slot);
					}
				}
			}
		};
		list.sortMode = UIList.SortMode.ByNumber;
		list.List();
	}
}
