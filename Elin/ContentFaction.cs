using UnityEngine;

public class ContentFaction : EContent
{
	public UIList listFaction;

	public UIFactionInfo info;

	public Sprite spriteFaction;

	public Sprite spriteFaith;

	public UIText textHeader;

	public override void OnSwitchContent(int idTab)
	{
		if (idTab == 2)
		{
			RefreshZones();
		}
		else
		{
			RefreshFactions(idTab == 4);
		}
		LayerJournal componentInParent = GetComponentInParent<LayerJournal>();
		textHeader.SetText(componentInParent.GetTextHeader(componentInParent.windows[0]));
	}

	public void RefreshFactions(bool religion)
	{
		UIList uIList = listFaction;
		uIList.Clear();
		if (religion)
		{
			uIList.callbacks = new UIList.Callback<Religion, ItemGeneral>
			{
				onClick = delegate(Religion a, ItemGeneral b)
				{
					info.SetReligion(a);
				},
				onInstantiate = delegate(Religion a, ItemGeneral b)
				{
					b.SetSound();
					b.SetMainText(a.Name, spriteFaith);
					b.SetSubText(a.TextType, 260, FontColor.Default, TextAnchor.MiddleRight);
					a.SetTextRelation(b.button1.subText);
					b.Build();
					b.button1.refStr = a.id;
				}
			};
			foreach (Religion value in EClass.game.religions.dictAll.Values)
			{
				if (!value.IsMinorGod)
				{
					uIList.Add(value);
				}
			}
		}
		else
		{
			uIList.callbacks = new UIList.Callback<Faction, ItemGeneral>
			{
				onClick = delegate(Faction a, ItemGeneral b)
				{
					info.SetFaction(a);
				},
				onInstantiate = delegate(Faction a, ItemGeneral b)
				{
					b.SetSound();
					b.SetMainText(a.name, spriteFaction);
					b.SetSubText(a.TextType, 260, FontColor.Default, TextAnchor.MiddleRight);
					a.relation.SetTextHostility(b.button1.subText);
					b.Build();
					b.button1.refStr = a.id;
				}
			};
			foreach (Faction value2 in EClass.game.factions.dictAll.Values)
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
		foreach (Spatial value in EClass.game.spatials.map.Values)
		{
			if (value is Zone && value.parent == EClass.pc.currentZone.Region && value.mainFaction == EClass.pc.faction)
			{
				uIList.Add(value);
			}
		}
		uIList.Refresh();
		if (uIList.items.Count == 0)
		{
			info.Clear();
		}
	}
}
