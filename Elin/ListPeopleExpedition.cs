using System.Collections.Generic;

public class ListPeopleExpedition : ListPeople
{
	public Dictionary<int, Expedition> expeditions = new Dictionary<int, Expedition>();

	public ExpeditionType type;

	public override bool ShowGoto => false;

	public override bool ShowHome => false;

	public override void OnClick(Chara c, ItemGeneral i)
	{
		Expedition ex = expeditions[c.uid];
		if (!c.IsAliveInCurrentZone || !ex.costs.CanPay())
		{
			SE.Beep();
			return;
		}
		UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction().SetHighlightTarget(i);
		uIContextMenu.AddButton("sendExpedition", delegate
		{
			EClass.Branch.expeditions.Add(ex);
			SE.MoveZone();
			list.List();
		});
		uIContextMenu.Show();
	}

	public override void OnList()
	{
		foreach (Chara member in EClass.Branch.members)
		{
			if (!member.IsPCParty)
			{
				list.Add(member);
				expeditions[member.uid] = Expedition.Create(member, type);
			}
		}
	}

	public override void SetSubText(Chara a, ItemGeneral b)
	{
		Expedition expedition = expeditions[a.uid];
		string lang = "";
		if (a.IsAliveInCurrentZone)
		{
			lang = expedition.costs.GetText() + "subExpedition".lang((expedition.MinHour / 24).ToString() ?? "", (expedition.MaxHour / 24).ToString() ?? "");
		}
		else if (a.isDead)
		{
			lang = "isDead".lang();
		}
		else if (a.currentZone?.id == "somewhere")
		{
			lang = "isExploring".lang();
		}
		b.SetSubText(lang, 280);
	}
}
