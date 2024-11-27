using System;
using System.Collections.Generic;
using UnityEngine;

public class DramaOutcome : EMono
{
	public DramaSequence sequence
	{
		get
		{
			return this.manager.sequence;
		}
	}

	public Person tg
	{
		get
		{
			return this.manager.tg;
		}
	}

	public Chara cc
	{
		get
		{
			return this.tg.chara;
		}
	}

	public void StartNewGame()
	{
		if (!LayerTitle.actor)
		{
			return;
		}
		EMono.game.StartNewGame();
	}

	public void StartNewGame2()
	{
		if (!LayerTitle.actor)
		{
			return;
		}
		EMono.core.actionsNextFrame.Add(new Action(LayerTitle.KillActor));
		EMono.pc.MoveZone(EMono.game.Prologue.idStartZone);
		EMono.pc.global.transition = new ZoneTransition
		{
			state = ZoneTransition.EnterState.Exact,
			x = EMono.game.Prologue.startX,
			z = EMono.game.Prologue.startZ
		};
	}

	public void PutOutFire()
	{
		foreach (Card card in EMono._map.props.installed.traits.GetTraitSet<TraitHearth>().Values)
		{
			EMono._zone.AddCard(ThingGen.Create("dish_soup", -1, -1), card.pos.GetRandomNeighbor());
		}
	}

	public void OnClaimLand()
	{
		Chara c = EMono.game.cards.globalCharas.Find("ashland");
		EMono.game.quests.globalList.Add(Quest.Create("sharedContainer", null, null).SetClient(c, false));
		EMono.game.quests.globalList.Add(Quest.Create("crafter", null, null).SetClient(c, false));
		EMono.game.quests.globalList.Add(Quest.Create("defense", null, null).SetClient(c, false));
		EMono.game.quests.Get<QuestHome>().ChangePhase(2);
		this.AddMaid();
		if (WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance.transHighlightSwitch.SetActive(true);
			EMono.player.flags.toggleHotbarHighlightDisabled = false;
		}
	}

	public void AddMaid()
	{
	}

	public void QuestSharedContainer_Drop1()
	{
		EMono.player.DropReward(ThingGen.Create("chest6", -1, -1), false);
	}

	public void QuestShippingChest_Drop1()
	{
		Recipe.DropIngredients("container_shipping", "palm", 6);
	}

	public void QuestExploration_Drop1()
	{
		EMono.player.DropReward(ThingGen.CreateScroll(8220, 1), true).c_IDTState = 0;
		EMono.player.DropReward(ThingGen.CreateScroll(8221, 1), false).c_IDTState = 0;
	}

	public void QuestExploration_MeetFarris()
	{
		EMono.game.quests.Get<QuestExploration>().ChangePhase(1);
		this.cc.RemoveEditorTag(EditorTag.AINoMove);
		this.cc.RemoveEditorTag(EditorTag.InvulnerableToMobs);
		this.cc.RemoveEditorTag(EditorTag.Invulnerable);
		this.cc.homeZone = EMono.game.StartZone;
		this.cc.MoveZone(EMono.game.StartZone, ZoneTransition.EnterState.Return);
	}

	public void QuestExploration_MeetFarris2()
	{
		EMono.game.quests.Get<QuestExploration>().ChangePhase(2);
		if (EMono.Branch == null)
		{
			EMono._zone.ClaimZone(false);
		}
		EMono.Branch.Recruit(this.cc);
		if (EMono.game.quests.Main == null)
		{
			EMono.game.quests.Start("main", null, true);
		}
		EMono.game.quests.Main.ChangePhase(300);
	}

	public void QuestExploration_AfterCrystal()
	{
		Quest quest = EMono.game.quests.Get<QuestExploration>();
		if (quest == null)
		{
			quest = EMono.game.quests.Start("exploration", EMono.game.cards.globalCharas.Find("ashland"), false);
		}
		quest.ChangePhase(5);
		Chara chara = EMono.game.cards.globalCharas.Find("fiama");
		EMono._zone.AddCard(ThingGen.CreateScroll(8220, 1).Identify(false, IDTSource.Identify), chara.pos);
		chara.MoveZone(EMono.game.StartZone, ZoneTransition.EnterState.Auto);
		chara.RemoveEditorTag(EditorTag.AINoMove);
	}

