using System;
using UnityEngine;

public class WidgetNotice : BaseWidgetNotice
{
	public override object CreateExtra()
	{
		return new WidgetNotice.Extra();
	}

	public WidgetNotice.Extra extra
	{
		get
		{
			return base.config.extra as WidgetNotice.Extra;
		}
	}

	public static void RefreshAll()
	{
		if (!WidgetNotice.Instance)
		{
			return;
		}
		WidgetNotice.Instance._RefreshAll();
	}

	public override void _OnActivate()
	{
		WidgetNotice.Instance = this;
		LittlePopper.showStock = this.extra.showStock;
		base.Add(new NotificationHome(), this.H1);
		base.Add(new NotificationStockpile(), this.H3);
		this.itemBattle = base.Add(new NotificationBattle(), this.H4);
		this.itemGuest = base.Add(new NotificationGuest(), this.H4);
	}

	public override void OnRefresh()
	{
		this.battles = 0;
		this.guests = 0;
		foreach (Chara chara in EMono._map.charas)
		{
			if (chara.IsHomeMember())
			{
				if (chara.enemy != null && chara.IsAliveInCurrentZone)
				{
					this.battles++;
				}
			}
			else if (chara.IsGuest())
			{
				this.guests++;
			}
		}
		this.H4.SetActive(this.itemBattle.gameObject.activeSelf || this.itemGuest.gameObject.activeSelf);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddToggle("showStockPop", this.extra.showStock, delegate(bool a)
		{
			WidgetNotice.Extra extra = this.extra;
			LittlePopper.showStock = a;
			extra.showStock = a;
		});
		base.OnSetContextMenu(m);
	}

	public static WidgetNotice Instance;

	public Transform H1;

	public Transform H2;

	public Transform H3;

	public Transform H4;

	public ItemNotice itemBattle;

	public ItemNotice itemGuest;

	[NonSerialized]
	public int battles;

	[NonSerialized]
	public int guests;

	public class Extra
	{
		public bool showStock;
	}
}
