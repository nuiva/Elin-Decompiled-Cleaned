using System;
using UnityEngine;

public class WidgetNotice : BaseWidgetNotice
{
	public class Extra
	{
		public bool showStock;
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

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public static void RefreshAll()
	{
		if ((bool)Instance)
		{
			Instance._RefreshAll();
		}
	}

	public override void _OnActivate()
	{
		Instance = this;
		LittlePopper.showStock = extra.showStock;
		Add(new NotificationHome(), H1);
		itemBattle = Add(new NotificationBattle(), H4);
		itemGuest = Add(new NotificationGuest(), H4);
	}

	public override void OnRefresh()
	{
		battles = 0;
		guests = 0;
		foreach (Chara chara in EMono._map.charas)
		{
			if (chara.IsHomeMember())
			{
				if (chara.enemy != null && chara.IsAliveInCurrentZone)
				{
					battles++;
				}
			}
			else if (chara.IsGuest())
			{
				guests++;
			}
		}
		H4.SetActive(itemBattle.gameObject.activeSelf || itemGuest.gameObject.activeSelf);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddToggle("showStockPop", extra.showStock, delegate(bool a)
		{
			extra.showStock = (LittlePopper.showStock = a);
		});
		base.OnSetContextMenu(m);
	}
}