	public void QuestExploration_AfterComplete()
	{
		Chara chara = EMono.game.cards.globalCharas.Find("ashland");
		chara.MoveHome("lothria", 40, 49);
		EMono.game.quests.RemoveAll(chara);
		chara = EMono.game.cards.globalCharas.Find("fiama");
		chara.MoveHome("lothria", 46, 56);
		EMono.game.quests.RemoveAll(chara);
		if (EMono.game.quests.Main == null)
		{
			EMono.game.quests.Start("main", null, true);
		}
		EMono.game.quests.Main.ChangePhase(700);
	}

	public void QuestCraft_Drop1()
	{
		EMono.player.DropReward(ThingGen.CreateRawMaterial(EMono.sources.materials.alias["straw"]), false);
	}

	public void QuestDefense_0()
	{
		Prologue prologue = EMono.game.Prologue;
		Card card = EMono._zone.AddChara("punk", prologue.posPunk.x, prologue.posPunk.y);
		card.things.DestroyAll(null);
		(EMono._zone.AddThing("gallows", prologue.posPunk.x, prologue.posPunk.y).Install().trait as TraitShackle).Restrain(card, false);
		CardBlueprint.SetNormalRarity(false);
	}

	public void QuestVernis_DropRecipe()
	{
		EMono.player.DropReward(ThingGen.CreateRecipe("explosive"), false);
	}

	public void QuestDefense_1()
	{
		Prologue prologue = EMono.game.Prologue;
		Card tc = EMono._zone.AddChara("boar", prologue.posPunk.x + 1, prologue.posPunk.y);
		(EMono._zone.AddThing("gallows", prologue.posPunk.x + 1, prologue.posPunk.y).Install().trait as TraitShackle).Restrain(tc, false);
		EMono.player.DropReward(ThingGen.Create("stone", -1, -1).SetNum(20), false);
		EMono.player.DropReward(ThingGen.Create("330", -1, -1).SetNum(3), true).Identify(false, IDTSource.Identify);
		EMono.player.DropReward(ThingGen.Create("331", -1, -1).SetNum(3), true).Identify(false, IDTSource.Identify);
		EMono.player.DropReward(ThingGen.Create("bandage", -1, -1).SetNum(5), false);
	}

	public void QuestDefense_2()
	{
	}

	public void QuestDebt_reward()
	{
		EMono.game.quests.Get<QuestDebt>().GiveReward();
	}

	public void Tutorial1()
	{
		Thing t = ThingGen.Create("log", -1, -1);
		Point point = new Point(53, 52);
		EMono._zone.AddCard(t, point).SetPlaceState(PlaceState.installed, false);
		t = ThingGen.Create("crimAle", -1, -1);
		EMono._zone.AddCard(t, point).SetPlaceState(PlaceState.installed, false);
	}

	public void WelcomeMsg()
	{
	}

	public void chara_hired()
	{
		if (EMono.Branch.IsRecruit(this.cc))
		{
			EMono.pc.ModCurrency(-CalcGold.Hire(this.cc), "money2");
			this.cc.SetBool(18, true);
		}
		EMono.Sound.Play("good");
		EMono.Branch.Recruit(this.cc);
	}

	public void chara_hired_ticket()
	{
		EMono.pc.things.Find("ticket_resident", -1, -1).ModNum(-1, true);
		EMono.Sound.Play("good");
		this.cc.SetBool(18, true);
		EMono.Branch.Recruit(this.cc);
	}

	public void nerun_gift()
	{
		Dialog.Gift("", true, new Card[]
		{
			ThingGen.Create("rp1", -1, -1)
		});
	}

	public void nerun_gift2()
	{
		Dialog.Gift("", true, new Card[]
		{
			ThingGen.Create("rp1", -1, -1)
		});
	}

