using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetEquip : Widget
{
	public override object CreateExtra()
	{
		return new WidgetEquip.Extra();
	}

	public WidgetEquip.Extra extra
	{
		get
		{
			return base.config.extra as WidgetEquip.Extra;
		}
	}

	public static void SetDirty()
	{
		WidgetEquip.dirty = true;
	}

	public override void OnActivate()
	{
		this.chara = EMono.pc;
		WidgetEquip.Instance = this;
		foreach (Thing thing in EMono.pc.things)
		{
			if (thing.trait is TraitToolBelt)
			{
				base.AddLayer(LayerInventory.CreateContainer(thing), this.transLayer);
			}
		}
		this.Rebuild();
	}

	public void Rebuild()
	{
		this.RefreshEquipment(this.listMain, false);
		this.RefreshEquipment(this.listEtc, true);
		WidgetEquip.dirty = true;
		this.RebuildLayout(true);
	}

	public void CheckDirty()
	{
		if (WidgetEquip.dirty)
		{
			this._Redraw();
		}
	}

	public static void Redraw()
	{
		if (WidgetEquip.Instance)
		{
			WidgetEquip.Instance._Redraw();
		}
	}

	public void _Redraw()
	{
		WidgetEquip.dirty = false;
		this.listMain.Redraw();
		this.listEtc.Redraw();
	}

	public void RefreshEquipment(UIList list, bool etc = false)
	{
		list.Clear();
		list.callbacks = new UIList.Callback<BodySlot, ButtonGridDrag>
		{
			onInstantiate = delegate(BodySlot a, ButtonGridDrag b)
			{
				b.SetBodySlot(a, new InvOwnerEquip(this.chara, a, null, CurrencyType.None), this.extra.showIndex);
			},
			onRedraw = delegate(BodySlot a, ButtonGridDrag b, int i)
			{
				b.SetBodySlot(a, new InvOwnerEquip(this.chara, a, null, CurrencyType.None), this.extra.showIndex);
			},
			onSort = ((BodySlot a, UIList.SortMode b) => this.chara.body.GetSortVal(a)),
			onList = delegate(UIList.SortMode m)
			{
				int num = 0;
				foreach (BodySlot bodySlot in this.chara.body.slots)
				{
					if (bodySlot.elementId != 44)
					{
						if (bodySlot.elementId == 36 || bodySlot.elementId == 31 || bodySlot.elementId == 45)
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
						num++;
					}
				}
				if (list == this.listMain)
				{
					this.grid.constraintCount = num / 12 + 1;
				}
			}
		};
		list.onAfterRedraw = delegate()
		{
			LayerInventory.TryShowGuide(list);
		};
		list.sortMode = UIList.SortMode.ByNumber;
		list.List(false);
	}

	public override bool CanShowContextMenu()
	{
		ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
		return (!componentOf || componentOf.card == null) && base.CanShowContextMenu();
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("setting").AddToggle("showWeaponIndex", this.extra.showIndex, delegate(bool a)
		{
			this.extra.showIndex = a;
			this.Rebuild();
			SE.ClickOk();
		});
		base.SetBaseContextMenu(m);
	}

	public static WidgetEquip Instance;

	public static bool dirty;

	public static Thing dragEquip;

	public UIList listMain;

	public UIList listEtc;

	public Chara chara;

	public RectTransform transLayer;

	public GridLayoutGroup grid;

	public class Extra
	{
		public bool showIndex;
	}
}
