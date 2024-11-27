using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class Game : EClass
{
	public static bool IsWaiting
	{
		get
		{
			return Game.waitTimer > 0f;
		}
	}

	public new static void Wait(float a, Point p)
	{
		if (p != null)
		{
			bool isSync = p.IsSync;
		}
	}

	public new static void Wait(float a, Card c)
	{
		if (c == null || !c.isSynced || c.IsPC)
		{
			return;
		}
		Game.Wait(a);
	}

	public static void Wait(float a)
	{
		Game.waitTimer = a * (float)(EClass.core.config.game.waiter * 25) / 100f;
	}

	public Prologue Prologue
	{
		get
		{
			return EClass.setting.start.prologues[this.idPrologue];
		}
	}

	public GameDifficulty Difficulty
	{
		get
		{
			return EClass.setting.start.difficulties[this.idDifficulty];
		}
	}

	public bool UseGrid
	{
		get
		{
			return EClass.core.config.game.useGrid;
		}
	}

	public bool altUI
	{
		get
		{
			return EClass.core.config.game.altUI;
		}
	}

	public bool altInv
	{
		get
		{
			return EClass.core.config.game.altInv;
		}
	}

	public bool altCraft
	{
		get
		{
			return EClass.core.config.game.altCraft;
		}
	}

	public bool altAbility
	{
		get
		{
			return EClass.core.config.game.altAbility;
		}
	}

	public Zone StartZone
	{
		get
		{
			return this.spatials.Find(EClass.game.Prologue.idStartZone);
		}
	}

	public void OnUpdate()
	{
		this.backupTime += (double)Time.deltaTime;
		Game.waitTimer -= Time.deltaTime;
		this.timeSinceStart += Time.deltaTime;
		this.player.stats.timeElapsed += (double)Time.deltaTime;
		if (!this.flags.IsWelcomeMessageShown && this.player.stats.timeElapsed > 2.0)
		{
			this.flags.IsWelcomeMessageShown = true;
		}
		this.updater.FixedUpdate();
	}

	public static void Load(string slot)
	{
		Game.Load(slot, GameIO.pathSaveRoot + slot);
	}

	public static void Load(string id, string root)
	{
		Debug.Log("Loading: " + id + ": " + root);
		if (EClass.game != null)
		{
			EClass.game.Kill();
		}
		Game.OnBeforeInstantiate();
		EClass.core.game = GameIO.LoadGame(id, root);
		EClass.game.isLoading = true;
		EClass.game.OnGameInstantiated();
		EClass.game.OnLoad();
		EClass.scene.Init(Scene.Mode.StartGame);
		if (EClass.game.altInv)
		{
			SoundManager.ignoreSounds = true;
			if (EClass.game.player.pref.layerInventory)
			{
				EClass.ui.OpenFloatInv(false);
			}
			SoundManager.ignoreSounds = true;
			if (EClass.game.player.pref.layerAbility)
			{
				EClass.ui.ToggleAbility(false);
			}
			SoundManager.ignoreSounds = false;
		}
		EClass.game.isLoading = false;
	}

	public void OnLoad()
	{
		this.domains.OnLoad();
		this.religions.OnLoad();
		this.factions.OnLoad();
		this.world._OnLoad();
		this.player.OnLoad();
		if (EClass.pc.currentZone == null || EClass.pc.currentZone.destryoed)
		{
			EClass.pc.party.members.ForEach(delegate(Chara c)
			{
				c.currentZone = EClass.pc.homeZone;
				c.global.transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Return
				};
			});
		}
		this.player.zone = EClass.pc.currentZone;
		EClass.pc.currentZone.lastActive = this.world.date.GetRaw(0);
		if (this.spatials.Find("somewhere") == null)
		{
			SpatialGen.Create("somewhere", this.world.region, true, -99999, -99999, 0);
		}
		EClass.pc.homeZone.isKnown = true;
		EClass.debug.OnLoad();
		EClass.pc.things.RefreshGridRecursive();
		foreach (Thing thing in EClass.pc.things.List((Thing t) => t.Num <= 0 || t.isDestroyed, false))
		{
			thing.parent.RemoveCard(thing);
		}
		foreach (Thing thing2 in EClass.pc.things.List((Thing t) => t.trait is TraitChestMerchant, false))
		{
			thing2.Destroy();
		}
		foreach (Thing thing3 in EClass.pc.things.List((Thing t) => t.invY == 1 && t.invX == -1, false))
		{
			thing3.invY = 0;
		}
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara.id == "demitas" && chara.faction.id == "wilds")
			{
				EClass.pc.homeBranch.AddMemeber(chara);
			}
			if (chara.memberType == FactionMemberType.Livestock && !chara.IsPCFaction)
			{
				chara.memberType = FactionMemberType.Default;
			}
		}
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			foreach (Chara chara2 in factionBranch.members)
			{
				if (!chara2.isDead && (chara2.currentZone == null || chara2.currentZone.id == "somewhere"))
				{
					string[] array = new string[8];
					array[0] = "exception: Moving invalid chara: ";
					array[1] = chara2.Name;
					array[2] = "/";
					int num = 3;
					Zone currentZone = chara2.currentZone;
					array[num] = ((currentZone != null) ? currentZone.ToString() : null);
					array[4] = "/";
					int num2 = 5;
					FactionBranch homeBranch = chara2.homeBranch;
					array[num2] = ((homeBranch != null) ? homeBranch.ToString() : null);
					array[6] = "/";
					int num3 = 7;
					Faction faction = chara2.faction;
					array[num3] = ((faction != null) ? faction.ToString() : null);
					Debug.Log(string.Concat(array));
					chara2.MoveZone(factionBranch.owner, ZoneTransition.EnterState.RandomVisit);
				}
				chara2.RefreshWorkElements(factionBranch.owner.elements);
				chara2.Refresh(false);
			}
		}
		EClass.pc.angle = this.player.angle;
		EClass.pc.elements.CheckSkillActions();
		if (this.cards.listAdv.Count == 0)
		{
			this.AddAdventurers();
		}
		this.quests.list.ForeachReverse(delegate(Quest q)
		{
			if (!(q is QuestRandom))
			{
				foreach (Quest quest3 in this.quests.list)
				{
					if (q != quest3 && quest3.id == q.id)
					{
						this.quests.list.Remove(quest3);
						break;
					}
				}
			}
		});
		this.quests.globalList.ForeachReverse(delegate(Quest q)
		{
			if (!(q is QuestRandom))
			{
				foreach (Quest quest3 in this.quests.globalList)
				{
					if (q != quest3 && quest3.id == q.id)
					{
						this.quests.globalList.Remove(quest3);
						break;
					}
				}
			}
		});
		if (this.version.IsBelow(0, 22, 91))
		{
			this.<OnLoad>g__TryAddQuestIfActive|67_5("demitas_spellwriter", "into_darkness");
		}
		if (this.version.IsBelow(0, 22, 91))
		{
			this.<OnLoad>g__TryAddQuest|67_4("into_darkness", "exile_kettle");
		}
		if (this.version.IsBelow(0, 22, 86))
		{
			Chara chara3 = this.cards.globalCharas.Find("demitas");
			if (chara3 != null && chara3._works != null && chara3._works.Count > 0)
			{
				chara3._works[0] = 51;
			}
		}
		if (this.version.IsBelow(0, 22, 86))
		{
			this.<OnLoad>g__TryAddQuest|67_4("farris_tulip", "greatDebt");
		}
		if (this.version.IsBelow(0, 22, 86))
		{
			this.<OnLoad>g__TryAddQuest|67_4("exile_whisper", "exile_kettle");
		}
		if (this.version.IsBelow(0, 22, 85))
		{
			this.<OnLoad>g__TryAddQuest|67_4("exile_meet", "pre_debt");
		}
		if (this.version.IsBelow(0, 22, 60) && EClass.game.quests.Get<QuestDebt>() != null)
		{
			Chara chara4 = this.cards.globalCharas.Find("loytel");
			string str = "Loytelfix: ";
			Zone homeZone = chara4.homeZone;
			Debug.Log(str + ((homeZone != null) ? homeZone.ToString() : null));
			if (chara4.homeZone == null || !chara4.homeZone.IsPCFaction)
			{
				EClass.pc.homeBranch.AddMemeber(chara4);
				chara4.homeZone = EClass.pc.homeBranch.owner;
				chara4.RemoveEditorTag(EditorTag.Invulnerable);
			}
		}
		if (this.version.IsBelow(0, 22, 52) && this.quests.completedIDs.Contains("vernis_gold"))
		{
			this.quests.Add("pre_debt", "farris");
		}
		if (this.quests.completedIDs.Contains("farris_tulip"))
		{
			this.quests.globalList.ForeachReverse(delegate(Quest q)
			{
				if (q.id == "farris_tulip")
				{
					this.quests.globalList.Remove(q);
				}
			});
		}
		this.quests.list.ForeachReverse(delegate(Quest q)
		{
			if (q is QuestDialog && this.quests.completedIDs.Contains(q.id))
			{
				this.quests.list.Remove(q);
			}
		});
		if (this.version.IsBelow(0, 22, 20))
		{
			foreach (Chara chara5 in this.cards.globalCharas.Values)
			{
				chara5.SetBool(18, false);
			}
		}
		if (this.version.IsBelow(0, 22, 22))
		{
			if (EClass.pc.faithElements != null)
			{
				EClass.pc.faithElements.SetParent(null);
			}
			foreach (Element element in EClass.pc.elements.ListElements((Element e) => e.source.categorySub == "god", null))
			{
				EClass.pc.SetFeat(element.id, 0, false);
			}
			EClass.pc.RefreshFaithElement();
		}
		if (this.version.IsBelow(0, 22, 45))
		{
			this.player.debt = 20000000;
		}
		if (!this.version.Equals(EClass.core.version))
		{
			this.player.recipes.OnVersionUpdate();
		}
		if (this.cards.container_deposit == null)
		{
			this.cards.container_deposit = ThingGen.Create("container_deposit", -1, -1);
			if (this.player.bankMoney > 0)
			{
				this.cards.container_deposit.Add("money", this.player.bankMoney, 1);
			}
		}
		foreach (Quest quest in this.quests.list)
		{
			if (quest is QuestSequence && !EClass.sources.quests.map.ContainsKey(quest.idSource))
			{
				quest.phase = 0;
			}
		}
		foreach (Quest quest2 in this.quests.globalList)
		{
			if (quest2 is QuestSequence && !EClass.sources.quests.map.ContainsKey(quest2.idSource))
			{
				quest2.phase = 0;
			}
		}
	}

	public static void Create(string _id = null)
	{
		Game.id = (_id ?? GameIO.GetNewId(GameIO.pathSaveRoot, "world_", 1));
		GameIO.ResetTemp();
		Game.OnBeforeInstantiate();
		EClass.core.game = (Game.Instance = new Game());
		EClass.core.game.OnGameInstantiated();
		EClass.core.game._Create();
	}

	public static void OnBeforeInstantiate()
	{
		Player.seedHallucination = 0;
	}

	public void OnGameInstantiated()
	{
		HotItemHeld.taskBuild = null;
		InvOwner.Trader = (InvOwner.Main = null);
		LayerDrama.currentQuest = null;
		BookList.Init();
		ContentGallery.lastPage = 0;
		ContentGallery.listMode = false;
		if (!Application.isEditor || !EClass.debug.enable)
		{
			RecipeManager.BuildList();
		}
		foreach (Chara chara in this.cards.globalCharas.Values)
		{
			if (chara.uid == this.player.uidChara)
			{
				this.player.chara = chara;
				break;
			}
		}
		ActionMode.OnGameInstantiated();
		EClass.ui.widgets.OnGameInstantiated();
		EClass.Sound.ResetPlaylist();
		EClass.core.config.ApplyFPS(true);
	}

	public void _Create()
	{
		this.config.snapFreePos = (this.config.slope = (this.config.autoWall = true));
		this.config.autoCombat.abortOnAllyDead = true;
		this.config.autoCombat.abortOnHalfHP = true;
		this.config.autoCombat.bUseHotBar = true;
		this.config.autoCombat.bUseInventory = true;
		this.config.autoCombat.bDontChangeTarget = true;
		this.config.autoCombat.abortOnKill = true;
		this.config.autoCombat.abortOnItemLoss = true;
		this.idDifficulty = 1;
		this.seed = EClass.rnd(10000);
		Debug.Log("creating game: " + Game.id + " seed:" + this.seed.ToString());
		this.uniforms.Import("uniforms");
		this.religions.OnCreateGame();
		this.factions.OnCreateGame();
		this.world = (SpatialGen.CreateRecursive("world", null) as World);
		this.domains.OnCreateGame();
		foreach (Spatial spatial in this.spatials.map.Values)
		{
			spatial.SetMainFaction(this.factions.Find(spatial.source.faction.IsEmpty("wilds")));
		}
		this.player.OnCreateGame();
		this.cards.container_shipping = ThingGen.Create("container_shipping", -1, -1);
		this.cards.container_deliver = ThingGen.Create("container_delivery", -1, -1);
		this.cards.container_deposit = ThingGen.Create("container_deposit", -1, -1);
		this.bp = new GameBlueprint();
		this.bp.Create();
	}

	public void StartNewGame()
	{
		EClass.pc.homeZone = this.StartZone;
		EClass.pc.SetGlobal();
		this.parties.Create(EClass.pc);
		Prologue prologue = EClass.game.Prologue;
		CharaGen.Create("fiama", -1).SetGlobal(EClass.game.StartZone, prologue.posFiama.x, prologue.posFiama.y);
		CharaGen.Create("ashland", -1).SetGlobal(EClass.game.StartZone, prologue.posAsh.x, prologue.posAsh.y);
		if (LayerTitle.actor)
		{
			this.world.date.hour = EClass.game.Prologue.hour;
			EClass.core.actionsNextFrame.Add(new Action(LayerTitle.KillActor));
			this.player.zone = (EClass.pc.currentZone = (EClass.pc.homeZone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone)));
			EClass.pc.global.transition = new ZoneTransition
			{
				state = ZoneTransition.EnterState.Exact,
				x = EClass.game.Prologue.startX,
				z = EClass.game.Prologue.startZ
			};
			EClass.pc.hp = EClass.pc.MaxHP;
			EClass.pc.mana.Set(EClass.pc.mana.max / 2);
			EClass.pc.stamina.Set(EClass.pc.stamina.max / 2);
			EClass.pc.AddCondition<ConFaint>(200, true);
		}
		else
		{
			EClass.pc.homeZone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
			Zone zone = null;
			ZoneTransition transition = new ZoneTransition
			{
				state = ZoneTransition.EnterState.Auto
			};
			Chara c = EClass.game.cards.globalCharas.Find("ashland");
			switch (EClass.debug.startScene)
			{
			case CoreDebug.StartScene.Zone:
				zone = EClass.game.spatials.Find(EClass.debug.startZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				break;
			case CoreDebug.StartScene.Home:
			case CoreDebug.StartScene.Story_Test:
				zone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				break;
			case CoreDebug.StartScene.Home_Cave:
				EClass.game.idPrologue = 2;
				zone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				break;
			case CoreDebug.StartScene.MeetFarris:
			{
				Zone parent = EClass.game.spatials.Find("nymelle");
				zone = (SpatialGen.Create("nymelle", parent, true, -99999, -99999, 0) as Zone);
				zone.lv = -2;
				EClass.game.quests.Start("exploration", c, false);
				break;
			}
			case CoreDebug.StartScene.NymelleBoss:
			{
				Zone parent2 = EClass.game.spatials.Find("nymelle");
				zone = (SpatialGen.Create("nymelle_boss", parent2, true, -99999, -99999, 0) as Zone);
				zone.lv = -5;
				EClass.game.quests.Start("exploration", c, false).ChangePhase(2);
				break;
			}
			case CoreDebug.StartScene.AfterNymelle:
				zone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				EClass.game.quests.Start("exploration", c, false).ChangePhase(5);
				EClass.game.quests.globalList.Add(Quest.Create("sharedContainer", null, null).SetClient(c, false));
				EClass.game.quests.globalList.Add(Quest.Create("crafter", null, null).SetClient(c, false));
				EClass.game.quests.globalList.Add(Quest.Create("defense", null, null).SetClient(c, false));
				break;
			case CoreDebug.StartScene.Melilith:
			{
				Zone parent3 = EClass.game.spatials.Find("cursed_manor");
				zone = (SpatialGen.Create("cursed_manor_dungeon", parent3, true, -99999, -99999, 0) as Zone);
				zone.lv = -5;
				break;
			}
			case CoreDebug.StartScene.Tefra:
			{
				Zone parent4 = EClass.game.spatials.Find("vernis");
				zone = (SpatialGen.Create("vernis_mine", parent4, true, -99999, -99999, 0) as Zone);
				zone.lv = -7;
				break;
			}
			}
			if (zone != null)
			{
				this.player.zone = (EClass.pc.currentZone = (EClass.pc.homeZone = zone));
				EClass.pc.global.transition = transition;
			}
			else
			{
				this.player.zone = (EClass.pc.currentZone = EClass.game.spatials.Find(EClass.debug.startZone));
				EClass.pc.global.transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Return
				};
			}
		}
		EClass.pc.homeZone.isKnown = true;
		this.AddAdventurers();
		this.player.OnStartNewGame();
		EClass.scene.Init(Scene.Mode.StartGame);
	}

	public void AddAdventurers()
	{
		List<Zone> source = this.world.region.ListTowns();
		string[] array = new string[]
		{
			"adv_kiria",
			"adv_gaki",
			"adv_wini",
			"adv_verna",
			"adv_ivory",
			"adv_mesherada"
		};
		for (int i = 0; i < EClass.setting.balance.numAdv; i++)
		{
			Zone zone = source.RandomItem<Zone>();
			if (zone is Zone_SubTown && EClass.rnd(5) != 0)
			{
				i--;
			}
			else
			{
				if (i >= array.Length && i < array.Length + 2)
				{
					CardBlueprint.Set(CardBlueprint.Chara(i * 15, Rarity.Normal));
				}
				Chara chara = CharaGen.Create((i < array.Length) ? array[i] : ((EClass.rnd(10) == 0) ? "adv_fairy" : "adv"), -1);
				chara.SetHomeZone(zone);
				chara.global.transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.RandomVisit
				};
				zone.AddCard(chara);
				this.cards.listAdv.Add(chara);
			}
		}
	}

	public void GotoTitle(bool showDialog = true)
	{
		if (showDialog)
		{
			Dialog.YesNo("dialog_gotoTitle", delegate
			{
				EClass.game.Save(true, delegate
				{
					EClass.scene.Init(Scene.Mode.Title);
				}, false);
			}, null, "yes", "no");
			return;
		}
		EClass.scene.Init(Scene.Mode.Title);
	}

	public void Quit()
	{
		Dialog.YesNo("dialog_quit", delegate
		{
			EClass.game.Save(true, delegate
			{
				EClass.core.Quit();
			}, false);
		}, null, "yes", "no");
	}

	public void Save(bool isAutoSave = false, Action onComplete = null, bool silent = false)
	{
		if (EClass.ui.IsDragging)
		{
			EClass.ui.EndDrag(true);
		}
		if (!isAutoSave && !silent)
		{
			SE.WriteJournal();
		}
		if (isAutoSave && EClass.debug.ignoreAutoSave)
		{
			if (onComplete != null)
			{
				onComplete();
			}
			return;
		}
		bool flag = EClass.core.config.game.autoBackup && this.backupTime >= (double)(EClass.core.config.game.backupInterval * 60 * 30);
		if (flag)
		{
			this.backupTime = 0.0;
		}
		this.countLoadedMaps = 0;
		this.player.pref.layerInventory = EClass.ui.layerFloat.GetLayer<LayerInventory>(false);
		if (this.player.pref.layerInventory)
		{
			this.player.pref.layerAbility = EClass.ui.layerFloat.GetLayer<LayerAbility>(false);
		}
		this.player.pref.layerCraft = EClass.ui.layerFloat.GetLayer<LayerCraftFloat>(false);
		this.player.angle = EClass.pc.angle;
		this.version = EClass.core.version;
		EClass.ui.widgets.UpdateConfigs();
		this.OnBeforeSave();
		GameIndex index = GameIO.SaveGame();
		if (flag)
		{
			GameIO.MakeBackup(index, "");
			EClass.ui.Say("backupDone", null);
		}
		if (!silent)
		{
			Msg.Say("saved");
		}
		this.saveCount++;
		if (onComplete != null)
		{
			onComplete();
		}
	}

	public void OnBeforeSave()
	{
		foreach (Zone zone in this.spatials.Zones)
		{
			zone.OnBeforeSave();
		}
		this.player.OnBeforeSave();
		foreach (Spatial spatial in this.spatials.listDestryoed)
		{
			spatial.DeleteMapRecursive();
		}
		this.spatials.listDestryoed.Clear();
	}

	public void Kill()
	{
		this.isKilling = true;
		Debug.Log("Killing Game: IsStarted? " + EClass.core.IsGameStarted.ToString());
		if (EClass.core.IsGameStarted)
		{
			EClass.ui.OnKillGame();
			EClass.scene.OnKillGame();
			this.activeZone.OnKillGame();
		}
		EClass.core.game = (Game.Instance = null);
		EClass.Sound.currentPlaylist = null;
		Window.dictTab.Clear();
		foreach (Texture2D texture2D in this.loadedTextures)
		{
			if (texture2D != null)
			{
				UnityEngine.Object.Destroy(texture2D);
			}
		}
	}

	public void Pause(Action onUnpause = null)
	{
		Game.isPaused = true;
		LayerPause layerPause = EClass.ui.AddLayer<LayerPause>();
		Game lastGame = EClass.game;
		if (onUnpause != null)
		{
			layerPause.SetOnKill(delegate
			{
				if (!EClass.core.IsGameStarted || lastGame != EClass.game)
				{
					return;
				}
				onUnpause();
			});
		}
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		Debug.Log("#io game OnSerializing");
	}

	[OnDeserializing]
	private void OnDeserializing(StreamingContext context)
	{
		Debug.Log("#io game OnDeserializing");
		Game.Instance = this;
	}

	[CompilerGenerated]
	private void <OnLoad>g__TryAddQuest|67_4(string idQuest, string idReqQuest)
	{
		if (this.quests.completedIDs.Contains(idReqQuest) && !this.quests.completedIDs.Contains(idQuest) && this.quests.GetGlobal(idQuest) == null && this.quests.Get(idQuest) == null)
		{
			this.quests.Add(idQuest, null);
		}
	}

	[CompilerGenerated]
	private void <OnLoad>g__TryAddQuestIfActive|67_5(string idQuest, string idReqQuest)
	{
		if (this.quests.Get(idReqQuest) != null && !this.quests.completedIDs.Contains(idQuest) && this.quests.GetGlobal(idQuest) == null && this.quests.Get(idQuest) == null)
		{
			this.quests.Add(idQuest, null);
		}
	}

	public static Game Instance;

	public static float waitTimer;

	public static string id;

	public static bool isPaused;

	[JsonProperty(Order = -90)]
	public SpatialManager spatials = new SpatialManager();

	[JsonProperty(Order = -80)]
	public CardManager cards = new CardManager();

	[JsonProperty(Order = -100)]
	public ReligionManager religions = new ReligionManager();

	[JsonProperty(Order = -60)]
	public FactionManager factions = new FactionManager();

	[JsonProperty(Order = -50)]
	public DomainManager domains = new DomainManager();

	[JsonProperty]
	public global::Version version;

	[JsonProperty]
	public PartyManager parties = new PartyManager();

	[JsonProperty]
	public new World world;

	[JsonProperty]
	public new Player player = new Player();

	[JsonProperty]
	public QuestManager quests = new QuestManager();

	[JsonProperty]
	public RelationManager relations = new RelationManager();

	[JsonProperty]
	public MsgLog log = new MsgLog
	{
		id = "log"
	};

	[JsonProperty]
	public PCCUniformManager uniforms = new PCCUniformManager();

	[JsonProperty]
	public TeleportManager teleports = new TeleportManager();

	[JsonProperty]
	public int seed;

	[JsonProperty]
	public int idPrologue;

	[JsonProperty]
	public int idDifficulty;

	[JsonProperty]
	public int dateScratch;

	[JsonProperty]
	public double backupTime;

	[JsonProperty]
	public Game.Flags flags = new Game.Flags();

	[JsonProperty]
	public Game.Config config = new Game.Config();

	[JsonProperty]
	public List<Thing> lostThings = new List<Thing>();

	public int gameSpeedIndex = 1;

	public int lastGameSpeedIndex = 1;

	public int sessionMin;

	public int countLoadedMaps;

	public int saveCount;

	public float timeSinceStart;

	public bool isLoading;

	public bool isKilling;

	public GameBlueprint bp;

	public GameUpdater updater = new GameUpdater();

	public Zone activeZone;

	public Zone lastActiveZone;

	public Dictionary<string, IGlobalValue> referenceMap = new Dictionary<string, IGlobalValue>();

	public HashSet<Texture2D> loadedTextures = new HashSet<Texture2D>();

	private float welcomeTimer;

	public class Config : EClass
	{
		public bool FreePos
		{
			get
			{
				return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			}
		}

		[JsonProperty]
		public bool showRoof;

		[JsonProperty]
		public bool showWall;

		[JsonProperty]
		public bool tilt;

		[JsonProperty]
		public bool tiltRegion;

		[JsonProperty]
		public bool slope;

		[JsonProperty]
		public bool noRoof;

		[JsonProperty]
		public bool freePos;

		[JsonProperty]
		public bool snapFreePos;

		[JsonProperty]
		public bool buildLight;

		[JsonProperty]
		public bool highlightArea;

		[JsonProperty]
		public bool autoWall;

		[JsonProperty]
		public bool showGuideGrid;

		[JsonProperty]
		public bool reverseSnow;

		[JsonProperty]
		public bool autoCollectCard;

		[JsonProperty]
		public bool cheat;

		[JsonProperty]
		public int slopeMod = 50;

		[JsonProperty]
		public int defaultZoom = 100;

		[JsonProperty]
		public int zoomedZoom = 125;

		[JsonProperty]
		public int regionZoom = 100;

		[JsonProperty]
		public int tiltPower = 80;

		[JsonProperty]
		public int tiltPowerRegion = 50;

		[JsonProperty]
		public int gridIconSize = 100;

		[JsonProperty]
		public int backDrawAlpha = 10;

		[JsonProperty]
		public int animeSpeed = 100;

		[JsonProperty]
		public int lowWallObjAltitude = 2;

		[JsonProperty]
		public ConfigTactics tactics = new ConfigTactics();

		[JsonProperty]
		public ConfigAutoCombat autoCombat = new ConfigAutoCombat();

		[JsonProperty]
		public ConfigPreference preference = new ConfigPreference();
	}

	public class Flags : EClass
	{
		public bool IsWelcomeMessageShown
		{
			get
			{
				return this.b1[0];
			}
			set
			{
				this.b1[0] = value;
			}
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			this._b1 = (int)this.b1.Bits;
		}

		[OnDeserialized]
		private void _OnDeserialized(StreamingContext context)
		{
			this.b1.Bits = (uint)this._b1;
		}

		[JsonProperty]
		public int _b1;

		public BitArray32 b1;
	}
}
