using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DramaCustomSequence : EClass
{
	public string StepDefault
	{
		get
		{
			return this.idDefault;
		}
	}

	public string StepEnd
	{
		get
		{
			return "end";
		}
	}

	public void Build(Chara c)
	{
		DramaCustomSequence.<>c__DisplayClass14_0 CS$<>8__locals1 = new DramaCustomSequence.<>c__DisplayClass14_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.c = c;
		bool flag = this.idCustom == "Unique";
		bool flag2 = CS$<>8__locals1.c.bio.IsUnderAge || EClass.pc.bio.IsUnderAge;
		CS$<>8__locals1.isInGuild = (Guild.Fighter.IsCurrentZone || Guild.Mage.IsCurrentZone || Guild.Thief.IsCurrentZone || Guild.Merchant.IsCurrentZone);
		CS$<>8__locals1.bird = (flag2 ? "bird" : "tail");
		string name = CS$<>8__locals1.c.Name;
		CS$<>8__locals1.rumor = (CS$<>8__locals1.c.IsPCParty ? CS$<>8__locals1.<Build>g__GetTalk|6("sup") : this.GetRumor(CS$<>8__locals1.c));
		CS$<>8__locals1.layer = null;
		bool flag3 = CS$<>8__locals1.c.IsHuman || EClass.pc.HasElement(1640, 1);
		if (!flag)
		{
			this.Step("Resident");
			this._Talk("tg", () => CS$<>8__locals1.rumor, null);
			if (flag3)
			{
				DramaChoice choice = this.Choice2("letsTalk", this.StepDefault);
				choice.SetOnClick(delegate
				{
					DramaEventTalk firstTalk = CS$<>8__locals1.<>4__this.sequence.firstTalk;
					Func<string> funcText;
					if ((funcText = CS$<>8__locals1.<>9__92) == null)
					{
						funcText = (CS$<>8__locals1.<>9__92 = (() => CS$<>8__locals1.rumor));
					}
					firstTalk.funcText = funcText;
					List<Hobby> list = CS$<>8__locals1.c.ListHobbies(true);
					Hobby hobby = (list.Count > 0) ? list[0] : null;
					if (EClass.rnd(20) == 0 || EClass.debug.showFav)
					{
						if (EClass.rnd(2) == 0 || hobby == null)
						{
							GameLang.refDrama1 = CS$<>8__locals1.c.GetFavCat().GetName().ToLower();
							GameLang.refDrama2 = CS$<>8__locals1.c.GetFavFood().GetName();
							CS$<>8__locals1.rumor = CS$<>8__locals1.<>4__this.GetText(CS$<>8__locals1.c, "general", "talk_fav");
							CS$<>8__locals1.c.knowFav = true;
						}
						else
						{
							GameLang.refDrama1 = hobby.Name.ToLower();
							CS$<>8__locals1.rumor = CS$<>8__locals1.<>4__this.GetText(CS$<>8__locals1.c, "general", "talk_hobby");
						}
					}
					else
					{
						CS$<>8__locals1.rumor = CS$<>8__locals1.<>4__this.GetRumor(CS$<>8__locals1.c);
					}
					CS$<>8__locals1.c.affinity.OnTalkRumor();
					choice.forceHighlight = true;
				}).SetCondition(() => CS$<>8__locals1.c.interest > 0);
			}
		}
		bool flag4 = false;
		if (!CS$<>8__locals1.c.IsPCFaction && CS$<>8__locals1.c.affinity.CanInvite() && !EClass._zone.IsInstance && CS$<>8__locals1.c.c_bossType == BossType.none)
		{
			if ((CS$<>8__locals1.c.trait.IsUnique || CS$<>8__locals1.c.IsGlobal) && CS$<>8__locals1.c.GetInt(111, null) == 0 && !CS$<>8__locals1.c.IsPCFaction)
			{
				this.Choice2("daBout", "_bout");
				flag4 = true;
			}
			else
			{
				this.Choice2("daInvite", "_invite");
			}
		}
		Thing t;
		foreach (Quest quest in EClass.game.quests.list)
		{
			Quest _quest = quest;
			if (quest.CanDeliverToClient(CS$<>8__locals1.c))
			{
				QuestDeliver questDeliver = _quest as QuestDeliver;
				using (List<Thing>.Enumerator enumerator2 = questDeliver.ListDestThing().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						t = enumerator2.Current;
						Thing _t = t;
						this.Choice2("daDeliver".lang(quest.GetTitle() ?? "", _t.GetName(NameStyle.Full, questDeliver.num), null, null, null), "_deliver").SetOnClick(delegate
						{
							CS$<>8__locals1.<>4__this.destThing = _t;
							CS$<>8__locals1.<>4__this.destQuest = _quest;
						}).SetOnTooltip(delegate(UITooltip a)
						{
							_t.WriteNote(a.note, null, IInspect.NoteMode.Default, null);
						});
					}
				}
			}
		}
		if (CS$<>8__locals1.c.IsPCParty)
		{
			if (!CS$<>8__locals1.c.isSummon)
			{
				if (EClass._zone.IsPCFaction && CS$<>8__locals1.c.homeBranch != EClass._zone.branch)
				{
					this.Choice2("daMakeHome", "_makeHome");
				}
				if (CS$<>8__locals1.c.host == null)
				{
					this.Choice2("daLeaveParty", "_leaveParty");
				}
			}
		}
		else if (CS$<>8__locals1.c.memberType != FactionMemberType.Livestock && !CS$<>8__locals1.c.IsGuest())
		{
			if (CS$<>8__locals1.c.trait.CanGuide)
			{
				foreach (Quest quest2 in EClass.game.quests.list)
				{
					if (quest2.IsRandomQuest)
					{
						Chara dest = (quest2.chara != null && quest2.chara.IsAliveInCurrentZone) ? quest2.chara : null;
						if (dest != null)
						{
							this.Choice2("daGoto".lang(dest.Name, quest2.GetTitle() ?? "", null, null, null), "_goto").SetOnClick(delegate
							{
								CS$<>8__locals1.<>4__this.destCard = dest;
							});
						}
						QuestDeliver questDeliver2 = quest2 as QuestDeliver;
						if (questDeliver2 != null && questDeliver2.IsDeliver && questDeliver2.DestZone == EClass._zone && EClass._zone.dictCitizen.ContainsKey(questDeliver2.uidTarget))
						{
							Chara dest2 = EClass._zone.FindChara(questDeliver2.uidTarget);
							if (dest2 != null)
							{
								this.Choice2("daGoto".lang(dest2.Name, quest2.GetTitle() ?? "", null, null, null), "_goto").SetOnClick(delegate
								{
									CS$<>8__locals1.<>4__this.destCard = dest2;
								});
							}
						}
					}
				}
				if (this.GetListGuide().Count > 0)
				{
					this.Choice2("daGuide", "_Guide");
				}
			}
			string s = "daQuest";
			Quest quest3 = CS$<>8__locals1.c.quest;
			this.Choice2(s.lang(((quest3 != null) ? quest3.GetTitle() : null) ?? "", null, null, null, null), "_quest").SetCondition(() => CS$<>8__locals1.c.quest != null);
			if (CS$<>8__locals1.c.trait is TraitGuard)
			{
				EClass.pc.things.Foreach(delegate(Thing _t)
				{
					if (_t.isLostProperty)
					{
						CS$<>8__locals1.<>4__this.Choice2("daLostProperty".lang(_t.Name, null, null, null, null), "_lostProperty").SetOnClick(delegate
						{
							CS$<>8__locals1.<>4__this.destThing = _t;
						});
					}
				}, true);
			}
			if (CS$<>8__locals1.c.trait is TraitGM_Mage && Guild.Mage.relation.rank >= 4)
			{
				this.Choice2("daChangeDomain", "_changeDomain").DisableSound();
			}
			if (CS$<>8__locals1.c.trait.ShopType != ShopType.None)
			{
				this.Choice2(CS$<>8__locals1.c.trait.TextNextRestock, "_buy").DisableSound();
			}
			if (CS$<>8__locals1.c.trait.SlaverType != SlaverType.None)
			{
				this.Choice2(CS$<>8__locals1.c.trait.TextNextRestockPet, "_buySlave").DisableSound();
			}
			if (CS$<>8__locals1.c.trait.CopyShop != Trait.CopyShopType.None)
			{
				this.Choice2(("daCopy" + CS$<>8__locals1.c.trait.CopyShop.ToString()).lang(CS$<>8__locals1.c.trait.NumCopyItem.ToString() ?? "", null, null, null, null), "_copyItem").DisableSound();
			}
			if (CS$<>8__locals1.c.trait.HaveNews && CS$<>8__locals1.c.GetInt(33, null) + 10080 < EClass.world.date.GetRaw(0))
			{
				this.Choice2("daNews", "_news");
			}
			if (!flag4 && !EClass._zone.IsInstance && !CS$<>8__locals1.c.IsPCFaction && CS$<>8__locals1.c.trait.CanBout && CS$<>8__locals1.c.IsGlobal && CS$<>8__locals1.c.GetInt(59, null) + 10080 < EClass.world.date.GetRaw(0))
			{
				this.Choice2("daBout", "_bout");
			}
			if (CS$<>8__locals1.c.isDrunk)
			{
				this.Choice2(flag2 ? "daBird" : "daTail", "_tail");
			}
			if (CS$<>8__locals1.c.trait.CanRevive)
			{
				this.Choice2("daRevive", "_revive").DisableSound();
			}
			if (!CS$<>8__locals1.c.trait.IDTrainer.IsEmpty() && !EClass._zone.IsUserZone && (Guild.GetCurrentGuild() == null || Guild.GetCurrentGuild().relation.IsMember()))
			{
				this.Choice2("daTrain", "_train").DisableSound();
			}
			if (CS$<>8__locals1.c.trait.CanWhore)
			{
				this.Choice2(flag2 ? "daBirdBuy" : "daTailBuy", "_whore");
			}
			if (CS$<>8__locals1.c.trait.CanHeal)
			{
				this.Choice2("daHeal", "_heal");
			}
			if (CS$<>8__locals1.c.trait.CanServeFood)
			{
				this.Choice2("daFood", "_food");
			}
			if (CS$<>8__locals1.c.trait is TraitInformer)
			{
				this.Choice2("daSellFame", "_sellFame");
			}
			if (CS$<>8__locals1.c.trait.CanInvestTown && Guild.GetCurrentGuild() == null)
			{
				this.Choice2("daInvest", "_investZone");
			}
			if (CS$<>8__locals1.c.trait.CanInvest)
			{
				this.Choice2("daInvest", "_investShop");
			}
			if (CS$<>8__locals1.c.trait.CanIdentify)
			{
				this.Choice2("daIdentify", "_identify").DisableSound();
				this.Choice2("daIdentifyAll", "_identifyAll");
				this.Choice2("daIdentifySP", "_identifySP").DisableSound();
			}
			if (CS$<>8__locals1.c.trait.CanPicklock)
			{
				if (CS$<>8__locals1.c.Evalue(280) < 20)
				{
					CS$<>8__locals1.c.elements.SetBase(280, 20, 0);
				}
				foreach (Thing t2 in EClass.pc.things.List((Thing a) => a.c_lockLv > 0, false))
				{
					Thing _t = t2;
					this.Choice2("daPicklock".lang(_t.Name, null, null, null, null), "_picklock").SetOnClick(delegate
					{
						CS$<>8__locals1.<>4__this.destThing = _t;
					});
				}
			}
			if (CS$<>8__locals1.c.trait is TraitBanker)
			{
				this.Choice2("daDeposit", "_deposit");
			}
			if (CS$<>8__locals1.c.IsMaid || (CS$<>8__locals1.c.trait.CanInvestTown && (EClass._zone.source.faction == "mysilia" || EClass._zone.IsPCFaction)))
			{
				this.Choice2("daExtraTax", "_extraTax");
			}
			if (CS$<>8__locals1.c.IsMaid)
			{
				if (EClass.Branch.meetings.CanStartMeeting)
				{
					this.Choice2("daMeeting".lang(EClass.Branch.meetings.list.Count.ToString() ?? "", null, null, null, null), "_meeting");
				}
				this.Choice2("daBuyLand", "_buyLand");
				this.Choice2("daChangeTitle", "_changeTitle");
			}
			if ((CS$<>8__locals1.c.trait is TraitMiko_Mifu || CS$<>8__locals1.c.trait is TraitMiko_Nefu || CS$<>8__locals1.c.trait is TraitEureka) && EClass.world.date.IsExpired(CS$<>8__locals1.c.c_dateStockExpire))
			{
				this.Choice2("daBlessing", "_blessing");
			}
		}
		if (CS$<>8__locals1.c.IsHomeMember())
		{
			if (CS$<>8__locals1.c.noMove)
			{
				this.Choice2("enableMove", "_enableMove");
			}
			if (!CS$<>8__locals1.c.IsPCParty && CS$<>8__locals1.c.memberType != FactionMemberType.Livestock && CS$<>8__locals1.c.trait.CanJoinParty)
			{
				this.Choice2("daJoinParty", "_joinParty");
			}
			this.Choice2("daFactionOther", "_factionOther");
		}
		if (CS$<>8__locals1.c.trait is TraitLoytel && EClass.game.quests.Get<QuestDebt>() != null)
		{
			this.Choice2("daGreatDebt", "_greatDebt");
		}
		if (!flag)
		{
			this.Choice2("bye", "_bye");
			this.EnableCancel(null);
		}
		this.Step("_factionOther");
		CS$<>8__locals1.<Build>g__Talk|0("what", this.StepDefault);
		if (CS$<>8__locals1.c.trait is TraitLoytel)
		{
			QuestDebt questDebt = EClass.game.quests.Get<QuestDebt>();
			if (questDebt != null && questDebt.gaveBill)
			{
				this.Choice("daGreatDebt2", "_greatDebt2", false);
			}
		}
		if (CS$<>8__locals1.c.IsPCParty)
		{
			if (!CS$<>8__locals1.c.isSummon)
			{
				this.Choice((CS$<>8__locals1.c.GetInt(106, null) == 0) ? "daShutup" : "daShutup2", "_shutup", false);
				if (CS$<>8__locals1.c.CanInsult())
				{
					this.Choice((CS$<>8__locals1.c.GetInt(108, null) == 0) ? "daInsult" : "daInsult2", "_insult", false);
				}
			}
		}
		else if (!CS$<>8__locals1.c.noMove)
		{
			this.Choice("disableMove", "_disableMove", false);
		}
		if (CS$<>8__locals1.c.GetInt(113, null) == 0)
		{
			this.Choice("daEquipSharedOff", "_toggleSharedEquip", false);
		}
		else
		{
			this.Choice("daEquipSharedOn", "_toggleSharedEquip", false);
		}
		if (!CS$<>8__locals1.c.IsMaid && CS$<>8__locals1.c.homeBranch == EClass.Branch)
		{
			this.Choice("daMakeMaid", "_daMakeMaid", false);
		}
		if (CS$<>8__locals1.c.trait.CanBeBanished && !CS$<>8__locals1.c.IsPCParty)
		{
			this.Choice("daBanish", "_depart", false);
		}
		this.Choice("daNothing", this.StepDefault, true);
		this.Step("_toggleSharedEquip");
		this.Method(delegate
		{
			CS$<>8__locals1.c.SetInt(113, (CS$<>8__locals1.c.GetInt(113, null) == 0) ? 1 : 0);
		}, null, null);
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, (CS$<>8__locals1.c.GetInt(113, null) == 0) ? "shutup" : "shutup2"), null);
		this.End();
		this.Step("_daMakeMaid");
		this.Method(delegate
		{
			EClass.Branch.uidMaid = CS$<>8__locals1.c.uid;
		}, null, null);
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, "becomeMaid"), null);
		this.End();
		this.Step("_joinParty");
		this.Method(delegate
		{
			if (!CS$<>8__locals1.c.trait.CanJoinPartyResident)
			{
				GameLang.refDrama1 = (CS$<>8__locals1.c.GetBestAttribute().ToString() ?? "");
				base.<Build>g__TempTalkTopic|4("invite3", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			EClass.pc.party.AddMemeber(CS$<>8__locals1.c);
		}, null, null);
		CS$<>8__locals1.<Build>g__Talk|0("hired", this.StepEnd);
		this.Step("_leaveParty");
		this.Method(delegate
		{
			EClass.pc.party.RemoveMember(CS$<>8__locals1.c);
			if (EClass.game.activeZone != CS$<>8__locals1.c.homeZone && CS$<>8__locals1.c.homeZone != null)
			{
				EClass.pc.Say("tame_send", CS$<>8__locals1.c, CS$<>8__locals1.c.homeZone.Name, null);
				CS$<>8__locals1.c.MoveZone(CS$<>8__locals1.c.homeZone, ZoneTransition.EnterState.Auto);
			}
		}, null, null);
		this.Goto("_bye");
		this.Step("_banish");
		this.Goto("_bye");
		this.Step("_makeLivestock");
		this.Method(delegate
		{
			CS$<>8__locals1.c.memberType = FactionMemberType.Livestock;
		}, null, null);
		CS$<>8__locals1.<Build>g__Talk|0("becomeLivestock", this.StepEnd);
		this.Step("_makeResident");
		this.Method(delegate
		{
			CS$<>8__locals1.c.memberType = FactionMemberType.Default;
		}, null, null);
		CS$<>8__locals1.<Build>g__Talk|0("becomeResident", this.StepEnd);
		this.Step("_depart");
		CS$<>8__locals1.<Build>g__Talk|0("depart_choice", this.StepDefault);
		this.Choice("depart1", "_depart1", false);
		this.Choice("depart2", "_depart2", false);
		this.Step("_depart1");
		this.Method(delegate
		{
			Layer instance = LayerDrama.Instance;
			Action onKill;
			if ((onKill = CS$<>8__locals1.<>9__100) == null)
			{
				onKill = (CS$<>8__locals1.<>9__100 = delegate()
				{
					EClass.Branch.BanishMember(CS$<>8__locals1.c, false);
				});
			}
			instance.SetOnKill(onKill);
		}, null, null);
		CS$<>8__locals1.<Build>g__Talk|0("depart1", this.StepEnd);
		this.Step("_depart2");
		CS$<>8__locals1.<Build>g__Talk|0("depart2", this.StepDefault);
		this.Step("_gift");
		CS$<>8__locals1.<Build>g__Talk|0("gift_good", null);
		this.End();
		this.Step("_goto");
		this.Method(delegate
		{
			GameLang.refDrama1 = CS$<>8__locals1.<>4__this.destCard.Name;
			if (CS$<>8__locals1.<>4__this.destCard == CS$<>8__locals1.c)
			{
				base.<Build>g__TempTalkTopic|4("goto2", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			base.<Build>g__TempTalkTopic|4("goto", "_goto2");
		}, null, null);
		this.Step("_goto2");
		this.Method(delegate
		{
			if (CS$<>8__locals1.<>4__this.destCard.isChara && !PathManager.Instance.RequestPathImmediate(EClass.pc.pos, CS$<>8__locals1.<>4__this.destCard.pos, EClass.pc, PathManager.MoveType.Default, -1, 0).HasPath)
			{
				CS$<>8__locals1.<>4__this.destCard.Teleport(EClass.pc.pos.GetNearestPoint(false, false, true, true) ?? EClass.pc.pos, true, true);
			}
			EClass.pc.SetAIImmediate(new AI_Goto(CS$<>8__locals1.<>4__this.destCard, 1, false, false));
			EInput.Consume(false, 20);
		}, null, null);
		this.End();
		this.Step("_rumor");
		CS$<>8__locals1.<Build>g__Talk|0("rumor", this.StepDefault);
		this.Step("_lostProperty");
		this.Method(delegate
		{
			GameLang.refDrama1 = CS$<>8__locals1.<>4__this.destThing.Name;
			CS$<>8__locals1.<>4__this.destThing.Destroy();
			EClass.player.ModKarma(5);
		}, null, null);
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, "deliver_purse"), this.StepEnd);
		Quest quest4 = CS$<>8__locals1.c.quest;
		string text = (!this.setup.forceJump.IsEmpty()) ? this.StepEnd : this.StepDefault;
		this.Step("_deliver");
		this.Method(delegate
		{
			GameLang.refDrama1 = CS$<>8__locals1.<>4__this.destQuest.NameDeliver;
			CS$<>8__locals1.<>4__this.destQuest.Deliver(CS$<>8__locals1.c, CS$<>8__locals1.<>4__this.destThing);
		}, null, null);
		this._Talk("tg", delegate()
		{
			if (CS$<>8__locals1.<>4__this.destQuest != null)
			{
				return CS$<>8__locals1.<>4__this.destQuest.GetTalkComplete().IsEmpty(CS$<>8__locals1.<>4__this.GetTopic(CS$<>8__locals1.c, (CS$<>8__locals1.<>4__this.destQuest.bonusMoney > 0) ? "questCompleteDeliverExtra" : "questCompleteDeliver"));
			}
			return "";
		}, this.StepEnd);
		this.Step("_quest");
		this._Talk("tg", delegate()
		{
			if (CS$<>8__locals1.c.quest == null)
			{
				return "";
			}
			GameLang.refDrama1 = CS$<>8__locals1.c.quest.RefDrama1;
			GameLang.refDrama2 = CS$<>8__locals1.c.quest.RefDrama2;
			GameLang.refDrama3 = CS$<>8__locals1.c.quest.RefDrama3;
			if (!base.<Build>g__taken|20())
			{
				return CS$<>8__locals1.c.quest.GetDetail(false);
			}
			return CS$<>8__locals1.c.quest.GetTalkProgress().IsEmpty(CS$<>8__locals1.<>4__this.GetTopic(CS$<>8__locals1.c, "questInProgress"));
		}, text);
		string text2 = "daAccept".lang();
		if (CS$<>8__locals1.c.quest != null && CS$<>8__locals1.c.quest.deadline != 0)
		{
			text2 += "hintDeadline".lang(CS$<>8__locals1.c.quest.TextDeadline, null, null, null, null).ToLower();
		}
		this.Choice(text2, (CS$<>8__locals1.c.quest != null && CS$<>8__locals1.c.quest.UseInstanceZone) ? "_questAccept_instance" : "_questAccept", false).SetOnClick(delegate
		{
			EClass.game.quests.Start(CS$<>8__locals1.c.quest);
		}).SetCondition(() => !base.<Build>g__taken|20() && EClass.game.quests.CountRandomQuest() < 5);
		this.Choice(text2, "_questFull", false).SetOnClick(delegate
		{
		}).SetCondition(() => !base.<Build>g__taken|20() && EClass.game.quests.CountRandomQuest() >= 5);
		this.Choice("daDecline", text, false).SetOnClick(new Action(CS$<>8__locals1.<Build>g__RumorChill|5)).SetCondition(() => !base.<Build>g__taken|20());
		if (CS$<>8__locals1.c.quest != null && EClass.game.quests.Get(CS$<>8__locals1.c.quest.uid) == null)
		{
			QuestSupply supply = CS$<>8__locals1.c.quest as QuestSupply;
			if (supply != null)
			{
				foreach (Thing t3 in supply.ListDestThing())
				{
					Thing _t = t3;
					this.Choice("daDeliver".lang(supply.GetTitle() ?? "", _t.GetName(NameStyle.Full, supply.num), null, null, null), "_deliver", false).SetOnClick(delegate
					{
						EClass.game.quests.Start(CS$<>8__locals1.c.quest);
						CS$<>8__locals1.<>4__this.destThing = _t;
						CS$<>8__locals1.<>4__this.destQuest = supply;
					}).SetOnTooltip(delegate(UITooltip a)
					{
						_t.WriteNote(a.note, null, IInspect.NoteMode.Default, null);
					});
				}
			}
		}
		this.EnableCancel(text);
		this.Step("_questAccept");
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, "questAccept"), this.StepEnd);
		this.Step("_questAccept_instance");
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, "questAccept"), null);
		this.Method(delegate
		{
			Zone z = CS$<>8__locals1.c.quest.CreateInstanceZone(CS$<>8__locals1.c);
			EClass.pc.MoveZone(z, ZoneTransition.EnterState.Center);
		}, null, this.StepEnd);
		this.Step("_questFull");
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, "questFull"), text);
		this.Step("_greatDebt");
		this.Method(delegate
		{
			QuestDebt questDebt2 = EClass.game.quests.Get<QuestDebt>();
			if (!questDebt2.CanGiveBill())
			{
				base.<Build>g__TempTalkTopic|4("loytel_bill_give_wait", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			if (questDebt2.gaveBill)
			{
				base.<Build>g__TempTalkTopic|4("loytel_bill_give_given", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			base.<Build>g__TempTalkTopic|4(questDebt2.GetIdTalk_GiveBill(), CS$<>8__locals1.<>4__this.StepEnd);
			questDebt2.GiveBill();
		}, null, null);
		this.Step("_greatDebt2");
		this.Method(delegate
		{
			QuestDebt questDebt2 = EClass.game.quests.Get<QuestDebt>();
			base.<Build>g__TempTalkTopic|4("loytel_bill_give_lost", CS$<>8__locals1.<>4__this.StepEnd);
			questDebt2.GiveBill();
		}, null, null);
		this.Step("_shutup");
		this.Method(delegate
		{
			CS$<>8__locals1.c.SetInt(106, (CS$<>8__locals1.c.GetInt(106, null) == 0) ? 1 : 0);
		}, null, null);
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, (CS$<>8__locals1.c.GetInt(106, null) == 0) ? "shutup" : "shutup2"), null);
		this.End();
		this.Step("_insult");
		this.Method(delegate
		{
			CS$<>8__locals1.c.SetInt(108, (CS$<>8__locals1.c.GetInt(108, null) == 0) ? 1 : 0);
		}, null, null);
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, (CS$<>8__locals1.c.GetInt(108, null) == 0) ? "insult" : "insult2"), null);
		this.Method(delegate
		{
			if (CS$<>8__locals1.c.GetInt(108, null) == 1)
			{
				CS$<>8__locals1.c.Talk("insult", null, null, false);
			}
		}, null, null);
		this.End();
		this.Step("_makeHome");
		this.Method(delegate
		{
			EClass._zone.branch.AddMemeber(CS$<>8__locals1.c);
		}, null, null);
		this._Talk("tg", this.GetTopic(CS$<>8__locals1.c, "ok"), null);
		this.End();
		this.Step("_hire");
		CS$<>8__locals1.<Build>g__Talk|0("rumor", this.StepDefault);
		this.Choice("daAccept", this.StepDefault, false).SetOnClick(delegate
		{
		});
		this.Choice("daDecline", this.StepDefault, false).SetOnClick(delegate
		{
		});
		this.Step("_invite");
		this.Method(delegate
		{
			if (!CS$<>8__locals1.c.trait.CanInvite)
			{
				base.<Build>g__TempTalkTopic|4("invite2", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			if (CS$<>8__locals1.c.GetBestAttribute() > EClass.pc.CHA && !EClass.debug.godMode)
			{
				GameLang.refDrama1 = (CS$<>8__locals1.c.GetBestAttribute().ToString() ?? "");
				base.<Build>g__TempTalkTopic|4("invite3", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			base.<Build>g__TempTalkTopic|4("invite", null);
			DramaCustomSequence <>4__this = CS$<>8__locals1.<>4__this;
			string lang = "yes";
			Action onJump;
			if ((onJump = CS$<>8__locals1.<>9__103) == null)
			{
				onJump = (CS$<>8__locals1.<>9__103 = delegate()
				{
					base.<Build>g__TempTalk|3("hired", CS$<>8__locals1.<>4__this.StepEnd);
					EClass.Sound.Play("good");
					CS$<>8__locals1.c.MakeAlly(true);
				});
			}
			<>4__this.Choice(lang, onJump);
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_Guide");
		this.Method(delegate
		{
			base.<Build>g__TempTalkTopic|4("guide", null);
			using (List<Card>.Enumerator enumerator3 = CS$<>8__locals1.<>4__this.GetListGuide().GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Card guide = enumerator3.Current;
					CS$<>8__locals1.<>4__this.Choice("daGotoGuide".lang(guide.Name, "", null, null, null), "_goto", false).SetOnClick(delegate
					{
						CS$<>8__locals1.<>4__this.destCard = guide;
					});
				}
			}
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
			base.<Build>g__TempCancel|88();
		}, null, null);
		CS$<>8__locals1.<Build>g__BackChill|89();
		this.Step("_tail");
		this.Method(delegate
		{
			base.<Build>g__TempTalkTopic|4(CS$<>8__locals1.bird + "1", null);
			DramaCustomSequence <>4__this = CS$<>8__locals1.<>4__this;
			string lang = "yes2";
			Action onJump;
			if ((onJump = CS$<>8__locals1.<>9__105) == null)
			{
				onJump = (CS$<>8__locals1.<>9__105 = delegate()
				{
					base.<Build>g__TempTalkTopic|4(CS$<>8__locals1.bird + "2", CS$<>8__locals1.<>4__this.StepEnd);
					EClass.pc.SetAI(new AI_Fuck
					{
						target = CS$<>8__locals1.c,
						sell = true
					});
				});
			}
			<>4__this.Choice(lang, onJump);
			CS$<>8__locals1.<>4__this.Choice("no2", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_whore");
		this.Method(delegate
		{
			int costWhore = CalcMoney.Whore(CS$<>8__locals1.c);
			GameLang.refDrama1 = (costWhore.ToString() ?? "");
			base.<Build>g__TempTalkTopic|4(CS$<>8__locals1.bird + "3", null);
			CS$<>8__locals1.<>4__this.Choice("yes2", delegate()
			{
				if (EClass.pc.GetCurrency("money") < costWhore)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("nomoney", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				SE.Pay();
				EClass.pc.ModCurrency(-costWhore, "money");
				CS$<>8__locals1.<Build>g__TempTalkTopic|4(CS$<>8__locals1.bird + "2", CS$<>8__locals1.<>4__this.StepEnd);
				EClass.pc.SetAI(new AI_Fuck
				{
					target = CS$<>8__locals1.c
				});
			});
			CS$<>8__locals1.<>4__this.Choice("no2", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_picklock");
		this.Method(delegate
		{
			int cost = CalcMoney.Picklock(EClass.pc, CS$<>8__locals1.<>4__this.destThing);
			GameLang.refDrama1 = (cost.ToString() ?? "");
			base.<Build>g__TempTalkTopic|4("bird3", null);
			CS$<>8__locals1.<>4__this.Choice("yes2", delegate()
			{
				if (CS$<>8__locals1.<>4__this.destThing.c_lockedHard)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("lockTooHard", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				if (EClass.pc.GetCurrency("money") < cost)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("nomoney", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				SE.Pay();
				EClass.pc.ModCurrency(-cost, "money");
				Layer layer = CS$<>8__locals1.<>4__this.manager.layer;
				Action onKill;
				if ((onKill = CS$<>8__locals1.<>9__108) == null)
				{
					onKill = (CS$<>8__locals1.<>9__108 = delegate()
					{
						CS$<>8__locals1.c.PlaySound("lock_open", 1f, true);
						CS$<>8__locals1.c.Say("lockpick_success", CS$<>8__locals1.c, CS$<>8__locals1.<>4__this.destThing, null, null);
						CS$<>8__locals1.c.ModExp(280, 200 + CS$<>8__locals1.<>4__this.destThing.c_lockLv * 20);
						CS$<>8__locals1.<>4__this.destThing.c_lockLv = 0;
						if (CS$<>8__locals1.<>4__this.destThing.isLostProperty)
						{
							EClass.player.ModKarma(-8);
						}
						CS$<>8__locals1.<>4__this.destThing.isLostProperty = false;
					});
				}
				layer.SetOnKill(onKill);
				CS$<>8__locals1.<Build>g__TempTalkTopic|4(CS$<>8__locals1.<>4__this.destThing.isLostProperty ? "lockpick_purse" : "bird2", CS$<>8__locals1.<>4__this.StepEnd);
			});
			CS$<>8__locals1.<>4__this.Choice("no2", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_buy");
		this.Method(delegate
		{
			if (CS$<>8__locals1.c.id == "miral")
			{
				SE.Play("click_chat");
				if (EClass.pc.GetCurrency("medal") > 0)
				{
					base.<Build>g__TempTalkTopic|4("miral_medal", null);
					return;
				}
				base.<Build>g__TempTalkTopic|4("miral_medal2", CS$<>8__locals1.<>4__this.StepDefault);
			}
		}, null, null);
		this.Method(delegate
		{
			if (EClass.player.IsCriminal && !EClass._zone.AllowCriminal && !EClass._zone.IsPCFaction && !CS$<>8__locals1.c.trait.AllowCriminal)
			{
				SE.Play("click_chat");
				base.<Build>g__TempTalkTopic|4("shop_criminal", CS$<>8__locals1.<>4__this.StepEnd);
				return;
			}
			CS$<>8__locals1.<>4__this.sequence.Exit();
			CS$<>8__locals1.<>4__this.manager.layer.Close();
			CS$<>8__locals1.c.trait.OnBarter();
			if (WidgetFeed.Instance)
			{
				WidgetFeed.Instance.Talk(CS$<>8__locals1.c, "barter");
			}
			CS$<>8__locals1.layer = EClass.ui.AddLayer(LayerInventory.CreateBuy(CS$<>8__locals1.c, CS$<>8__locals1.c.trait.CurrencyType, CS$<>8__locals1.c.trait.PriceType));
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_buyPlan");
		this.Method(delegate
		{
			List<ResearchPlan> plans = new List<ResearchPlan>();
			foreach (SourceResearch.Row row in EClass.sources.researches.rows)
			{
				if (EClass.BranchOrHomeBranch.researches.IsListBarter(row.id))
				{
					plans.Add(ResearchPlan.Create(row.id));
				}
			}
			EClass.ui.AddLayer<LayerList>().ManualList(delegate(UIList list, LayerList l)
			{
				list.moldItem = Resources.Load<ItemGeneral>("UI/Element/Item/ItemGeneralBarter").transform;
				list.callbacks = new UIList.Callback<ResearchPlan, ItemGeneral>
				{
					onInstantiate = delegate(ResearchPlan a, ItemGeneral b)
					{
						b.button1.mainText.text = a.Name;
						UIItem uiitem = Util.Instantiate<UIItem>("UI/Element/Item/Extra/costBarter", b.layout);
						HomeResource.Cost c = new HomeResource.Cost(EClass.BranchOrHomeBranch.resources.money, a.source.money);
						uiitem.text1.SetText(c.cost.ToString() ?? "", (c.resource.value >= c.cost) ? FontColor.Good : FontColor.Bad);
						uiitem.image1.sprite = c.resource.Sprite;
						b.button1.SetTooltip(delegate(UITooltip t)
						{
							a.WriteNote(t.note);
						}, true);
						b.button1.onClick.AddListener(delegate()
						{
							if (c.resource.value < c.cost)
							{
								SE.Beep();
								return;
							}
							c.resource.Mod(-c.cost, true);
							plans.Remove(a);
							EClass.BranchOrHomeBranch.researches.AddPlan(a);
							SE.Pay();
							list.List(true);
						});
						b.RebuildLayout(true);
					},
					onList = delegate(UIList.SortMode m)
					{
						foreach (ResearchPlan o in plans)
						{
							list.Add(o);
						}
					}
				};
			}).SetSize(450f, -1f).windows[0].AttachCurrency().Build(new UICurrency.Options
			{
				branchMoney = true
			});
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_upgradeHearth");
		this.Method(delegate
		{
			int cost = EClass.Branch.GetUpgradeCost();
			GameLang.refDrama1 = Lang._currency(cost, "money");
			GameLang.refDrama2 = ((EClass.Branch.lv + 1).ToString() ?? "");
			GameLang.refDrama3 = "hearth_dialog".lang(EClass.Branch.GetHearthHint(EClass.Branch.lv + 1), null, null, null, null);
			base.<Build>g__TempTalkTopic|4("upgrade_heath1", null);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				if (EClass.pc.GetCurrency("money") < cost)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("nomoney", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				EClass.pc.ModCurrency(-cost, "money");
				SE.Pay();
				LayerDrama.Instance.SetOnKill(delegate
				{
					EClass.Branch.Upgrade();
				});
				CS$<>8__locals1.<Build>g__TempTalkTopic|4("upgrade_heath2", CS$<>8__locals1.<>4__this.StepEnd);
			});
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_sellFame");
		this.Method(delegate
		{
			int cost = EClass.player.fame / 5;
			GameLang.refDrama1 = (cost.ToString() ?? "");
			if (cost == 0)
			{
				base.<Build>g__TempTalkTopic|4("goto2", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			base.<Build>g__TempTalkTopic|4("sellFame1", null);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				EClass.pc.ModCurrency(cost, "money");
				SE.Pay();
				EClass.player.ModFame(-cost);
				CS$<>8__locals1.<Build>g__TempTalkTopic|4("sellFame2", CS$<>8__locals1.<>4__this.StepDefault);
			});
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_investZone");
		this.Method(delegate
		{
			int cost = CalcMoney.InvestZone(EClass.pc);
			GameLang.refDrama1 = (cost.ToString() ?? "");
			GameLang.refDrama2 = (EClass._zone.investment.ToString() ?? "");
			base.<Build>g__TempTalkTopic|4("invest1", null);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				base.<Build>g__Invest|117(false);
			});
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
			CS$<>8__locals1.<>4__this.Choice("quickInvest", delegate()
			{
				base.<Build>g__Invest|117(true);
			});
		}, null, null);
		this.Step("_investShop");
		this.Method(delegate
		{
			int cost = CalcMoney.InvestShop(EClass.pc, CS$<>8__locals1.c);
			GameLang.refDrama1 = (cost.ToString() ?? "");
			GameLang.refDrama2 = (CS$<>8__locals1.c.trait.ShopLv.ToString() ?? "");
			base.<Build>g__TempTalkTopic|4("invest_shop1", null);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				base.<Build>g__Invest|120(false);
			});
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
			CS$<>8__locals1.<>4__this.Choice("quickInvest", delegate()
			{
				base.<Build>g__Invest|120(true);
			});
		}, null, null);
		this.Step("_changeTitle");
		this.Method(delegate
		{
			EClass.player.title = WordGen.Get("title");
			GameLang.refDrama1 = EClass.player.title;
			base.<Build>g__TempTalk|3("changeTitle", CS$<>8__locals1.<>4__this.StepDefault);
		}, null, this.StepDefault);
		this.Step("_buyLand");
		this.Method(delegate
		{
			bool flag5 = EClass._map.bounds.CanExpand(1);
			int costLand = CalcGold.ExpandLand();
			GameLang.refDrama1 = "";
			GameLang.refDrama2 = (costLand.ToString() ?? "");
			if (!flag5)
			{
				base.<Build>g__TempTalkTopic|4("expand3", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			base.<Build>g__TempTalkTopic|4("expand1", CS$<>8__locals1.<>4__this.StepDefault);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				if (EClass.pc.GetCurrency("money2") < costLand)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("nomoney", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				SE.Pay();
				EClass.pc.ModCurrency(-costLand, "money2");
				CS$<>8__locals1.<Build>g__TempTalkTopic|4("expand2", CS$<>8__locals1.<>4__this.StepDefault);
				EClass._map.bounds.Expand(1);
				SE.Play("good");
				EClass._map.RefreshAllTiles();
				ScreenEffect.Play("Firework");
			});
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_meeting");
		this.Method(delegate
		{
			CS$<>8__locals1.<>4__this.sequence.Exit();
			CS$<>8__locals1.<>4__this.manager.layer.Close();
			EClass.Branch.meetings.Start();
		}, null, null);
		this.End();
		this.Step("_give");
		this.Method(delegate
		{
			CS$<>8__locals1.<>4__this.manager.Hide();
			CS$<>8__locals1.layer = LayerDragGrid.CreateGive(CS$<>8__locals1.c);
			CS$<>8__locals1.layer.SetOnKill(new Action(CS$<>8__locals1.<>4__this.manager.Show));
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_blessing");
		this.Method(delegate
		{
			bool flag5 = CS$<>8__locals1.c.trait is TraitMiko_Mifu;
			base.<Build>g__TempTalkTopic|4("blessing", CS$<>8__locals1.<>4__this.StepEnd);
			Layer instance = LayerDrama.Instance;
			Action onKill;
			if ((onKill = CS$<>8__locals1.<>9__124) == null)
			{
				onKill = (CS$<>8__locals1.<>9__124 = delegate()
				{
					foreach (Chara chara in EClass.pc.party.members)
					{
						if (CS$<>8__locals1.c.trait is TraitMiko_Mifu)
						{
							Condition condition = chara.AddCondition<ConHolyVeil>(100, false);
							if (condition != null)
							{
								condition.SetPerfume(3);
							}
						}
						else if (CS$<>8__locals1.c.trait is TraitMiko_Nefu)
						{
							Condition condition2 = chara.AddCondition<ConEuphoric>(100, false);
							if (condition2 != null)
							{
								condition2.SetPerfume(3);
							}
						}
						else
						{
							Condition condition3 = chara.AddCondition<ConNightVision>(100, false);
							if (condition3 != null)
							{
								condition3.SetPerfume(3);
							}
						}
						chara.Say("blessing", chara, null, null);
						chara.PlaySound("pray", 1f, true);
						chara.PlayEffect("holyveil", true, 0f, default(Vector3));
					}
					CS$<>8__locals1.c.isRestocking = true;
				});
			}
			instance.SetOnKill(onKill);
			CS$<>8__locals1.c.c_dateStockExpire = EClass.world.date.GetRaw(0) + (flag5 ? 180 : 180) * 1440;
		}, null, null);
		this.Step("_train");
		this.Method(delegate
		{
			LayerList layerList = EClass.ui.AddLayer<LayerList>();
			Action<UIList, LayerList> onInit;
			if ((onInit = CS$<>8__locals1.<>9__125) == null)
			{
				onInit = (CS$<>8__locals1.<>9__125 = delegate(UIList list, LayerList l)
				{
					list.moldItem = Resources.Load<ButtonElement>("UI/Element/Button/ButtonElementTrain").transform;
					BaseList list2 = list;
					UIList.Callback<Element, ButtonElement> callback = new UIList.Callback<Element, ButtonElement>();
					callback.onClick = delegate(Element a, ButtonElement b)
					{
						int num = EClass.pc.elements.HasBase(a.id) ? CalcPlat.Train(EClass.pc, a) : CalcPlat.Learn(EClass.pc, a);
						if (num == 0)
						{
							SE.Beep();
							return;
						}
						if (!EClass.pc.TryPay(num, "plat"))
						{
							return;
						}
						if (EClass.pc.elements.HasBase(a.id))
						{
							EClass.pc.elements.Train(a.id, 10);
						}
						else
						{
							EClass.pc.elements.Learn(a.id, 1);
						}
						list.Redraw();
						UIButton.TryShowTip(null, true, true);
					};
					callback.onRedraw = delegate(Element a, ButtonElement b, int i)
					{
						bool flag5 = EClass.pc.elements.HasBase(a.id);
						b.imagePotential.enabled = flag5;
						b.SetElement(EClass.pc.elements.GetElement(a.id) ?? a, EClass.pc.elements, ButtonElement.Mode.Skill);
						int plat = EClass.pc.GetCurrency("plat");
						int cost = EClass.pc.elements.HasBase(a.id) ? CalcPlat.Train(EClass.pc, a) : CalcPlat.Learn(EClass.pc, a);
						b.mainText.text = b.mainText.text + " " + (flag5 ? "" : ("notLearned".lang() + " "));
						b.subText2.text = ((cost == 0) ? "-" : (cost.ToString() ?? "")).TagColor(() => plat >= cost && cost != 0, null);
						b.RebuildLayout(false);
					};
					callback.onInstantiate = delegate(Element a, ButtonElement b)
					{
					};
					callback.onList = delegate(UIList.SortMode m)
					{
						IEnumerable<SourceElement.Row> rows = EClass.sources.elements.rows;
						Func<SourceElement.Row, bool> predicate;
						if ((predicate = CS$<>8__locals1.<>9__131) == null)
						{
							predicate = (CS$<>8__locals1.<>9__131 = delegate(SourceElement.Row a)
							{
								if (a.tag.Contains("unused"))
								{
									return false;
								}
								if (a.tag.Contains("guild"))
								{
									return (a.tag.Contains("fighter") && Guild.Fighter.IsCurrentZone) || (a.tag.Contains("mage") && Guild.Mage.IsCurrentZone) || (a.tag.Contains("thief") && Guild.Thief.IsCurrentZone) || (a.tag.Contains("merchant") && Guild.Merchant.IsCurrentZone);
								}
								return !CS$<>8__locals1.isInGuild && a.category == "skill" && a.categorySub == CS$<>8__locals1.c.trait.IDTrainer;
							});
						}
						foreach (SourceElement.Row row in rows.Where(predicate).ToList<SourceElement.Row>())
						{
							list.Add(Element.Create(row.id, 0));
						}
					};
					list2.callbacks = callback;
				});
			}
			CS$<>8__locals1.layer = layerList.ManualList(onInit).SetSize(450f, -1f).SetTitles("wTrain", null).SetOnKill(new Action(SE.PopDrama));
			CS$<>8__locals1.layer.windows[0].AttachCurrency().Build(new UICurrency.Options
			{
				plat = true
			});
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_changeDomain");
		this.Method(delegate
		{
			CS$<>8__locals1.layer = EClass.player.SelectDomain(new Action(SE.PopDrama));
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_revive");
		this.Method(delegate
		{
			CS$<>8__locals1.layer = EClass.ui.AddLayer(LayerPeople.Create<ListPeopleRevive>("h_revive", null));
			CS$<>8__locals1.layer.SetOnKill(new Action(SE.PopDrama));
			CS$<>8__locals1.layer.windows[0].AttachCurrency().Build(new UICurrency.Options
			{
				money = true
			});
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_buySlave");
		this.Method(delegate
		{
			LayerPeople.slaveToBuy = null;
			CS$<>8__locals1.layer = EClass.ui.AddLayer(LayerPeople.Create<ListPeopleBuySlave>("h_invBuy", CS$<>8__locals1.c));
			CS$<>8__locals1.layer.SetOnKill(new Action(SE.PopDrama));
			CS$<>8__locals1.layer.windows[0].AttachCurrency().Build(new UICurrency.Options
			{
				money = true
			});
			CS$<>8__locals1.<>4__this.manager.Load();
		}, () => !CS$<>8__locals1.layer, "_buySlaveConfirm");
		this.Step("_buySlaveConfirm");
		this.Method(delegate
		{
			Chara tc = LayerPeople.slaveToBuy;
			if (tc == null)
			{
				base.<Build>g__RumorChill|5();
				return;
			}
			int cost = CalcMoney.BuySlave(tc);
			GameLang.refDrama1 = (cost.ToString() ?? "");
			GameLang.refDrama2 = tc.Name;
			base.<Build>g__TempTalkTopic|4("slave_buy", null);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				if (!EClass.pc.TryPay(cost, "money"))
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("nomoney", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				GameLang.refDrama1 = tc.Name;
				EClass._zone.AddCard(tc, EClass.pc.pos.GetNearestPoint(false, true, true, false));
				tc.MakeAlly(true);
				CS$<>8__locals1.c.GetObj<SlaverData>(5).list.Remove(tc);
				CS$<>8__locals1.<Build>g__TempTalkTopic|4("slave_buy2", CS$<>8__locals1.<>4__this.StepEnd);
			}).DisableSound();
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Goto(this.StepDefault);
		this.Step("_trade");
		this.Method(delegate
		{
			CS$<>8__locals1.layer = EClass.ui.AddLayer(LayerInventory.CreateContainer(CS$<>8__locals1.c));
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_identify");
		this.Method(delegate
		{
			CS$<>8__locals1.<>4__this.manager.Hide();
			CS$<>8__locals1.c.trait.OnBarter();
			CS$<>8__locals1.layer = LayerDragGrid.CreateIdentify(EClass.pc, false, BlessedState.Normal, CalcMoney.Identify(EClass.pc, false), -1);
			CS$<>8__locals1.layer.SetOnKill(new Action(CS$<>8__locals1.<>4__this.manager.Show));
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_identifyAll");
		this.Method(delegate
		{
			int costIdentify = 0;
			int numIdentify = 0;
			int numSuperior = 0;
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (t.IsIdentified || t.c_IDTState == 1)
				{
					return;
				}
				int numIdentify = numIdentify;
				numIdentify++;
				costIdentify += CalcMoney.Identify(EClass.pc, false);
			}, true);
			GameLang.refDrama1 = (costIdentify.ToString() ?? "");
			GameLang.refDrama2 = (numIdentify.ToString() ?? "");
			if (numIdentify == 0)
			{
				base.<Build>g__TempTalkTopic|4("appraise3", CS$<>8__locals1.<>4__this.StepDefault);
				return;
			}
			base.<Build>g__TempTalkTopic|4("appraise1", CS$<>8__locals1.<>4__this.StepDefault);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				if (EClass.pc.GetCurrency("money") < costIdentify)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("nomoney", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				SE.Pay();
				EClass.pc.ModCurrency(-costIdentify, "money");
				foreach (Thing thing in EClass.pc.things.List((Thing t) => !t.IsIdentified, true))
				{
					thing.Thing.Identify(false, IDTSource.Identify);
					if (!thing.IsInstalled)
					{
						int numSuperior = numSuperior;
						numSuperior++;
					}
				}
				CS$<>8__locals1.<Build>g__TempTalkTopic|4("appraise2", CS$<>8__locals1.<>4__this.StepDefault);
			});
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_identifySP");
		this.Method(delegate
		{
			CS$<>8__locals1.<>4__this.manager.Hide();
			CS$<>8__locals1.c.trait.OnBarter();
			CS$<>8__locals1.layer = LayerDragGrid.CreateIdentify(EClass.pc, true, BlessedState.Normal, CalcMoney.Identify(EClass.pc, true), -1);
			CS$<>8__locals1.layer.SetOnKill(new Action(CS$<>8__locals1.<>4__this.manager.Show));
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.Step("_bout");
		this.Method(delegate
		{
			base.<Build>g__TempTalkTopic|4("bout1", CS$<>8__locals1.<>4__this.StepDefault);
			DramaCustomSequence <>4__this = CS$<>8__locals1.<>4__this;
			string lang = "yes";
			Action onJump;
			if ((onJump = CS$<>8__locals1.<>9__136) == null)
			{
				onJump = (CS$<>8__locals1.<>9__136 = delegate()
				{
					Zone z = SpatialGen.CreateInstance("field", new ZoneInstanceBout
					{
						uidTarget = CS$<>8__locals1.c.uid,
						targetX = CS$<>8__locals1.c.pos.x,
						targetZ = CS$<>8__locals1.c.pos.z
					});
					CS$<>8__locals1.c.SetGlobal();
					z.events.AddPreEnter(new ZonePreEnterBout
					{
						target = CS$<>8__locals1.c
					}, true);
					CS$<>8__locals1.c.SetInt(59, EClass.world.date.GetRaw(0));
					LayerDrama.Instance.SetOnKill(delegate
					{
						EClass.pc.MoveZone(z, ZoneTransition.EnterState.Center);
					});
					base.<Build>g__TempTalkTopic|4("bout2", CS$<>8__locals1.<>4__this.StepEnd);
				});
			}
			<>4__this.Choice(lang, onJump);
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, this.StepDefault);
		this.Step("_news");
		this.Method(delegate
		{
			Zone zone = EClass.world.region.CreateRandomSite(EClass._zone, 5, null, true, 0);
			if (zone == null)
			{
				base.<Build>g__TempTalkTopic|4("news2", null);
			}
			else
			{
				zone.isKnown = true;
				Msg.Say("discoverZone", zone.Name, null, null, null);
				GameLang.refDrama1 = zone.Name;
				base.<Build>g__TempTalkTopic|4("news1", null);
			}
			CS$<>8__locals1.c.SetInt(33, EClass.world.date.GetRaw(0));
		}, null, null);
		this.Method(delegate
		{
			CS$<>8__locals1.<>4__this.manager.Load();
		}, null, this.StepDefault);
		this.Step("_heal");
		this.Method(delegate
		{
			int costHeal = CalcMoney.Heal(EClass.pc);
			GameLang.refDrama1 = (costHeal.ToString() ?? "");
			base.<Build>g__TempTalkTopic|4("healer1", null);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				if (EClass.pc.GetCurrency("money") < costHeal)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("nomoney", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				SE.Pay();
				foreach (Chara cc in EClass.pc.party.members)
				{
					ActEffect.Proc(EffectId.HealComplete, cc, null, 100, default(ActRef));
				}
				EClass.pc.ModCurrency(-costHeal, "money");
				CS$<>8__locals1.<Build>g__TempTalkTopic|4("healer2", CS$<>8__locals1.<>4__this.StepEnd);
			});
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_food");
		this.Method(delegate
		{
			int cost = CalcMoney.Meal(EClass.pc);
			GameLang.refDrama1 = (cost.ToString() ?? "");
			base.<Build>g__TempTalkTopic|4("food1", null);
			CS$<>8__locals1.<>4__this.Choice("yes", delegate()
			{
				if (EClass.pc.hunger.GetPhase() <= 0)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("alreadyFull", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				if (EClass.pc.GetCurrency("money") < cost)
				{
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("nomoney", CS$<>8__locals1.<>4__this.StepDefault);
					return;
				}
				SE.Pay();
				EClass.pc.ModCurrency(-cost, "money");
				CS$<>8__locals1.<Build>g__TempTalkTopic|4("food2", CS$<>8__locals1.<>4__this.StepDefault);
				FoodEffect.Proc(EClass.pc, ThingGen.Create("dish_lunch", -1, -1));
				EClass.pc.hunger.value = 0;
			});
			CS$<>8__locals1.<>4__this.Choice("no", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		CS$<>8__locals1.bankTier = new int[]
		{
			100,
			1000,
			10000,
			100000,
			1000000
		};
		this.Step("_deposit");
		this.Method(delegate
		{
			CS$<>8__locals1.<>4__this.sequence.Exit();
			CS$<>8__locals1.<>4__this.manager.layer.Close();
			if (WidgetFeed.Instance)
			{
				WidgetFeed.Instance.Talk(CS$<>8__locals1.c, "barter");
			}
			SE.Play("shop_open");
			CS$<>8__locals1.layer = LayerInventory.CreateContainer(EClass.game.cards.container_deposit);
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.End();
		this.Step("_copyItem");
		this.Method(delegate
		{
			CS$<>8__locals1.<>4__this.sequence.Exit();
			CS$<>8__locals1.<>4__this.manager.layer.Close();
			if (WidgetFeed.Instance)
			{
				WidgetFeed.Instance.Talk(CS$<>8__locals1.c, "barter");
			}
			SE.Play("shop_open");
			CS$<>8__locals1.c.trait.OnBarter();
			if (CS$<>8__locals1.c.c_copyContainer == null)
			{
				CS$<>8__locals1.c.c_copyContainer = ThingGen.Create("container_deposit", -1, -1);
			}
			int numCopyItem = CS$<>8__locals1.c.trait.NumCopyItem;
			CS$<>8__locals1.c.c_copyContainer.things.SetSize(numCopyItem, 1);
			CS$<>8__locals1.layer = LayerInventory.CreateContainer<InvOwnerCopyShop>(CS$<>8__locals1.c, CS$<>8__locals1.c.c_copyContainer, CurrencyType.None);
		}, () => !CS$<>8__locals1.layer, this.StepDefault);
		this.End();
		CS$<>8__locals1.taxTier = new int[]
		{
			0,
			1000,
			2000,
			5000,
			10000,
			20000
		};
		this.Step("_extraTax");
		this.Method(delegate
		{
			GameLang.refDrama1 = Lang._currency(EClass.player.extraTax, "money");
			base.<Build>g__TempTalkTopic|4("extraTax", null);
			int[] taxTier = CS$<>8__locals1.taxTier;
			for (int i = 0; i < taxTier.Length; i++)
			{
				int i2 = taxTier[i];
				int _i = i2;
				CS$<>8__locals1.<>4__this.Choice(Lang._currency(_i, true, 14), delegate()
				{
					EClass.player.extraTax = _i;
					GameLang.refDrama1 = Lang._currency(_i, "money");
					CS$<>8__locals1.<Build>g__TempTalkTopic|4("extraTax2", CS$<>8__locals1.<>4__this.StepDefault);
				});
			}
			CS$<>8__locals1.<>4__this.Choice("no2", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_withdraw");
		this.Method(delegate
		{
			int bankMoney = EClass.player.bankMoney;
			GameLang.refDrama1 = (bankMoney.ToString() ?? "");
			base.<Build>g__TempTalkTopic|4("banker2", null);
			int[] bankTier = CS$<>8__locals1.bankTier;
			for (int i = 0; i < bankTier.Length; i++)
			{
				int i2 = bankTier[i];
				int _i = i2;
				if (EClass.player.bankMoney >= _i)
				{
					CS$<>8__locals1.<>4__this.Choice(Lang._currency(_i, true, 14), delegate()
					{
						SE.Pay();
						EClass.pc.ModCurrency(_i, "money");
						EClass.player.bankMoney -= _i;
						CS$<>8__locals1.<Build>g__TempTalkTopic|4("banker4", CS$<>8__locals1.<>4__this.StepDefault);
					});
				}
			}
			CS$<>8__locals1.<>4__this.Choice("no2", CS$<>8__locals1.<>4__this.StepDefault, true).SetOnClick(new Action(base.<Build>g__RumorChill|5));
		}, null, null);
		this.Step("_disableMove");
		this.Method(delegate
		{
			CS$<>8__locals1.c.noMove = true;
			CS$<>8__locals1.c.orgPos = new Point(CS$<>8__locals1.c.pos);
			CS$<>8__locals1.c.Talk("ok", null, null, false);
		}, null, null);
		this.End();
		this.Step("_enableMove");
		this.Method(delegate
		{
			CS$<>8__locals1.c.noMove = false;
			CS$<>8__locals1.c.orgPos = null;
			CS$<>8__locals1.c.Talk("thanks", null, null, false);
		}, null, null);
		this.End();
		this.Step("_bye");
		this.Method(delegate
		{
			CS$<>8__locals1.c.Talk("bye", null, null, false);
		}, null, null);
		this.End();
	}

	public string GetRumor(Chara c)
	{
		if (c.interest <= 0)
		{
			return this.GetText(c, "rumor", "bored");
		}
		if (this.HasTopic("unique", c.id))
		{
			this.manager.enableTone = false;
			return this.GetText(c, "unique", c.id);
		}
		if (EClass.rnd(2) == 0 && !c.trait.IDRumor.IsEmpty())
		{
			return this.GetText(c, "rumor", c.trait.IDRumor);
		}
		if (EClass.rnd(2) == 0 && this.HasTopic("zone", EClass._zone.id))
		{
			return this.GetText(c, "zone", EClass._zone.id);
		}
		if (EClass.rnd(2) == 0)
		{
			return this.GetText(c, "rumor", "interest_" + c.bio.idInterest.ToEnum<Interest>().ToString());
		}
		if (EClass.rnd(2) == 0)
		{
			return c.GetTalkText("rumor", false, true);
		}
		if (EClass.rnd(4) == 0)
		{
			return this.GetText(c, "rumor", "hint");
		}
		return this.GetText(c, "rumor", "default");
	}

	public bool HasTopic(string idSheet, string idTopic)
	{
		string path = CorePath.CorePackage.TextDialog + "dialog.xlsx";
		if (!File.Exists(path))
		{
			return false;
		}
		ExcelData excelData = new ExcelData();
		excelData.path = path;
		excelData.BuildMap(idSheet);
		return excelData.sheets[idSheet].map.ContainsKey(idTopic);
	}

	public string GetText(Chara c, string idSheet, string idTopic)
	{
		if (!idTopic.IsEmpty() && this.manager.customTalkTopics.ContainsKey(idTopic))
		{
			return this.manager.customTalkTopics[idTopic];
		}
		string[] dialog = Lang.GetDialog(idSheet, idTopic);
		if (!this.manager.enableTone && !(idSheet == "general"))
		{
			return dialog.RandomItem<string>();
		}
		return c.ApplyTone(dialog.RandomItem<string>(), false);
	}

	public string GetTopic(Chara c, string idTopic = null)
	{
		return this.GetText(c, "general", idTopic);
	}

	public void BuildTextData()
	{
		this.Step("import");
		foreach (string text in this.setup.textData.Split(new string[]
		{
			Environment.NewLine + Environment.NewLine
		}, StringSplitOptions.RemoveEmptyEntries))
		{
			if (!(text == Environment.NewLine))
			{
				this._Talk("", text.TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray()), null);
			}
		}
		this.End();
	}

	public DramaEvent Event(DramaEvent e)
	{
		this.events.Add(e);
		e.sequence = this.sequence;
		return e;
	}

	public void Step(string step)
	{
		this.Event(new DramaEvent
		{
			step = step
		});
	}

	public void Method(Action action, Func<bool> endFunc = null, string idJump = null)
	{
		this.Event(new DramaEventMethod(action, 0f, false)
		{
			endFunc = endFunc
		});
		if (!idJump.IsEmpty())
		{
			this.Event(new DramaEventGoto(idJump));
		}
	}

	public void End()
	{
		this.Event(new DramaEventGoto("end"));
	}

	public void Goto(string idJump)
	{
		this.Event(new DramaEventGoto(idJump));
	}

	public void GotoDefault()
	{
		this.Event(new DramaEventGoto(this.StepDefault));
	}

	public void _Talk(string idActor, string text, string idJump = null)
	{
		this.manager.lastTalk = (this.Event(new DramaEventTalk
		{
			idActor = idActor,
			idJump = idJump,
			text = text
		}) as DramaEventTalk);
	}

	public void _Talk(string idActor, Func<string> funcText, string idJump = null)
	{
		this.manager.lastTalk = (this.Event(new DramaEventTalk
		{
			idActor = idActor,
			idJump = idJump,
			funcText = funcText
		}) as DramaEventTalk);
	}

	public void _TempTalk(string idActor, string text, string idJump = null)
	{
		this.manager.lastTalk = (this.Event(new DramaEventTalk
		{
			idActor = idActor,
			idJump = idJump,
			text = text,
			temp = true
		}) as DramaEventTalk);
		this.sequence.tempEvents.Add(this.manager.lastTalk);
	}

	public void TempGoto(string idJump = null)
	{
		this.sequence.tempEvents.Clear();
		this.sequence.Play(idJump);
	}

	public DramaChoice Choice(string lang, string idJump, bool cancel = false)
	{
		DramaChoice dramaChoice = new DramaChoice(lang.lang(), idJump, "", "", "");
		this.manager.lastTalk.AddChoice(dramaChoice);
		if (cancel)
		{
			this.EnableCancel(idJump);
		}
		return dramaChoice;
	}

	public DramaChoice Choice(string lang, Action onJump)
	{
		DramaChoice dramaChoice = new DramaChoice(lang.lang(), null, "", "", "");
		dramaChoice.onJump = onJump;
		this.manager.lastTalk.AddChoice(dramaChoice);
		return dramaChoice;
	}

	public DramaChoice Choice2(string lang, string idJump)
	{
		DramaChoice dramaChoice = new DramaChoice(lang.lang(), idJump, "", "", "");
		this.manager.lastTalk.AddChoice(dramaChoice);
		this.sequence.manager._choices.Add(dramaChoice);
		return dramaChoice;
	}

	public void EnableCancel(string idCancelJump = null)
	{
		this.manager.lastTalk.canCancel = true;
		this.manager.lastTalk.idCancelJump = idCancelJump;
	}

	public List<Card> GetListGuide()
	{
		List<Card> list = new List<Card>();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait.GuidePriotiy > 0)
			{
				list.Add(thing);
			}
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCParty && chara.trait.GuidePriotiy > 0)
			{
				list.Add(chara);
			}
		}
		list.Sort((Card a, Card b) => b.trait.GuidePriotiy - a.trait.GuidePriotiy);
		return list;
	}

	public Card destCard;

	public Chara destChara;

	public Quest destQuest;

	public Thing destThing;

	public string idDefault;

	public string idCustom;

	public DramaSetup setup;

	public DramaSequence sequence;

	public List<DramaEvent> events;

	public DramaManager manager;
}