	public void nerun_gift3()
	{
		Dialog.Gift("", true, new List<Card>
		{
			ThingGen.Create("rp1", -1, -1),
			ThingGen.Create("rp2", -1, -1),
			ThingGen.Create("rp3", -1, -1),
			ThingGen.Create("rp4", -1, -1),
			ThingGen.Create("rp5", -1, -1)
		});
	}

	public void fiama_gold()
	{
		EMono.player.DropReward(ThingGen.Create("money2", -1, -1).SetNum(10), false);
		if (EMono.game.idPrologue == 2)
		{
			EMono.player.DropReward(ThingGen.Create("hammer", -1, -1), false);
		}
	}

	public void fiama_pet1()
	{
		Chara c = CharaGen.Create("dog", -1);
		this.fiama_pet(c);
	}

	public void fiama_pet2()
	{
		Chara c = CharaGen.Create("cat", -1);
		this.fiama_pet(c);
	}

	public void fiama_pet3()
	{
		Chara c = CharaGen.Create("bearCub", -1);
		this.fiama_pet(c);
	}

	public void fiama_pet4()
	{
		Chara c = CharaGen.Create("shojo", -1);
		this.fiama_pet(c);
	}

	private void fiama_pet(Chara c)
	{
		EMono._zone.AddCard(c, EMono.pc.pos);
		c.MakeAlly(true);
		c.SetInt(100, 1);
	}

	public void fiama_starter_gift()
	{
		switch (DramaChoice.lastChoice.index)
		{
		case 0:
		{
			Thing thing = ThingGen.Create("ring_decorative", -1, -1).SetNoSell();
			thing.elements.SetBase(65, 10, 0);
			EMono.player.DropReward(thing, false);
			return;
		}
		case 1:
			EMono.player.DropReward(ThingGen.Create("ticket_resident", -1, -1).SetNoSell(), false);
			EMono.player.DropReward(ThingGen.Create("1174", -1, -1).SetNoSell(), false);
			return;
		case 2:
		{
			Thing thing = ThingGen.Create("boots_", -1, -1).SetNoSell();
			thing.elements.SetBase(65, 5, 0);
			thing.elements.SetBase(407, 5, 0);
			EMono.player.DropReward(thing, false);
			return;
		}
		case 3:
			EMono.player.DropReward(ThingGen.Create("1085", -1, -1).SetNum(3).SetNoSell(), false);
			return;
		case 4:
			for (int i = 0; i < 10; i++)
			{
				EMono.player.DropReward(ThingGen.Create("234", -1, -1).SetNoSell(), false);
			}
			return;
		default:
			this.cc.DoHostileAction(EMono.pc, true);
			this.cc.calmCheckTurn = 100;
			return;
		}
	}

	public void revive_pet()
	{
		foreach (Chara chara in EMono.pc.homeBranch.members)
		{
			if (chara.isDead && chara.GetInt(100, null) != 0)
			{
				chara.GetRevived();
			}
		}
		this.cc.ModAffinity(EMono.pc, -2, true);
	}

	public void melilith_friend()
	{
		if (this.cc.id == "melilith" && EMono.game.quests.completedIDs.Contains("melilith"))
		{
			this.cc.MakeAlly(true);
		}
	}

	public void sister_friend()
	{
		if (this.cc.id == "olderyoungersister")
		{
			this.cc.MakeAlly(true);
		}
		EMono.pc.ModCurrency(-10000, "money");
		this.cc.Say("hug", this.cc, EMono.pc, null, null);
	}

	public void sister_change()
	{
		this.cc.idSkin = ((this.cc.idSkin == 1) ? 2 : 1);
	}

	public void get_scratch()
	{
		EMono._map.TrySmoothPick(EMono.pc.pos, ThingGen.Create("scratchcard", -1, -1), EMono.pc);
		EMono.game.dateScratch = EMono.world.date.GetRaw(24);
	}

	public void poppy_found()
	{
		if (this.cc.id == "poppy")
		{
			this.cc.MakeAlly(true);
		}
		EMono.game.quests.Get("puppy").NextPhase();
		Msg.Say("npc_rescue", this.cc, null, null, null);
		this.cc.RemoveEditorTag(EditorTag.InvulnerableToMobs);
		this.cc.RemoveEditorTag(EditorTag.Invulnerable);
	}

