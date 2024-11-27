using System;
using System.Collections.Generic;

public class ContentHomeReport : Content
{
	public int tabResearch
	{
		get
		{
			return 1;
		}
	}

	public override void OnSwitchContent(int idTab)
	{
		this.faction = EClass.pc.faction;
		this.branch = (EClass.Branch ?? EClass.pc.homeZone.branch);
		FactionBranch factionBranch = EClass.Branch;
		this.zone = (((factionBranch != null) ? factionBranch.owner : null) ?? EClass.pc.homeZone);
	}

	public void RefreshInfo()
	{
		this.textName.SetText(this.faction.name + " <size=13>" + "_branch".lang(this.zone.Name, null, null, null, null) + "</size>");
		this.textRank.SetText(this.branch.RankText);
		this.textReknown.SetText("0");
		this.textKarma.SetText("20");
		this.textPopu.SetText((this.branch.rank != 0) ? this.branch.faith.Name : "none".lang());
		this.textHeaderReport.text = "headerHomeReport".lang(EClass.world.date.year.ToString() ?? "", EClass.world.date.month.ToString() + "/" + EClass.world.date.day.ToString(), null, null, null);
		this.buttonHappinessResident.mainText.text = this.branch.happiness.residents.GetText();
		this.buttonHappinessLivestock.mainText.text = this.branch.happiness.livestocks.GetText();
		this.buttonHappinessResident.subText.text = (this.branch.CountMembers(FactionMemberType.Default, false).ToString() ?? "");
		this.buttonHappinessLivestock.subText.text = (this.branch.CountMembers(FactionMemberType.Livestock, false).ToString() ?? "");
		this.buttonHappinessResident.SetTooltip(delegate(UITooltip t)
		{
			this.branch.happiness.residents.WriteNote(t.note);
		}, true);
		this.buttonHappinessLivestock.SetTooltip(delegate(UITooltip t)
		{
			this.branch.happiness.livestocks.WriteNote(t.note);
		}, true);
	}

	public string GetTextHappiness(List<Chara> list)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Chara chara in list)
		{
			int happiness = chara.GetHappiness();
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
		int avg = (list.Count == 0) ? 0 : (num / list.Count);
		string s = string.Concat(new string[]
		{
			"(",
			num2.ToString().TagColor(FontColor.Good, null),
			"/",
			num3.ToString().TagColor(FontColor.Default, null),
			"/",
			num4.ToString().TagColor(FontColor.Bad, null),
			")"
		});
		return (((list.Count == 0) ? " - " : avg.ToString()) + "%").TagColorGoodBad(() => list.Count == 0 || avg >= 50, false) + " " + s.TagSize(14);
	}

	public void RefreshResources()
	{
		this.listCurrencies.Clear();
		BaseList baseList = this.listCurrencies;
		UIList.Callback<BaseHomeResource, ItemHomeResource> callback = new UIList.Callback<BaseHomeResource, ItemHomeResource>();
		callback.onInstantiate = delegate(BaseHomeResource a, ItemHomeResource b)
		{
			b.SetResource(a);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (BaseHomeResource baseHomeResource in this.branch.resources.list)
			{
				if (baseHomeResource.IsCurrency)
				{
					this.listCurrencies.Add(baseHomeResource);
				}
			}
		};
		baseList.callbacks = callback;
		this.listCurrencies.List(false);
		this.listResources.Clear();
		BaseList baseList2 = this.listResources;
		UIList.Callback<BaseHomeResource, ItemHomeResource> callback2 = new UIList.Callback<BaseHomeResource, ItemHomeResource>();
		callback2.onInstantiate = delegate(BaseHomeResource a, ItemHomeResource b)
		{
			b.SetResource(a);
		};
		callback2.onList = delegate(UIList.SortMode m)
		{
			foreach (BaseHomeResource baseHomeResource in this.branch.resources.list)
			{
				if (baseHomeResource.IsRate)
				{
					this.listResources.Add(baseHomeResource);
				}
			}
		};
		baseList2.callbacks = callback2;
		this.listResources.List(false);
	}

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
}
