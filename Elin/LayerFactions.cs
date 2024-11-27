using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerFactions : ELayer
{
	public override bool HeaderIsListOf(int id)
	{
		return true;
	}

	public override void OnSwitchContent(Window window)
	{
		if (window.windowIndex != 1)
		{
			return;
		}
		if (window.idTab == 2)
		{
			this.RefreshZones();
			return;
		}
		this.RefreshFactions();
	}

	public void RefreshFactions()
	{
		UIList uilist = this.listFaction;
		uilist.Clear();
		uilist.callbacks = new UIList.Callback<Faction, ItemGeneral>
		{
			onClick = delegate(Faction a, ItemGeneral b)
			{
				this.info.SetFaction(a);
			},
			onInstantiate = delegate(Faction a, ItemGeneral b)
			{
				b.SetSound(null);
				b.SetMainText(a.name, (this.windows[1].idTab == 0) ? this.spriteFaction : this.spriteFaith, true);
				b.SetSubText(a.TextType, 260, FontColor.Default, TextAnchor.MiddleRight);
				a.relation.SetTextHostility(b.button1.subText);
				b.Build();
			}
		};
		if (this.windows[1].idTab == 0)
		{
			using (Dictionary<string, Faction>.ValueCollection.Enumerator enumerator = ELayer.game.factions.dictAll.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Faction o = enumerator.Current;
					uilist.Add(o);
				}
				goto IL_D9;
			}
		}
		foreach (Religion o2 in ELayer.game.religions.dictAll.Values)
		{
			uilist.Add(o2);
		}
		IL_D9:
		uilist.Refresh(false);
	}

	public void RefreshZones()
	{
		UIList uilist = this.listFaction;
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
				uilist.Add(spatial);
			}
		}
		uilist.Refresh(false);
	}

	public UIList listFaction;

	public UIFactionInfo info;

	public Sprite spriteFaction;

	public Sprite spriteFaith;
}
