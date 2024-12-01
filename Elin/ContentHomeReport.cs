using System.Collections.Generic;

public class ContentHomeReport : Content
{
	public UIText textName;

	public UIText textRank;

	public UIText textWealth;

	public UIText textHearth;

	public UIText textPopu;

	public UIText textInfluence;

	public UIText textKnowledge;

	public UIText textHeaderReport;

	public UIText textReknown;

	public UIText textKarma;

	public UIButton buttonHappinessResident;

	public UIButton buttonHappinessLivestock;

	public UIList listCurrencies;

	public UIList listResources;

	public Faction faction;

	public FactionBranch branch;

	public Zone zone;

	public int tabResearch => 1;

	public override void OnSwitchContent(int idTab)
	{
		faction = EClass.pc.faction;
		branch = EClass.Branch ?? EClass.pc.homeZone.branch;
		zone = EClass.Branch?.owner ?? EClass.pc.homeZone;
	}

	public void RefreshInfo()
	{
		textName.SetText(faction.name + " <size=13>" + "_branch".lang(zone.Name) + "</size>");
		textRank.SetText(branch.RankText);
		textReknown.SetText("0");
		textKarma.SetText("20");
		textPopu.SetText((branch.rank != 0) ? branch.faith.Name : "none".lang());
		textHeaderReport.text = "headerHomeReport".lang(EClass.world.date.year.ToString() ?? "", EClass.world.date.month + "/" + EClass.world.date.day);
		buttonHappinessResident.mainText.text = branch.happiness.residents.GetText();
		buttonHappinessLivestock.mainText.text = branch.happiness.livestocks.GetText();
		buttonHappinessResident.subText.text = branch.CountMembers(FactionMemberType.Default).ToString() ?? "";
		buttonHappinessLivestock.subText.text = branch.CountMembers(FactionMemberType.Livestock).ToString() ?? "";
		buttonHappinessResident.SetTooltip(delegate(UITooltip t)
		{
			branch.happiness.residents.WriteNote(t.note);
		});
		buttonHappinessLivestock.SetTooltip(delegate(UITooltip t)
		{
			branch.happiness.livestocks.WriteNote(t.note);
		});
	}

	public string GetTextHappiness(List<Chara> list)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Chara item in list)
		{
			int happiness = item.GetHappiness();
			if (happiness >= 80)
			{
				num2++;
			}
			else if (happiness >= 30)
			{
				num3++;
			}
			else
			{
				num4++;
			}
			num += happiness;
		}
		int avg = ((list.Count != 0) ? (num / list.Count) : 0);
		string s = "(" + num2.ToString().TagColor(FontColor.Good) + "/" + num3.ToString().TagColor(FontColor.Default) + "/" + num4.ToString().TagColor(FontColor.Bad) + ")";
		return (((list.Count == 0) ? " - " : avg.ToString()) + "%").TagColorGoodBad(() => list.Count == 0 || avg >= 50) + " " + s.TagSize(14);
	}

	public void RefreshResources()
	{
		listCurrencies.Clear();
		listCurrencies.callbacks = new UIList.Callback<BaseHomeResource, ItemHomeResource>
		{
			onInstantiate = delegate(BaseHomeResource a, ItemHomeResource b)
			{
				b.SetResource(a);
			},
			onList = delegate
			{
				foreach (BaseHomeResource item in branch.resources.list)
				{
					if (item.IsCurrency)
					{
						listCurrencies.Add(item);
					}
				}
			}
		};
		listCurrencies.List();
		listResources.Clear();
		listResources.callbacks = new UIList.Callback<BaseHomeResource, ItemHomeResource>
		{
			onInstantiate = delegate(BaseHomeResource a, ItemHomeResource b)
			{
				b.SetResource(a);
			},
			onList = delegate
			{
				foreach (BaseHomeResource item2 in branch.resources.list)
				{
					if (item2.IsRate)
					{
						listResources.Add(item2);
					}
				}
			}
		};
		listResources.List();
	}
}
