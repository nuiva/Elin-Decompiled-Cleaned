using UnityEngine;

public class LayerLocation : ELayer
{
	public UIList listZone;

	public Sprite spriteFaction;

	public Sprite spriteFaith;

	public UIZoneInfo info;

	public override bool HeaderIsListOf(int id)
	{
		return true;
	}

	public override void OnSwitchContent(Window window)
	{
		if (window.windowIndex == 1)
		{
			RefreshZones();
		}
	}

	public void RefreshZones()
	{
		UIList uIList = listZone;
		uIList.Clear();
		uIList.callbacks = new UIList.Callback<Spatial, ItemGeneral>
		{
			onClick = delegate(Spatial a, ItemGeneral b)
			{
				if (a is Zone)
				{
					info.SetZone(a as Zone);
				}
			},
			onInstantiate = delegate(Spatial a, ItemGeneral b)
			{
				b.SetSound();
				b.SetMainText(a.Name);
				b.Build();
			}
		};
		foreach (Spatial value in ELayer.game.spatials.map.Values)
		{
			if (!(value is Zone) || value.parent != ELayer.pc.currentZone.Region)
			{
				continue;
			}
			if (windows[1].idTab == 0)
			{
				if (!value.IsPlayerFaction)
				{
					continue;
				}
			}
			else if (value.IsPlayerFaction)
			{
				continue;
			}
			uIList.Add(value);
		}
		uIList.Refresh();
	}
}
