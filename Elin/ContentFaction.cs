using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentFaction : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		if (idTab == 2)
		{
			this.RefreshZones();
		}
		else
		{
			this.RefreshFactions(idTab == 4);
		}
		LayerJournal componentInParent = base.GetComponentInParent<LayerJournal>();
		this.textHeader.SetText(componentInParent.GetTextHeader(componentInParent.windows[0]));
	}

	public void RefreshFactions(bool religion)
	{
		UIList uilist = this.listFaction;
		uilist.Clear();
		if (religion)
		{
			uilist.callbacks = new UIList.Callback<Religion, ItemGeneral>
			{
				onClick = delegate(Religion a, ItemGeneral b)
				{
					this.info.SetReligion(a);
				},
				onInstantiate = delegate(Religion a, ItemGeneral b)
				{
					b.SetSound(null);
					b.SetMainText(a.Name, this.spriteFaith, true);
					b.SetSubText(a.TextType, 260, FontColor.Default, TextAnchor.MiddleRight);
					a.SetTextRelation(b.button1.subText);
					b.Build();
					b.button1.refStr = a.id;
				}
			};
			using (Dictionary<string, Religion>.ValueCollection.Enumerator enumerator = EClass.game.religions.dictAll.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Religion religion2 = enumerator.Current;
					if (!religion2.IsMinorGod)
					{
						uilist.Add(religion2);
					}
				}
				goto IL_103;
			}
		}
		uilist.callbacks = new UIList.Callback<Faction, ItemGeneral>
		{
			onClick = delegate(Faction a, ItemGeneral b)
			{
				this.info.SetFaction(a);
			},
			onInstantiate = delegate(Faction a, ItemGeneral b)
			{
				b.SetSound(null);
				b.SetMainText(a.name, this.spriteFaction, true);
				b.SetSubText(a.TextType, 260, FontColor.Default, TextAnchor.MiddleRight);
				a.relation.SetTextHostility(b.button1.subText);
				b.Build();
				b.button1.refStr = a.id;
			}
		};
		foreach (Faction o in EClass.game.factions.dictAll.Values)
		{
			uilist.Add(o);
		}
		IL_103:
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
		foreach (Spatial spatial in EClass.game.spatials.map.Values)
		{
			if (spatial is Zone && spatial.parent == EClass.pc.currentZone.Region && spatial.mainFaction == EClass.pc.faction)
			{
				uilist.Add(spatial);
			}
		}
		uilist.Refresh(false);
		if (uilist.items.Count == 0)
		{
			this.info.Clear();
		}
	}

	public UIList listFaction;

	public UIFactionInfo info;

	public Sprite spriteFaction;

	public Sprite spriteFaith;

	public UIText textHeader;
}
