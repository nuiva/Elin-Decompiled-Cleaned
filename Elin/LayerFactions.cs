using UnityEngine;

public class LayerFactions : ELayer
{
	public UIList listFaction;

	public UIFactionInfo info;

	public Sprite spriteFaction;

	public Sprite spriteFaith;

	public override bool HeaderIsListOf(int id)
	{
		return true;
	}

	public override void OnSwitchContent(Window window)
	{
		if (window.windowIndex == 1)
		{
			if (window.idTab == 2)
			{
				RefreshZones();
			}
			else
			{
				RefreshFactions();
			}
		}
	}

	public void RefreshFactions()
	{
		UIList uIList = listFaction;
		uIList.Clear();
		uIList.callbacks = new UIList.Callback<Faction, ItemGeneral>
		{
			onClick = delegate(Faction a, ItemGeneral b)
			{
				info.SetFaction(a);
			},
			onInstantiate = delegate(Faction a, ItemGeneral b)
			{
				b.SetSound();
				b.SetMainText(a.name, (windows[1].idTab == 0) ? spriteFaction : spriteFaith);
				b.SetSubText(a.TextType, 260, FontColor.Default, TextAnchor.MiddleRight);
				a.relation.SetTextHostility(b.button1.subText);
				b.Build();
			}
		};
		if (windows[1].idTab == 0)
		{
			foreach (Faction value in ELayer.game.factions.dictAll.Values)
			{
				uIList.Add(value);
			}
		}
		else
		{
			foreach (Religion value2 in ELayer.game.religions.dictAll.Values)
			{
				uIList.Add(value2);
			}
		}
		uIList.Refresh();
	}

	public void RefreshZones()
	{
		UIList uIList = listFaction;
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
			if (value is Zone && value.parent == ELayer.pc.currentZone.Region)
			{
				uIList.Add(value);
			}
		}
		uIList.Refresh();
	}
}
