using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeInfo : EMono
{
	public int tabResearch
	{
		get
		{
			return 1;
		}
	}

	public void Refresh()
	{
		EMono._zone.RefreshElectricity();
		this.reports.Clear();
		this.faction = EMono.pc.faction;
		this.branch = (EMono.Branch ?? EMono.pc.homeZone.branch);
		FactionBranch factionBranch = EMono.Branch;
		this.zone = (((factionBranch != null) ? factionBranch.owner : null) ?? EMono.pc.homeZone);
		this.branch.resources.Refresh();
		this.RefreshInfo();
		this.RefreshResources();
		this.RefreshReport();
		this.transLayout.RebuildLayoutTo(this);
	}

	public void RefreshInfo()
	{
		this.textHeaderReport.text = "headerHomeReport".lang(EMono.world.date.year.ToString() ?? "", EMono.world.date.month.ToString() + "/" + EMono.world.date.day.ToString(), null, null, null);
		this.textFaction.text = EMono.pc.faction.Name;
		this.textGod.text = this.branch.faith.Name;
		this.textContentLv.text = this.branch.DangerLV.ToString() + " " + ("(" + this.branch.ContentLV.ToString() + ")").TagSize(12);
		this.textDateFound.text = Date.GetText(this.branch.dateFound, Date.TextFormat.YearMonthDay);
		this.textPasture.text = this.branch.CountPasture().ToString() + " " + ("(-" + this.branch.GetPastureCost().ToString() + "dailyIncome".lang() + ")").TagSize(12);
		bool flag = this.branch.CountMembers(FactionMemberType.Default, false) > this.branch.MaxPopulation;
		this.textMember.SetText(this.branch.CountMembers(FactionMemberType.Default, false).ToString() + " / " + this.branch.MaxPopulation.ToString(), flag ? FontColor.Bad : FontColor.Default);
		if (flag)
		{
			this.AddReport("rPopOver", UIHomeInfo.ReportData.ReportType.Warning, null, null);
		}
		this.textDev.text = ((EMono._zone.development / 10).ToString() ?? "");
		this.textLand.text = (EMono._map.bounds.Width * EMono._map.bounds.Height / 100).ToString() + " ㎢";
		int civility = this.branch.GetCivility();
		this.textCivility.text = civility.ToString() + " " + ("(" + Lang.GetList("civility")[Mathf.Clamp(civility / 20, 0, 4)] + ")").TagSize(12);
		int electricity = EMono._zone.GetElectricity(true);
		int electricity2 = EMono._zone.GetElectricity(false);
		this.textElec.SetText("elecLv".lang(electricity2.ToString() ?? "", (electricity.ToString() ?? "").TagColor((electricity <= electricity2) ? FontColor.Good : FontColor.Bad, null), null, null, null));
		if (electricity > electricity2)
		{
			this.AddReport("rElecOver", UIHomeInfo.ReportData.ReportType.Warning, null, null);
		}
		int soilCost = this.zone.GetSoilCost();
		int maxSoil = this.branch.MaxSoil;
		this.textSoil.SetText("soilLv".lang(maxSoil.ToString() ?? "", ((maxSoil - soilCost).ToString() ?? "").TagColor((soilCost <= maxSoil) ? FontColor.Good : FontColor.Bad, null), null, null, null));
		if (soilCost > maxSoil)
		{
			this.AddReport("rSoilOver", UIHomeInfo.ReportData.ReportType.Warning, null, null);
		}
		this.textName.SetText(this.zone.Name);
		this.textRank.SetText(this.branch.RankText);
		this.textPopu.SetText(this.faction.name);
		this.textFaith.SetText(this.branch.faith.Name + Environment.NewLine + (" ♥" + this.branch.RankText).TagSize(13));
		this.imageFaith.sprite = this.branch.faith.GetSprite();
		this.textTemper.SetText(this.branch.faith.GetTextTemper(this.branch.temper));
		this.barTemper.fillAmount = 0.5f + (float)this.branch.temper * 0.01f * 0.5f;
		this.barTemper.color = ((this.branch.temper > -25) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad);
		if (this.buttonWorth)
		{
			this.buttonWorth.mainText.SetText(this.branch.Worth.ToFormat());
			this.buttonWorth.SetTooltip(delegate(UITooltip t)
			{
				this.branch.resources.worth.WriteNote(t.note);
			}, true);
		}
		this.buttonLv.mainText.SetText(this.branch.lv.ToString() ?? "");
		this.buttonLv.SetTooltip(delegate(UITooltip t)
		{
			UIHomeInfo.<>c__DisplayClass38_0 CS$<>8__locals1;
			CS$<>8__locals1.n = t.note;
			CS$<>8__locals1.n.Clear();
			CS$<>8__locals1.n.AddHeader("branchLv".lang(), null);
			CS$<>8__locals1.n.AddTopic("TopicLeft", "vCurrent".lang(), EMono.Branch.TextLv);
			CS$<>8__locals1.n.Space(0, 1);
			this.<RefreshInfo>g__Write|38_6(this.branch.lv, ref CS$<>8__locals1);
			if (this.branch.lv < this.branch.MaxLv)
			{
				CS$<>8__locals1.n.Space(0, 1);
				CS$<>8__locals1.n.AddHeaderTopic("nextLevel", null);
				this.<RefreshInfo>g__Write|38_6(this.branch.lv + 1, ref CS$<>8__locals1);
			}
			t.note.Build();
		}, true);
		this.buttonRanking.mainText.SetText(EMono.game.spatials.ranks.GetRankText(EMono._zone));
		this.buttonRanking.SetTooltip(delegate(UITooltip t)
		{
			UINote note = t.note;
			note.Clear();
			note.AddHeader("homeRanking".lang(), null);
			note.AddTopic("TopicLeft", "vCurrent".lang(), EMono.game.spatials.ranks.GetRankText(EMono._zone));
			note.Space(0, 1);
			note.AddHeader("HeaderTopic", "rank_income".lang(), null);
			note.Space(1, 1);
			note.AddText("rank_income2".lang(EMono.game.spatials.ranks.GetIncome(EMono._zone).ToFormat(), null, null, null, null), FontColor.DontChange);
			t.note.Build();
		}, true);
		this.buttonRank.mainText.SetText(this.branch.rank.ToString() ?? "");
		this.buttonRank.SetTooltip(delegate(UITooltip t)
		{
			UINote note = t.note;
			note.Clear();
			note.AddHeader("factionRank2".lang(), null);
			note.AddTopic("TopicLeft", "vCurrent".lang(), this.branch.rank.ToString() ?? "");
			t.note.Build();
		}, true);
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
		return (((list.Count == 0) ? " - " : avg.ToString()) + "%").TagColorGoodBad(() => list.Count == 0 || avg >= 50, false) + " " + s.TagSize(13);
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
				if (baseHomeResource.IsCurrency && baseHomeResource.IsAvailable)
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
				if (baseHomeResource.IsRate && baseHomeResource.IsAvailable)
				{
					this.listResources.Add(baseHomeResource);
				}
			}
		};
		baseList2.callbacks = callback2;
		this.listResources.List(false);
		this.listSkill.Clear();
		BaseList baseList3 = this.listSkill;
		UIList.Callback<BaseHomeResource, ItemHomeResource> callback3 = new UIList.Callback<BaseHomeResource, ItemHomeResource>();
		callback3.onInstantiate = delegate(BaseHomeResource a, ItemHomeResource b)
		{
			b.SetResource(a);
		};
		callback3.onList = delegate(UIList.SortMode m)
		{
			foreach (BaseHomeResource baseHomeResource in this.branch.resources.list)
			{
				if (baseHomeResource.IsSkill)
				{
					this.listSkill.Add(baseHomeResource);
				}
			}
		};
		baseList3.callbacks = callback3;
		this.listSkill.List(false);
	}

	public void AddReport(string text, UIHomeInfo.ReportData.ReportType type = UIHomeInfo.ReportData.ReportType.Default, Action onClick = null, Action<UITooltip> onShowTooltip = null)
	{
		UIHomeInfo.ReportData item = new UIHomeInfo.ReportData
		{
			text = text.lang(),
			type = type,
			onClick = onClick,
			onShowTooltip = onShowTooltip
		};
		this.reports.Add(item);
	}

	public void RefreshReport()
	{
		if (EMono.Branch.meetings.list.Count > 0)
		{
			this.AddReport("rMeeting".lang(EMono.Branch.meetings.list.Count.ToString() ?? "", null, null, null, null), UIHomeInfo.ReportData.ReportType.Default, null, null);
		}
		foreach (ZoneEvent zoneEvent in this.branch.owner.events.list)
		{
			if (zoneEvent.HasReport)
			{
				this.AddReport(zoneEvent.GetText(), UIHomeInfo.ReportData.ReportType.Trial, null, null);
			}
		}
		foreach (ResearchPlan researchPlan in this.branch.researches.newPlans)
		{
			this.AddReport("rNewPlan".lang(researchPlan.Name ?? "", null, null, null, null), UIHomeInfo.ReportData.ReportType.Good, delegate
			{
			}, null);
		}
		foreach (ResearchPlan researchPlan2 in this.branch.researches.plans)
		{
			if (researchPlan2.exp == researchPlan2.MaxExp)
			{
				this.AddReport("vResearched".lang(researchPlan2.Name, null, null, null, null), UIHomeInfo.ReportData.ReportType.Great, delegate
				{
				}, null);
			}
		}
		foreach (Chara chara in this.branch.members)
		{
			if (!chara.isDead && !chara.IsPCParty && chara.memberType == FactionMemberType.Default && !chara.noMove)
			{
				bool flag = false;
				if (EMono.game.activeZone == this.branch.owner)
				{
					using (List<Hobby>.Enumerator enumerator4 = chara.ListWorks(true).GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							if (enumerator4.Current.GetEfficiency(chara) > 0)
							{
								flag = true;
								break;
							}
						}
						goto IL_26A;
					}
					goto IL_259;
				}
				goto IL_259;
				IL_26A:
				if (!flag)
				{
					this.AddReport("rNoWork".lang(chara.Name, null, null, null, null).ToTitleCase(false), UIHomeInfo.ReportData.ReportType.Warning, null, null);
					continue;
				}
				continue;
				IL_259:
				if (chara.GetWorkSummary().work != null)
				{
					flag = true;
					goto IL_26A;
				}
				goto IL_26A;
			}
		}
		if (EMono.player.taxBills > 0)
		{
			this.AddReport("rBill".lang(EMono.player.taxBills.ToString() ?? "", null, null, null, null), UIHomeInfo.ReportData.ReportType.Warning, null, null);
		}
		if (this.reports.Count == 0)
		{
			this.AddReport("rNoReport", UIHomeInfo.ReportData.ReportType.Good, null, null);
		}
		this.AddReport("rTax".lang((30 - EMono.world.date.day + 1).ToString() ?? "", this.faction.GetTotalTax(false).ToFormat(), null, null, null), UIHomeInfo.ReportData.ReportType.Default, null, delegate(UITooltip t)
		{
			t.note.Clear();
			this.faction.SetTaxTooltip(t.note);
			t.note.Build();
		});
		if (EMono.debug.showExtra)
		{
			this.AddReport("efficiency:" + this.branch.efficiency.ToString(), UIHomeInfo.ReportData.ReportType.Default, null, null);
		}
		this.reports.Sort((UIHomeInfo.ReportData a, UIHomeInfo.ReportData b) => b.type - a.type);
		this.listReport.Clear();
		BaseList baseList = this.listReport;
		UIList.Callback<UIHomeInfo.ReportData, ItemGeneral> callback = new UIList.Callback<UIHomeInfo.ReportData, ItemGeneral>();
		callback.onClick = delegate(UIHomeInfo.ReportData a, ItemGeneral b)
		{
			if (a.onClick != null)
			{
				a.onClick();
			}
		};
		callback.onInstantiate = delegate(UIHomeInfo.ReportData a, ItemGeneral b)
		{
			b.SetMainText(a.text, this.iconReports[(int)a.type], true);
			b.button1.mainText.SetColor(a.GetFontColor());
			if (a.onClick != null)
			{
				b.AddSubButton(EMono.core.refs.icons.go, a.onClick, null, null);
			}
			b.Build();
			if (a.onShowTooltip != null)
			{
				b.button1.SetTooltip(a.onShowTooltip, true);
			}
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (UIHomeInfo.ReportData o in this.reports)
			{
				this.listReport.Add(o);
			}
		};
		callback.onRefresh = null;
		baseList.callbacks = callback;
		this.listReport.List(false);
	}

	[CompilerGenerated]
	private void <RefreshInfo>g__Write|38_6(int lv, ref UIHomeInfo.<>c__DisplayClass38_0 A_2)
	{
		string[] array = this.branch.GetHearthHint(lv).SplitNewline();
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Replace(Environment.NewLine, "");
			if (!text.IsEmpty())
			{
				A_2.n.AddText(text, FontColor.DontChange);
			}
		}
	}

	public Transform transLayout;

	public UIText textFaction;

	public UIText textDateFound;

	public UIText textGod;

	public UIText textMember;

	public UIText textLand;

	public UIText textHeaderReport;

	public UIText textDev;

	public UIText textElec;

	public UIText textCivility;

	public UIText textName;

	public UIText textRank;

	public UIText textPopu;

	public UIText textFaith;

	public UIText textTemper;

	public UIText textSoil;

	public UIText textContentLv;

	public UIText textPasture;

	public UIButton buttonHappinessResident;

	public UIButton buttonHappinessLivestock;

	public UIButton buttonWorth;

	public UIButton buttonRank;

	public UIButton buttonLv;

	public UIButton buttonRanking;

	public UIList listCurrencies;

	public UIList listResources;

	public UIList listSkill;

	public UIList listReport;

	public Faction faction;

	public FactionBranch branch;

	public Zone zone;

	public Image imageFaith;

	public Image barTemper;

	public List<UIHomeInfo.ReportData> reports = new List<UIHomeInfo.ReportData>();

	public Sprite[] iconReports;

	public class ReportData
	{
		public FontColor GetFontColor()
		{
			switch (this.type)
			{
			case UIHomeInfo.ReportData.ReportType.Warning:
				return FontColor.Warning;
			case UIHomeInfo.ReportData.ReportType.Critical:
				return FontColor.Bad;
			case UIHomeInfo.ReportData.ReportType.Good:
				return FontColor.Good;
			case UIHomeInfo.ReportData.ReportType.Great:
				return FontColor.Good;
			case UIHomeInfo.ReportData.ReportType.Trial:
				return FontColor.Great;
			default:
				return FontColor.Default;
			}
		}

		public UIHomeInfo.ReportData.ReportType type;

		public string text;

		public Action onClick;

		public Action<UITooltip> onShowTooltip;

		public enum ReportType
		{
			Default,
			Warning,
			Critical,
			Good,
			Great,
			Trial
		}
	}
}
