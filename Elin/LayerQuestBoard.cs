using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class LayerQuestBoard : ELayer
{
	public override void OnInit()
	{
		if (ELayer._zone.IsPCFaction)
		{
			ELayer._zone.branch.UpdateReqruits(false);
		}
		this.windows[0].setting.tabs[1].disable = !ELayer._zone.IsPCFaction;
	}

	public override void OnSwitchContent(Window window)
	{
		switch (window.idTab)
		{
		case 0:
			this.RefreshQuest();
			break;
		case 1:
			this.RefreshHire();
			break;
		case 2:
			this.RefreshRanking();
			break;
		}
		WindowMenu menuRight = this.windows[0].menuRight;
		menuRight.Clear();
		if (window.idTab == 0)
		{
			menuRight.AddButton("rerollQuest".lang(1.ToString() ?? "", null, null, null, null), delegate(UIButton b)
			{
				if (ELayer._zone.influence < 1)
				{
					SE.Beep();
					Msg.Say("notEnoughInfluence");
					return;
				}
				SE.Dice();
				ELayer._zone.UpdateQuests(true);
				this.RefreshQuest();
				Zone zone = ELayer._zone;
				int influence = zone.influence;
				zone.influence = influence - 1;
			}, null, "Default");
		}
	}

	public void RefreshQuest()
	{
		this.list.Clear();
		this.list.callbacks = new UIList.Callback<Quest, ItemQuest>
		{
			onInstantiate = delegate(Quest a, ItemQuest b)
			{
				b.SetQuest(a);
				b.button.SetOnClick(delegate
				{
					this.Close();
					a.OnClickQuest();
				});
			}
		};
		ELayer._zone.UpdateQuests(false);
		foreach (Quest quest in ELayer.game.quests.globalList)
		{
			if (quest.IsVisibleOnQuestBoard())
			{
				this.list.Add(quest);
			}
		}
		foreach (Chara chara in ELayer._map.charas)
		{
			if (chara.quest != null && !ELayer.game.quests.list.Contains(chara.quest) && !chara.IsPCParty && chara.memberType == FactionMemberType.Default && chara.quest.IsVisibleOnQuestBoard())
			{
				this.list.Add(chara.quest);
			}
		}
		this.list.Refresh(false);
		this.textQuestLimit.SetText("questLimit".lang(ELayer.game.quests.CountRandomQuest().ToString() ?? "", 5.ToString() ?? "", null, null, null));
	}

	public void RefreshHire()
	{
		UIList list = this.listHire;
		list.Clear();
		list.callbacks = new UIList.Callback<HireInfo, ButtonChara>
		{
			onClick = delegate(HireInfo a, ButtonChara b)
			{
				a.isNew = false;
				GameLang.refDrama1 = string.Concat(new string[]
				{
					"(",
					"money2".lang(),
					": ",
					CalcGold.Hire(a.chara).ToString(),
					")"
				});
				Thing thing = ELayer.pc.things.Find("ticket_resident", -1, -1);
				GameLang.refDrama2 = (((thing != null) ? thing.Name : null) ?? "");
				a.chara.ShowDialog("_chara", "4-1", "").onKill.AddListener(new UnityAction(this.RefreshHire));
			},
			onInstantiate = delegate(HireInfo a, ButtonChara b)
			{
				Chara chara = a.chara;
				b.SetChara(chara, ButtonChara.Mode.Hire);
				int cost = CalcGold.Hire(chara);
				b.item.text1.text = cost.ToString().TagColorGoodBad(() => ELayer.pc.GetCurrency("money2") >= cost || ELayer.pc.things.Find("ticket_resident", -1, -1) != null, false);
				b.item.text2.text = ((a.Days == -1) ? "-" : (a.Days.ToString() ?? ""));
				Action <>9__3;
				foreach (HintIcon hintIcon in b.layoutTag.GetComponentsInChildren<HintIcon>())
				{
					Action onPointerDown;
					if ((onPointerDown = <>9__3) == null)
					{
						onPointerDown = (<>9__3 = delegate()
						{
							list.callbacks.OnClick(a, b);
						});
					}
					hintIcon.onPointerDown = onPointerDown;
				}
				b.textHobby.text = chara.GetTextHobby(false);
				b.textWork.text = chara.GetTextWork(false);
				b.textLifeStyle.text = "lifestyle".lang() + ": " + ("lifestyle_" + chara.idTimeTable).lang();
				b.item.image1.SetActive(a.isNew);
			},
			onRefresh = null
		};
		foreach (HireInfo hireInfo in ELayer.Branch.listRecruit)
		{
			if (!hireInfo.chara.IsHomeMember() && (hireInfo.chara.currentZone == null || !hireInfo.chara.currentZone.IsPlayerFaction))
			{
				list.Add(hireInfo);
			}
		}
		list.Refresh(false);
		this.RebuildLayout(true);
	}

	public void RefreshRanking()
	{
		UIDynamicList list = this.listRanking;
		list.Clear();
		BaseList baseList = list;
		UIList.Callback<Chara, ButtonChara> callback = new UIList.Callback<Chara, ButtonChara>();
		callback.onClick = delegate(Chara a, ButtonChara b)
		{
		};
		callback.onRedraw = delegate(Chara a, ButtonChara b, int i)
		{
			b.SetChara(a, ButtonChara.Mode.Ranking);
			int advRank = a.trait.GetAdvRank();
			b.textAlias.SetText(a.trait.GetAdvRankText());
			b.textAlias.color = this.gradientRank.Evaluate(0.1f * (float)advRank);
			b.item.image2.SetActive(a.IsPCFaction);
			bool enable = a.GetInt(111, null) > 0;
			b.transDefeated.SetActive(enable);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Chara chara in ELayer.game.cards.globalCharas.Values)
			{
				if (chara.trait.ShowAdvRank)
				{
					list.Add(chara);
				}
			}
			list.objects.Sort((object a, object b) => LayerQuestBoard.<RefreshRanking>g__GetSortVal|18_0(b as Chara) - LayerQuestBoard.<RefreshRanking>g__GetSortVal|18_0(a as Chara));
		};
		callback.onRefresh = null;
		baseList.callbacks = callback;
		list.List();
		int num = list.objects.IndexOf(ELayer.pc);
		if (num != -1)
		{
			list.dsv.scrollByItemIndex(num);
			list.Refresh();
		}
		this.RebuildLayout(true);
	}

	public override void OnKill()
	{
		if (ELayer.Branch != null)
		{
			ELayer.Branch.ClearNewRecruits();
		}
	}

	[CompilerGenerated]
	internal static int <RefreshRanking>g__GetSortVal|18_0(Chara c)
	{
		return c.trait.GetAdvRank() * 10000 + Mathf.Clamp(c.LV, 0, 1000) + ((c.trait is TraitAdventurerBacker) ? 1000 : 0);
	}

	public UIList list;

	public UIList listHire;

	public UIDynamicList listRanking;

	public UIText textClient;

	public UIText textTitle;

	public UIText textDetail;

	public UIText textHours;

	public UIText textNote;

	public UIText textReward;

	public UIText textQuestLimit;

	public Portrait portrait;

	public UIButton buttonTalk;

	public UICurrency uiCurrency;

	public Gradient gradientRank;
}
