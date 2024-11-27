using System;
using System.Collections.Generic;
using UnityEngine;

public class ListPeopleExpedition : ListPeople
{
	public override bool ShowGoto
	{
		get
		{
			return false;
		}
	}

	public override bool ShowHome
	{
		get
		{
			return false;
		}
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		Expedition ex = this.expeditions[c.uid];
		if (!c.IsAliveInCurrentZone || !ex.costs.CanPay())
		{
			SE.Beep();
			return;
		}
		UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction().SetHighlightTarget(i);
		uicontextMenu.AddButton("sendExpedition", delegate()
		{
			EClass.Branch.expeditions.Add(ex);
			SE.MoveZone();
			this.list.List(false);
		}, true);
		uicontextMenu.Show();
	}

	public override void OnList()
	{
		foreach (Chara chara in EClass.Branch.members)
		{
			if (!chara.IsPCParty)
			{
				this.list.Add(chara);
				this.expeditions[chara.uid] = Expedition.Create(chara, this.type);
			}
		}
	}

	public override void SetSubText(Chara a, ItemGeneral b)
	{
		Expedition expedition = this.expeditions[a.uid];
		string lang = "";
		if (a.IsAliveInCurrentZone)
		{
			lang = expedition.costs.GetText() + "subExpedition".lang((expedition.MinHour / 24).ToString() ?? "", (expedition.MaxHour / 24).ToString() ?? "", null, null, null);
		}
		else if (a.isDead)
		{
			lang = "isDead".lang();
		}
		else
		{
			Zone currentZone = a.currentZone;
			if (((currentZone != null) ? currentZone.id : null) == "somewhere")
			{
				lang = "isExploring".lang();
			}
		}
		b.SetSubText(lang, 280, FontColor.Default, TextAnchor.MiddleLeft);
	}

	public Dictionary<int, Expedition> expeditions = new Dictionary<int, Expedition>();

	public ExpeditionType type;
}
