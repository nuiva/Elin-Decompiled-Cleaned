using System;
using UnityEngine;

public class LayerLocation : ELayer
{
	public override bool HeaderIsListOf(int id)
	{
		return true;
	}

	public override void OnSwitchContent(Window window)
	{
		if (window.windowIndex == 1)
		{
			this.RefreshZones();
		}
	}

	public void RefreshZones()
	{
		UIList uilist = this.listZone;
		uilist.Clear();
		BaseList baseList = uilist;
		UIList.Callback<Spatial, ItemGeneral> callback = new UIList.Callback<Spatial, ItemGeneral>();
		callback.onClick = delegate(Spatial a, ItemGeneral b)
		{
			if (!(a is Zone))
			{
				return;
			}
			this.info.SetZone(a as Zone);
		};
		callback.onInstantiate = delegate(Spatial a, ItemGeneral b)
		{
			b.SetSound(null);
			b.SetMainText(a.Name, null, true);
			b.Build();
		};
		baseList.callbacks = callback;
		foreach (Spatial spatial in ELayer.game.spatials.map.Values)
		{
			if (spatial is Zone && spatial.parent == ELayer.pc.currentZone.Region)
			{
				if (this.windows[1].idTab == 0)
				{
					if (!spatial.IsPlayerFaction)
					{
						continue;
					}
				}
				else if (spatial.IsPlayerFaction)
				{
					continue;
				}
				uilist.Add(spatial);
			}
		}
		uilist.Refresh(false);
	}

	public UIList listZone;

	public Sprite spriteFaction;

	public Sprite spriteFaith;

	public UIZoneInfo info;
}