	public void event_swordkeeper()
	{
		EMono._zone.AddChara("swordkeeper", 45, 52);
	}

	public static bool If(DramaChoice item, Chara c)
	{
		item.IF.Split('/', StringSplitOptions.None)[0] == "costHire";
		return true;
	}

	public void guild_trial()
	{
		if (Guild.Current == EMono.game.factions.Merchant)
		{
			EMono.game.quests.Start("guild_merchant", this.cc, false);
			return;
		}
		(this.cc.trait as TraitGuildDoorman).GiveTrial();
	}

	public void guild_join()
	{
		if (Guild.Current != EMono.game.factions.Merchant)
		{
			(this.cc.trait as TraitGuildDoorman).OnJoinGuild();
		}
		Msg.Say("guild_join", Guild.Current.Name, null, null, null);
		SE.Play("questComplete");
		Guild.Current.relation.type = FactionRelation.RelationType.Member;
		Guild.CurrentQuest.ChangePhase(10);
	}

	public void guild_mageTrial()
	{
		Guild.CurrentQuest.NextPhase();
		Thing thing = EMono.pc.things.Find("letter_trial", -1, -1);
		if (thing != null)
		{
			thing.ModNum(-1, true);
		}
	}

	public void guild_promote()
	{
		Guild.Current.relation.Promote();
		Guild currentGuild = Guild.GetCurrentGuild();
		if (currentGuild == null)
		{
			return;
		}
		currentGuild.RefreshDevelopment();
	}

	public bool check_sketch()
	{
		Thing thing = EMono.pc.things.Find("sketch_old", -1, -1);
		if (thing == null)
		{
			return false;
		}
		int num = thing.Num;
		thing.Destroy();
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				int item = EMono.core.refs.dictSketches.Keys.RandomItem<int>();
				if (!EMono.player.sketches.Contains(item))
				{
					EMono.player.sketches.Add(item);
					Msg.Say("add_sketch", item.ToString() ?? "", null, null, null);
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			SE.WriteJournal();
			return true;
		}
		return false;
	}

	public int GetFelmeraRewardIndex()
	{
		int count = EMono.player.sketches.Count;
		int num = count / 10;
		if (num >= 29)
		{
			num = 29;
		}
		Debug.Log(string.Concat(new string[]
		{
			count.ToString(),
			"/",
			num.ToString(),
			"/",
			EMono.player.lastFelmeraReward.ToString()
		}));
		return num;
	}

	public bool check_sketch2()
	{
		return this.GetFelmeraRewardIndex() > EMono.player.lastFelmeraReward;
	}

	public List<Thing> ListFelmeraBarter()
	{
		List<Thing> list = new List<Thing>();
		int felmeraRewardIndex = this.GetFelmeraRewardIndex();
		for (int i = 0; i < felmeraRewardIndex; i++)
		{
			Thing thing = ThingGen.Create("painting_reward", -1, -1);
			thing.idSkin = i;
			list.Add(thing);
		}
		return list;
	}

	public void give_sketch_reward()
	{
		int felmeraRewardIndex = this.GetFelmeraRewardIndex();
		for (int i = EMono.player.lastFelmeraReward; i < felmeraRewardIndex; i++)
		{
			Thing thing = ThingGen.Create("painting_reward", -1, -1);
			thing.idSkin = i;
			EMono.player.DropReward(thing, false);
		}
		EMono.player.lastFelmeraReward = felmeraRewardIndex;
	}

	public void give_sketch_special()
	{
		foreach (int num in EMono.core.refs.dictSketches.Keys)
		{
			if (num >= 500 && num < 700)
			{
				EMono.player.sketches.Add(num);
			}
		}
		Msg.Say("add_sketch_special");
		SE.WriteJournal();
		EMono.pc.things.Find("sketch_special", -1, -1).Destroy();
	}

	public static string idJump;

	public DramaManager manager;
}
