using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class Player : EClass
{
	public WidgetManager.SaveData widgets
	{
		get
		{
			if (!this.useSubWidgetTheme)
			{
				return this.mainWidgets;
			}
			return this.subWidgets;
		}
		set
		{
			if (this.useSubWidgetTheme)
			{
				this.subWidgets = value;
				return;
			}
			this.mainWidgets = value;
		}
	}

	public Zone spawnZone
	{
		get
		{
			return RefZone.Get(this.uidSpawnZone);
		}
		set
		{
			this.uidSpawnZone = RefZone.Set(value);
		}
	}

	public bool EnableDreamStory
	{
		get
		{
			return false;
		}
	}

	public Zone LastTravelZone
	{
		get
		{
			return RefZone.Get(this.uidLastTravelZone);
		}
	}

	public Zone LastZone
	{
		get
		{
			return RefZone.Get(this.uidLastZone);
		}
	}

	public Chara Agent
	{
		get
		{
			return this._agent;
		}
	}

	public int ContainerSearchDistance
	{
		get
		{
			return 1 + EClass.pc.elements.GetFeatRef(1643, 0);
		}
	}

	public int MaxAlly
	{
		get
		{
			return Mathf.Min(Mathf.Max(EClass.pc.CHA / 10, 1), 5) + EClass.pc.Evalue(1645);
		}
	}

	public int MaxExpKnowledge
	{
		get
		{
			return 1000;
		}
	}

	public int MaxExpInfluence
	{
		get
		{
			return 1000;
		}
	}

	public bool IsMageGuildMember
	{
		get
		{
			Quest quest = EClass.game.quests.Get("guild_mage");
			return quest != null && quest.phase >= 10;
		}
	}

	public bool IsFighterGuildMember
	{
		get
		{
			Quest quest = EClass.game.quests.Get("guild_fighter");
			return quest != null && quest.phase >= 10;
		}
	}

	public bool IsThiefGuildMember
	{
		get
		{
			Quest quest = EClass.game.quests.Get("guild_thief");
			return quest != null && quest.phase >= 10;
		}
	}

	public bool IsMerchantGuildMember
	{
		get
		{
			Quest quest = EClass.game.quests.Get("guild_merchant");
			return quest != null && quest.phase >= 10;
		}
	}

	public bool IsCriminal
	{
		get
		{
			return this.karma < 0 && !EClass.pc.HasCondition<ConIncognito>();
		}
	}

	public void OnCreateGame()
	{
		this.karma = 30;
		this.debt = 20000000;
		this.chara = CharaGen.Create("chara", -1);
		this.chara.SetFaction(EClass.Home);
		this.chara.things.SetSize(7, 5);
		this.chara.c_idTone = "default";
		this.chara.SetInt(56, 1);
		this.chara.things.DestroyAll(null);
		this.chara.faith = EClass.game.religions.Eyth;
		this.chara.RefreshFaithElement();
		this.uidChara = this.chara.uid;
		Fav fav = IO.LoadFile<Fav>(CorePath.user + "PCC/fav" + EClass.rnd(3).ToString(), false, null);
		this.chara.pccData = IO.Duplicate<PCCData>(fav.data);
		this.chara._CreateRenderer();
		this.RefreshDomain();
		this._agent = CharaGen.Create("chara", -1);
		this._agent.c_altName = "stash".lang();
		EClass.player.title = "master".lang();
		this.nums.OnCreateGame();
		this.hotbars.OnCreateGame();
		this.baseActTime = EClass.setting.defaultActPace;
		this.flags.OnCreateGame();
		this.sketches.Add(115);
		this.pref.sort_ascending_shop = true;
		EClass.game.config.preference.autoEat = true;
		this.flags.toggleHotbarHighlightDisabled = true;
		this.layerAbilityConfig.hideDepletedSpell = true;
		this.layerAbilityConfig.bgAlpha = 70;
		this.memo = "memo_blank".lang();
	}

	public void OnStartNewGame()
	{
		this.ModKeyItem("old_charm", 1, false);
		this.ModKeyItem("backpack", 1, false);
		EClass.player.knownBGMs.Add(1);
		EClass.player.knownBGMs.Add(3);
		EClass.player.questTracker = true;
		this.trackedCategories.Add("food");
		this.trackedCategories.Add("drink");
		this.trackedCategories.Add("resource");
		EClass.game.quests.Start("main", null, true);
		this.chara.hp = this.chara.MaxHP;
		this.chara.SetFaith(EClass.game.religions.list[0]);
		this.chara.elements.SetBase(135, 1, 0);
		this.chara.elements.SetBase(6003, 1, 0);
		this.chara.elements.SetBase(6012, 1, 0);
		this.chara.elements.SetBase(6015, 1, 0);
		this.chara.elements.SetBase(6050, 1, 0);
		List<Element> list = new List<Element>();
		foreach (Element item in EClass.pc.elements.dict.Values)
		{
			list.Add(item);
		}
		foreach (Element element in list)
		{
			if (element.Value == 0)
			{
				EClass.pc.elements.Remove(element.id);
			}
			else if (element.HasTag("primary"))
			{
				element.vTempPotential = Mathf.Max(30, (element.ValueWithoutLink - 8) * 7);
			}
		}
		foreach (BodySlot slot in this.chara.body.slots)
		{
			this.chara.body.Unequip(slot, true);
		}
		this.chara.things.DestroyAll(null);
		this.CreateEquip();
		this.dateTravel = EClass.world.date.GetRaw(0);
		this.uidLastTravelZone = EClass.pc.currentZone.uid;
		this.GenerateBackgroundText();
		if (!EClass.game.Difficulty.allowRevive)
		{
			EClass.pc.SetFeat(1220, 1, false);
		}
		EClass.pc.elements.CheckSkillActions();
		EClass.pc.hunger.value = 30;
		EClass.pc.CalculateMaxStamina();
		EClass.pc.stamina.Set(EClass.pc.stamina.max / 2);
		EClass.pc.Refresh(false);
		this.isEditor = Application.isEditor;
	}

	public void OnLoad()
	{
		this.nums.OnLoad();
		this.codex.OnLoad();
		if (this.dataWindow != null)
		{
			Window.dictData = this.dataWindow;
		}
		EClass.pc.Refresh(false);
		if (Application.isEditor && EClass.debug.resetPlayerConfig && !this.isEditor)
		{
			EClass.game.config = new Game.Config();
			if (this.dataWindow != null)
			{
				this.dataWindow.Clear();
			}
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				t.c_windowSaveData = null;
			}, false);
		}
		this.isEditor = Application.isEditor;
	}

	public void OnBeforeSave()
	{
		foreach (Layer comp in EClass.ui.layerFloat.layers)
		{
			foreach (Window window in comp.GetComponentsInDirectChildren(true))
			{
				window.UpdateSaveData();
			}
		}
		this.dataWindow = Window.dictData;
	}

	public void SetPriorityAction(string id, Thing t)
	{
		if (t == null)
		{
			return;
		}
		if (id.IsEmpty())
		{
			using (Dictionary<string, List<string>>.ValueCollection.Enumerator enumerator = this.priorityActions.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					List<string> p = enumerator.Current;
					p.ForeachReverse(delegate(string i)
					{
						if (i == t.id)
						{
							p.Remove(i);
						}
					});
				}
				return;
			}
		}
		if (!this.priorityActions.ContainsKey(id))
		{
			this.priorityActions[id] = new List<string>();
		}
		if (!this.priorityActions[id].Contains(t.id))
		{
			this.priorityActions[id].Add(t.id);
		}
	}

	public bool IsPriorityAction(string id, Thing t)
	{
		if (!id.IsEmpty() && t != null)
		{
			List<string> list = this.priorityActions.TryGetValue(id, null);
			if (list != null && list.Contains(t.id))
			{
				return true;
			}
		}
		return false;
	}

	public void OnAdvanceRealHour()
	{
		Player.realHour++;
		Msg.Say("time_warn", Player.realHour.ToString() ?? "", null, null, null);
		string text = "time_warn_" + Player.realHour.ToString();
		if (LangGame.Has(text))
		{
			Msg.Say(text);
		}
	}

	public void RefreshDomain()
	{
		this.domains.Clear();
		for (int i = 0; i < EClass.pc.job.domain.Length; i += 2)
		{
			this.domains.Add(EClass.pc.job.domain[i]);
		}
	}

	public ElementContainer GetDomains()
	{
		ElementContainer elementContainer = new ElementContainer();
		foreach (int id in this.domains)
		{
			elementContainer.GetOrCreateElement(id).vSource = 1;
		}
		return elementContainer;
	}

	public Layer SelectDomain(Action onKill = null)
	{
		List<SourceElement.Row> list2 = new List<SourceElement.Row>();
		foreach (SourceElement.Row row in EClass.sources.elements.rows)
		{
			if (row.categorySub == "eleAttack" && !row.tag.Contains("hidden") && (!row.tag.Contains("high") || EClass.pc.job.domain.Contains(row.id)))
			{
				list2.Add(row);
			}
		}
		return EClass.ui.AddLayer<LayerList>().SetListCheck<SourceElement.Row>(list2, (SourceElement.Row a) => a.GetName(), delegate(SourceElement.Row s, ItemGeneral b)
		{
			bool flag = false;
			foreach (int num in EClass.player.domains)
			{
				if (s.id == num)
				{
					flag = true;
				}
			}
			if (flag)
			{
				EClass.player.domains.Remove(s.id);
				return;
			}
			EClass.player.domains.Add(s.id);
		}, delegate(List<UIList.ButtonPair> list)
		{
			bool flag = EClass.player.domains.Count >= 3 + EClass.pc.Evalue(1402);
			foreach (UIList.ButtonPair buttonPair in list)
			{
				UIButton button = (buttonPair.component as ItemGeneral).button1;
				SourceElement.Row row2 = buttonPair.obj as SourceElement.Row;
				bool flag2 = false;
				bool flag3 = false;
				foreach (int num in EClass.player.domains)
				{
					if (row2.id == num)
					{
						flag3 = true;
					}
				}
				button.SetCheck(flag3);
				for (int i = 0; i < ((EClass.pc.job.id == "swordsage") ? 5 : 3); i++)
				{
					if (EClass.pc.job.domain[i * 2] == row2.id)
					{
						flag2 = true;
					}
				}
				button.interactable = (!flag2 && (!flag || flag3));
				button.GetComponent<CanvasGroup>().enabled = !button.interactable;
			}
		}).SetOnKill(delegate
		{
			Action onKill2 = onKill;
			if (onKill2 == null)
			{
				return;
			}
			onKill2();
		});
	}

	public void RefreshEmptyAlly()
	{
		int num = this.MaxAlly - EClass.pc.party.members.Count + 1;
		if (num != this.lastEmptyAlly)
		{
			this.lastEmptyAlly = num;
			foreach (Chara chara in EClass.pc.party.members)
			{
				chara.RefreshSpeed();
			}
		}
	}

	public void GenerateBackgroundText()
	{
		string text = IO.LoadText(new DirectoryInfo(CorePath.CorePackage.Background).GetFiles("*.txt").RandomItem<FileInfo>().FullName);
		IO.SaveText(GameIO.pathCurrentSave + "background.txt", text);
	}

	public string GetBackgroundText()
	{
		StringBuilder stringBuilder = new StringBuilder(IO.LoadText(GameIO.pathCurrentSave + "background.txt"));
		stringBuilder.Replace("#name", EClass.pc.NameSimple);
		stringBuilder.Replace("#aka", EClass.pc.Aka);
		stringBuilder.Replace("#father", EClass.pc.bio.nameDad);
		stringBuilder.Replace("#mother", EClass.pc.bio.nameMom);
		stringBuilder.Replace("#birthloc2", EClass.pc.bio.nameHome);
		stringBuilder.Replace("#birthloc", EClass.pc.bio.nameLoc);
		stringBuilder.Replace("#job", EClass.pc.job.GetName().AddArticle());
		stringBuilder.Replace("#year", EClass.pc.bio.birthYear.ToString() ?? "");
		stringBuilder.Replace("#month", EClass.pc.bio.birthMonth.ToString() ?? "");
		stringBuilder.Replace("#day", EClass.pc.bio.birthDay.ToString() ?? "");
		stringBuilder.Replace("#he", (EClass.pc.IsMale ? "he" : "she").lang());
		stringBuilder.Replace("#his", (EClass.pc.IsMale ? "his" : "her").lang());
		stringBuilder.Replace("#him", (EClass.pc.IsMale ? "him" : "her").lang());
		string text = "_period".lang();
		string[] array = stringBuilder.ToString().Split(text, StringSplitOptions.None);
		string text2 = "";
		if (array.Length != 0)
		{
			foreach (string text3 in array)
			{
				text2 += text3.ToTitleCase(false);
				if (text3 != text && text3 != Environment.NewLine && text3.Length > 2 && text3 != array[array.Length - 1])
				{
					text2 += text;
				}
			}
		}
		return text2.TrimEnd(Environment.NewLine.ToCharArray());
	}

	public void EditBackgroundText()
	{
		Util.Run(GameIO.pathCurrentSave + "background.txt");
	}

	public void CreateEquip()
	{
		Chara pc = EClass.pc;
		pc.body.AddBodyPart(45, null);
		if (EClass.debug.enable)
		{
			EClass.pc.EQ_ID("lantern_old", -1, Rarity.Random);
		}
		pc.body.AddBodyPart(44, null);
		pc.EQ_ID("toolbelt", -1, Rarity.Random).c_IDTState = 0;
		pc.EQ_ID("shirt", -1, Rarity.Random).c_IDTState = 0;
		pc.AddCard(ThingGen.CreateCurrency(1 + EClass.rnd(5), "money"));
		string id = EClass.pc.job.id;
		if (!(id == "paladin"))
		{
			if (!(id == "inquisitor"))
			{
				if (!(id == "witch"))
				{
					if (id == "swordsage")
					{
						pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(50300, 1), 4));
						pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(50301, 1), 4));
						pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(50802, 1), 2));
						pc.AddCard(ThingGen.Create("tool_talisman", -1, -1));
					}
				}
				else
				{
					pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(8790, 1), 4));
				}
			}
			else
			{
				pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(8706, 1), 4));
			}
		}
		else
		{
			pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(8400, 1), 2));
		}
		id = EClass.pc.job.id;
		if (!(id == "wizard") && !(id == "warmage") && !(id == "priest") && !(id == "witch"))
		{
			pc.AddCard(ThingGen.Create("bandage", -1, -1).SetNum(6 + EClass.rnd(3)));
			return;
		}
		int num = 0;
		foreach (int id2 in EClass.player.domains)
		{
			Element element = Element.Create(id2, 0);
			string text = "";
			foreach (string text2 in element.source.tag)
			{
				if (text != "")
				{
					break;
				}
				if (text2 == "hand" || text2 == "arrow" || text2 == "bolt")
				{
					text = text2 + "_";
				}
			}
			if (text != "")
			{
				pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(text + element.source.alias.Replace("ele", ""), 1), 4));
				num++;
				if (num >= 2)
				{
					break;
				}
			}
		}
		if (EClass.pc.job.id == "priest")
		{
			pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(8400, 1), 2));
			return;
		}
		pc.AddCard(Player.<CreateEquip>g__SetSpellbook|178_0(ThingGen.CreateSpellbook(8200, 1), 2));
	}

	public void DreamSpell()
	{
		int num = EClass.pc.Evalue(1653);
		if (num == 0 && EClass.pc.Evalue(1402) == 0 && EClass.rnd(4) != 0)
		{
			return;
		}
		List<SourceElement.Row> list = new List<SourceElement.Row>();
		foreach (int id in EClass.player.domains)
		{
			Element element = Element.Create(id, 0);
			string a2 = "";
			foreach (string text in element.source.tag)
			{
				if (a2 != "")
				{
					break;
				}
				bool flag = false;
				if (text == "hand" || text == "arrow" || text == "bolt")
				{
					flag = true;
				}
				if (num >= 3 && (text == "ball" || text == "weapon" || text == "funnel" || text == "miasma"))
				{
					flag = true;
				}
				if (flag)
				{
					list.Add(EClass.sources.elements.alias[text + "_" + element.source.alias.Replace("ele", "")]);
				}
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		SourceElement.Row row = list.RandomItemWeighted((SourceElement.Row a) => (float)a.chance);
		Element element2 = EClass.pc.elements.GetElement(row.id);
		int mtp = (num == 0) ? 100 : (75 + num * 25);
		if (num > 0 || element2 == null || element2.vPotential == 0)
		{
			Msg.Say("dream_spell", EClass.sources.elements.alias[row.aliasRef].GetText("altname", false).Split(',', StringSplitOptions.None)[0].ToLower(), null, null, null);
			EClass.pc.GainAbility(row.id, mtp);
		}
	}

	public void SimulateFaction()
	{
		this.simulatingZone = true;
		Zone currentZone = EClass.pc.currentZone;
		Point point = EClass.pc.pos.Copy();
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			if (factionBranch.owner != currentZone)
			{
				EClass.pc.MoveZone(factionBranch.owner, ZoneTransition.EnterState.Auto);
				this.zone = factionBranch.owner;
				EClass.scene.Init(Scene.Mode.Zone);
			}
		}
		EClass.pc.MoveZone(currentZone, new ZoneTransition
		{
			state = ZoneTransition.EnterState.Exact,
			x = point.x,
			z = point.z
		});
		this.zone = currentZone;
		EClass.scene.Init(Scene.Mode.Zone);
		this.simulatingZone = false;
	}

	public void ExitBorder(ActPlan p = null)
	{
		bool flag = EClass.game.quests.HasFarAwayEscort(false);
		string textDialog = (flag ? "ExitZoneEscort" : "ExitZone").lang(EClass._zone.Name, null, null, null, null);
		bool flag2 = EClass.pc.pos.x == EClass._map.bounds.x || EClass.pc.pos.x == EClass._map.bounds.x + EClass._map.bounds.Width - 1 || EClass.pc.pos.z == EClass._map.bounds.z || EClass.pc.pos.z == EClass._map.bounds.z + EClass._map.bounds.Height - 1;
		if (flag || EClass.core.config.game.confirmMapExit || (EClass._zone.instance != null && EClass._zone.instance.WarnExit))
		{
			if (p == null)
			{
				Dialog.YesNo(textDialog, delegate
				{
					EClass.game.quests.HasFarAwayEscort(true);
					EClass.pc.MoveZone(EClass._zone.ParentZone, ZoneTransition.EnterState.Auto);
				}, delegate
				{
				}, "yes", "no");
				return;
			}
			p.TrySetAct("actNewZone", delegate()
			{
				Dialog.YesNo(textDialog, delegate
				{
					EClass.game.quests.HasFarAwayEscort(true);
					EClass.pc.MoveZone(EClass._zone.ParentZone, ZoneTransition.EnterState.Auto);
				}, null, "yes", "no");
				return false;
			}, CursorSystem.MoveZone, flag2 ? 999 : 1);
			return;
		}
		else
		{
			if (p == null)
			{
				EClass.pc.MoveZone(EClass._zone.ParentZone, ZoneTransition.EnterState.Auto);
				return;
			}
			p.TrySetAct("actNewZone", delegate()
			{
				EClass.pc.MoveZone(EClass._zone.ParentZone, ZoneTransition.EnterState.Auto);
				return false;
			}, CursorSystem.MoveZone, flag2 ? 999 : 1);
			return;
		}
	}

	public bool CanExitBorder(Point p)
	{
		return (EClass._zone != EClass.game.StartZone || EClass.pc.homeBranch != null) && !EClass._zone.BlockBorderExit && (EClass.pc.held == null || !EClass.pc.held.trait.CanOnlyCarry);
	}

	public void OnExitBorder(Point p)
	{
		if (EClass._zone.ParentZone.IsRegion)
		{
			int num = 0;
			int num2 = 0;
			MapBounds bounds = EClass._map.bounds;
			if (p.x < bounds.x)
			{
				num = -1;
			}
			else if (p.x > bounds.maxX)
			{
				num = 1;
			}
			else if (p.z < bounds.z)
			{
				num2 = -1;
			}
			else
			{
				num2 = 1;
			}
			ZoneTransition.EnterState state = (num == 1) ? ZoneTransition.EnterState.Left : ((num == -1) ? ZoneTransition.EnterState.Right : ((num2 == 1) ? ZoneTransition.EnterState.Bottom : ZoneTransition.EnterState.Top));
			float ratePos = (num == 1 || num == -1) ? ((float)(p.z - bounds.z) / (float)bounds.Height) : ((float)(p.x - bounds.x) / (float)bounds.Width);
			ZoneTransition trans = new ZoneTransition
			{
				state = state,
				ratePos = ratePos
			};
			Point p2 = new Point(EClass._zone.x + num, EClass._zone.y + num2);
			this.EnterLocalZone(p2, trans, false, null);
			return;
		}
		EClass.pc.MoveZone(EClass._zone.ParentZone, ZoneTransition.EnterState.Auto);
	}

	public void EnterLocalZone(bool encounter = false, Chara mob = null)
	{
		this.EnterLocalZone(EClass.pc.pos.Copy(), null, encounter, mob);
	}

	public void EnterLocalZone(Point p, ZoneTransition trans = null, bool encounter = false, Chara mob = null)
	{
		if (EClass._zone.IsRegion)
		{
			p.Set(p.x + EClass.scene.elomap.minX, p.z + EClass.scene.elomap.minY);
		}
		Zone zone = EClass._zone.Region.GetZoneAt(p.x, p.z);
		int enemies = 1;
		int roadDist = EClass.scene.elomap.GetRoadDist(p.x, p.z);
		int num = (EClass.pc.homeBranch == null) ? 0 : EClass.pc.pos.Distance(EClass.game.StartZone.mapX, EClass.game.StartZone.mapY);
		if (zone == null)
		{
			if (!EClass._zone.Region.CanCreateZone(p))
			{
				Msg.Say("zoneBlocked");
				return;
			}
			int num2 = Mathf.Clamp(roadDist - 4, 0, 200) + Mathf.Clamp(num / 4, 0, 10);
			if (roadDist > 20)
			{
				num2 += roadDist - 20;
			}
			if (num2 >= 100)
			{
				num2 = 100;
			}
			Debug.Log(string.Concat(new string[]
			{
				"encounter roadiDist:",
				roadDist.ToString(),
				" homeDist:",
				num.ToString(),
				" lv:",
				num2.ToString()
			}));
			zone = EClass._zone.Region.CreateZone(p);
			zone._dangerLv = num2;
			enemies = 2 + Mathf.Min(num2 / 10, 4) + EClass.rnd(3 + Mathf.Min(num2 / 10, 4));
			if (num < 5)
			{
				enemies = 2;
			}
		}
		else
		{
			if (zone is Zone_Void && EClass.player.CountKeyItem("license_void") == 0)
			{
				Msg.Say("voidClosed");
				return;
			}
			if (zone.IsClosed)
			{
				Msg.Say("zoneClosed");
				return;
			}
			if (zone is Zone_Casino)
			{
				Msg.Say("zoneNoEntrance");
				return;
			}
		}
		if (trans == null)
		{
			ZoneTransition.EnterState state = encounter ? ZoneTransition.EnterState.Encounter : ((zone.RegionEnterState == ZoneTransition.EnterState.Dir) ? ZoneTransition.DirToState(EClass.pc.GetCurrentDir()) : zone.RegionEnterState);
			trans = new ZoneTransition
			{
				state = state
			};
		}
		EClass.pc.MoveZone(zone, trans);
		if (encounter)
		{
			zone.events.AddPreEnter(new ZonePreEnterEncounter
			{
				enemies = enemies,
				roadDist = roadDist,
				mob = mob
			}, true);
		}
	}

	public void MoveZone(Zone z)
	{
		CursorSystem.ignoreCount = 15;
		EClass.ui.hud.transRight.SetActive(false);
		this.zone = z;
		if (this.zone.IsRegion)
		{
			this.dateTravel = EClass.world.date.GetRaw(0);
		}
		EClass.scene.Init(Scene.Mode.Zone);
		if (WidgetRoster.Instance)
		{
			WidgetRoster.Instance.OnMoveZone();
		}
		if (UIResourceTrack.Instance)
		{
			UIResourceTrack.Instance.OnMoveZone();
		}
		if (WidgetMinimap.Instance)
		{
			WidgetMinimap.Instance.OnMoveZone();
		}
		if (EClass.pc.currentZone == this.zone && !this.zone.IsRegion && this.LastTravelZone != this.zone)
		{
			if (this.LastTravelZone != null)
			{
				int num = EClass.world.date.GetElapsedHour(this.dateTravel);
				int num2 = num / 24;
				if (this.distanceTravel > 2)
				{
					num %= 24;
					Msg.Say("travel", this.LastTravelZone.Name, Date.GetText(this.dateTravel, Date.TextFormat.Travel), num2.ToString() ?? "", num.ToString() ?? "");
					Msg.Say("travel2", this.distanceTravel.ToString() ?? "", ((EClass.pc.party.members.Count == 1) ? "you" : "youAndCompanion").lang(), null, null);
					foreach (Chara chara in EClass.pc.party.members)
					{
						chara.AddExp(this.distanceTravel / 3);
						chara.elements.ModExp(240, 30 + this.distanceTravel * 5, false);
					}
				}
			}
			this.distanceTravel = 0;
			this.dateTravel = EClass.world.date.GetRaw(0);
			this.uidLastTravelZone = this.zone.uid;
		}
		this.regionMoveWarned = false;
	}

	public void AddInventory(Card c)
	{
		EClass.pc.AddCard(c);
	}

	public void EndTurn(bool consume = true)
	{
		if (EClass.pc.isDead)
		{
			return;
		}
		if (consume)
		{
			EInput.Consume(false, 1);
		}
		EClass.pc.SetAI(new GoalEndTurn());
		EClass.player.baseActTime = EClass.setting.defaultActPace;
	}

	public void ModFame(int a)
	{
		if (a == 0)
		{
			return;
		}
		if (a >= 0 && this.fame + a >= 5000 && EClass.player.CountKeyItem("license_adv") == 0)
		{
			a = 5000 - this.fame;
			if (a <= 0)
			{
				a = 0;
				Msg.Say("gainFameLimit");
				return;
			}
		}
		this.fame += a;
		if (this.fame < 0)
		{
			this.fame = 0;
		}
		if (a > 0)
		{
			Msg.Say("gainFame", a.ToString() ?? "", null, null, null);
		}
		else
		{
			Msg.Say("looseFame", (-a).ToString() ?? "", null, null, null);
		}
		if (a > 0)
		{
			Tutorial.Reserve("fame", null);
		}
	}

	public void ModKeyItem(string alias, int num = 1, bool msg = true)
	{
		this.ModKeyItem(EClass.sources.keyItems.alias[alias].id, num, msg);
	}

	public void ModKeyItem(int id, int num = 1, bool msg = true)
	{
		if (!this.keyItems.ContainsKey(id))
		{
			this.keyItems.Add(id, 0);
		}
		Dictionary<int, int> dictionary = this.keyItems;
		dictionary[id] += num;
		if (msg)
		{
			if (num > 0)
			{
				SE.Play("keyitem");
				Msg.Say("get_keyItem", EClass.sources.keyItems.map[id].GetName(), null, null, null);
				return;
			}
			SE.Play("keyitem_lose");
			Msg.Say("lose_keyItem", EClass.sources.keyItems.map[id].GetName(), null, null, null);
		}
	}

	public int CountKeyItem(string alias)
	{
		return this.CountKeyItem(EClass.sources.keyItems.alias[alias].id);
	}

	public int CountKeyItem(int id)
	{
		if (!this.keyItems.ContainsKey(id))
		{
			return 0;
		}
		return this.keyItems[id];
	}

	public void EquipTool(Thing a, bool setHotItem = true)
	{
		if (a.GetRootCard() != EClass.pc)
		{
			if (a.parent is Thing)
			{
				Msg.Say("movedToEquip", a, a.parent as Thing, null, null);
			}
			a = EClass.pc.AddThing(a, true, -1, -1);
		}
		if (setHotItem)
		{
			EClass.player.SetCurrentHotItem(a.trait.GetHotItem());
			SE.SelectHotitem();
		}
	}

	public void RefreshCurrentHotItem()
	{
		WidgetCurrentTool instance = WidgetCurrentTool.Instance;
		if (this.currentHotItem != null)
		{
			if (instance)
			{
				instance.buttonHotItem.Refresh();
			}
			if (this.currentHotItem is HotItemHeld && this.currentHotItem.Thing != EClass.pc.held)
			{
				this.currentHotItem = null;
			}
			else if (this.currentHotItem is HotItemThing && (this.currentHotItem.Thing == null || this.currentHotItem.Thing.GetRootCard() != EClass.pc))
			{
				this.currentHotItem = null;
			}
		}
		if (EClass.pc.held != null)
		{
			this.currentHotItem = new HotItemHeld(EClass.pc.held as Thing);
		}
		if (this.currentHotItem == null)
		{
			if (instance && instance.selected != -1 && instance.selectedButton.card != null && instance.selectedButton.card.GetRootCard() == EClass.pc)
			{
				this.currentHotItem = instance.selectedButton.card.trait.GetHotItem();
			}
			else
			{
				this.currentHotItem = this.hotItemNoItem;
			}
		}
		if (this.currentHotItem != this.lastHotItem)
		{
			if (this.lastHotItem != null)
			{
				this.lastHotItem.OnUnsetCurrentItem();
			}
			this.currentHotItem.OnSetCurrentItem();
			this.lastHotItem = this.currentHotItem;
		}
		WidgetCurrentTool.RefreshCurrentHotItem();
		WidgetHotbar.dirtyCurrentItem = false;
		this.MarkMapHighlights();
		EClass.core.actionsNextFrame.Add(delegate
		{
			EClass.core.actionsNextFrame.Add(delegate
			{
				foreach (LayerInventory layerInventory in LayerInventory.listInv)
				{
					layerInventory.invs[0].RefreshHighlight();
				}
			});
		});
		EClass.pc.RecalculateFOV();
	}

	public void ResetCurrentHotItem()
	{
		EClass.pc.PickHeld(false);
		EClass.player.SetCurrentHotItem(null);
		SE.SelectHotitem();
	}

	public void SetCurrentHotItem(HotItem item)
	{
		EClass.pc.PickHeld(false);
		if (this.currentHotItem != item)
		{
			this.currentHotItem = item;
			HotItemHeld hotItemHeld = this.currentHotItem as HotItemHeld;
			if (hotItemHeld != null)
			{
				EClass.pc.HoldCard(hotItemHeld.Thing, -1);
			}
		}
		this.RefreshCurrentHotItem();
	}

	public void TryEquipBait()
	{
		if (this.eqBait != null && this.eqBait.GetRootCard() != EClass.pc)
		{
			this.eqBait = null;
		}
		if (this.eqBait == null)
		{
			Thing thing = EClass.pc.things.Find<TraitBait>();
			if (thing != null)
			{
				thing.trait.OnUse(EClass.pc);
			}
		}
	}

	public bool HasValidRangedTarget()
	{
		return this.target != null && this.target.IsAliveInCurrentZone && this.target.isSynced && EClass.pc.CanSee(this.target);
	}

	public bool TargetRanged()
	{
		Thing thing = EClass.player.currentHotItem.Thing;
		if (thing != null)
		{
			Trait trait = thing.trait;
		}
		int num = 999999999;
		Chara chara = null;
		Point pos = EClass.scene.mouseTarget.pos;
		List<Chara> list = new List<Chara>();
		bool flag = false;
		if (EInput.isShiftDown && pos.IsValid && pos.HasChara)
		{
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2.isSynced && chara2.IsAliveInCurrentZone && !chara2.IsPC && chara2.pos.Equals(pos) && EClass.pc.CanSeeLos(chara2.pos, -1))
				{
					list.Add(chara2);
				}
			}
			if (list.Count > 0)
			{
				chara = list.NextItem(EClass.player.target);
				flag = true;
			}
			list.Clear();
		}
		if (!flag)
		{
			foreach (Chara chara3 in EClass._map.charas)
			{
				int num2 = 0;
				if (chara3.isSynced && chara3.IsAliveInCurrentZone && !chara3.IsPC && EClass.pc.CanSeeLos(chara3.pos, -1) && EClass.pc.CanSee(chara3) && !chara3.IsFriendOrAbove())
				{
					if (chara3.IsNeutralOrAbove())
					{
						num2 += 10000;
					}
					num2 += EClass.pc.Dist(chara3);
					if (num2 < num)
					{
						chara = chara3;
						num = num2;
					}
					list.Add(chara3);
				}
			}
			if (EInput.isShiftDown)
			{
				chara = list.NextItem(EClass.player.target);
			}
		}
		if (chara == null)
		{
			if (this.target != null)
			{
				Msg.Say("noTargetFound");
			}
			this.target = null;
			return false;
		}
		if (this.target != chara)
		{
			Msg.Say("targetSet", chara, null, null, null);
		}
		this.target = chara;
		return true;
	}

	public void OnAdvanceHour()
	{
		EClass.pc.faith.OnChangeHour();
		if (EClass.pc.Evalue(289) > 0)
		{
			foreach (Thing t2 in EClass.pc.things.List((Thing t) => t.c_IDTState == 5, true))
			{
				EClass.pc.TryIdentify(t2, 1, true);
			}
		}
		if (!EClass.pc.IsDeadOrSleeping && EClass.rnd(2) == 0 && !EClass._zone.IsRegion)
		{
			foreach (Chara chara in EClass.pc.party.members)
			{
				chara.AddExp(1);
			}
		}
	}

	public void OnAdvanceDay()
	{
		this.nums.OnAdvanceDay();
		Chara pc = EClass.pc;
		int c_daysWithGod = pc.c_daysWithGod;
		pc.c_daysWithGod = c_daysWithGod + 1;
		if (EClass.pc.Evalue(85) > 0)
		{
			EClass.pc.ModExp(85, 10);
		}
		EClass.pc.RefreshFaithElement();
		this.prayed = false;
		if (this.karma < 0 && EClass.rnd(4) == 0)
		{
			this.ModKarma(1);
		}
	}

	public bool TryAbortAutoCombat()
	{
		if (!(EClass.pc.ai is GoalAutoCombat))
		{
			return false;
		}
		EClass.pc.ai.Cancel();
		EClass.pc.SetNoGoal();
		return true;
	}

	public void ClearMapHighlights()
	{
		foreach (Point point in this.lastMarkedHighlights)
		{
			point.cell.highlight = 0;
		}
		this.lastMarkedHighlights.Clear();
	}

	public void MarkMapHighlights()
	{
		this.ClearMapHighlights();
		this.currentHotItem.OnMarkMapHighlights();
	}

	public bool CanAcceptInput()
	{
		return EClass.pc.HasNoGoal && !EClass.pc.WillConsumeTurn();
	}

	public bool CanSee(Chara c)
	{
		if (EClass.pc.hasTelepathy && c.race.visibleWithTelepathy)
		{
			return true;
		}
		if (!c.IsPC && (EClass.pc.fov == null || EClass.pc.isBlind))
		{
			return false;
		}
		if (c.IsPCParty || c.isDead)
		{
			return true;
		}
		if (c.IsMultisize)
		{
			bool canSee = false;
			int dist = EClass.pc.Dist(c);
			c.ForeachPoint(delegate(Point p, bool main)
			{
				if (!canSee && (p.cell.light > 0 || dist < 2) && p.cell.pcSync)
				{
					canSee = true;
				}
			});
			return canSee;
		}
		return (c.pos.cell.light > 0 || EClass.pc.Dist(c) < 2) && c.pos.cell.pcSync;
	}

	public void AddExpKnowledge(int a)
	{
		this.expKnowledge += a;
		if (this.expKnowledge >= this.MaxExpKnowledge)
		{
			for (int i = 0; i < this.expKnowledge / this.MaxExpKnowledge; i++)
			{
				Player.<AddExpKnowledge>g__LvUp|208_0();
			}
			this.expKnowledge %= this.MaxExpKnowledge;
		}
	}

	public void AddExpInfluence(int a)
	{
		this.expInfluence += a;
		if (this.expInfluence >= this.MaxExpInfluence)
		{
			for (int i = 0; i < this.expInfluence / this.MaxExpInfluence; i++)
			{
				Player.<AddExpInfluence>g__LvUp|209_0();
			}
			this.expInfluence %= this.MaxExpInfluence;
		}
	}

	public void ModKarma(int a)
	{
		if (a == 0)
		{
			return;
		}
		if (a < 0)
		{
			Tutorial.Reserve("karma", null);
		}
		bool flag = this.karma < 0;
		this.karma += a;
		Msg.Say((a > 0) ? "karmaUp" : "karmaDown", a.ToString() ?? "", null, null, null);
		if (this.karma < 0 && !flag)
		{
			Msg.Say("becomeCriminal");
			EClass.pc.pos.TryWitnessCrime(EClass.pc, null, 4, null);
			EClass._zone.RefreshCriminal();
			Tutorial.Reserve("criminal", null);
		}
		if (this.karma >= 0 && flag)
		{
			Msg.Say("becomeNonCriminal");
			EClass._zone.RefreshCriminal();
		}
		EClass.game.quests.list.ForeachReverse(delegate(Quest q)
		{
			q.OnModKarma(a);
		});
		this.karma = Mathf.Clamp(this.karma, -100, 100);
	}

	public Thing DropReward(Thing t, bool silent = false)
	{
		t.things.DestroyAll(null);
		EClass._zone.AddCard(t, EClass.pc.pos);
		if (!silent)
		{
			Msg.Say("dropReward");
		}
		return t;
	}

	public bool TooHeavyToMove()
	{
		if (!EClass.debug.ignoreWeight && EClass.pc.burden.GetPhase() == 4)
		{
			Msg.Say("tooHeavyToMove");
			EClass.pc.renderer.NextFrame();
			EInput.Consume(true, 1);
			return true;
		}
		return false;
	}

	[CompilerGenerated]
	internal static Thing <CreateEquip>g__SetSpellbook|178_0(Thing t, int charge)
	{
		t.AddEditorTag(EditorTag.NoReadFail);
		t.c_charges = charge;
		t.SetBlessedState(BlessedState.Normal);
		return t;
	}

	[CompilerGenerated]
	internal static void <AddExpKnowledge>g__LvUp|208_0()
	{
		Msg.Say("DingKnowledge");
	}

	[CompilerGenerated]
	internal static void <AddExpInfluence>g__LvUp|209_0()
	{
		Msg.Say("DingInfluence");
	}

	[JsonProperty]
	public Player.ReturnInfo returnInfo;

	[JsonProperty]
	public int uidChara;

	[JsonProperty]
	public int uidSpawnZone;

	[JsonProperty]
	public int tutorialStep;

	[JsonProperty]
	public int karma;

	[JsonProperty]
	public int fame;

	[JsonProperty]
	public int expKnowledge;

	[JsonProperty]
	public int expInfluence;

	[JsonProperty]
	public int dateTravel;

	[JsonProperty]
	public int distanceTravel;

	[JsonProperty]
	public int uidLastTravelZone;

	[JsonProperty]
	public int uidLastZone;

	[JsonProperty]
	public int uidLastShippedZone;

	[JsonProperty]
	public int totalFeat;

	[JsonProperty]
	public int taxBills;

	[JsonProperty]
	public int unpaidBill;

	[JsonProperty]
	public int bankMoney;

	[JsonProperty]
	public int holyWell;

	[JsonProperty]
	public int safeTravel;

	[JsonProperty]
	public int hotbarPage;

	[JsonProperty]
	public int little_dead;

	[JsonProperty]
	public int little_saved;

	[JsonProperty]
	public int extraTax;

	[JsonProperty]
	public int lastFelmeraReward;

	[JsonProperty]
	public int uidLastTown;

	[JsonProperty]
	public int seedShrine;

	[JsonProperty]
	public int seedChest;

	[JsonProperty]
	public int debt;

	[JsonProperty]
	public float angle;

	[JsonProperty]
	public bool hasNewQuest;

	[JsonProperty]
	public bool dailyGacha;

	[JsonProperty]
	public bool useSubWidgetTheme;

	[JsonProperty]
	public bool wellWished;

	[JsonProperty]
	public bool prayed;

	[JsonProperty]
	public bool questTracker;

	[JsonProperty]
	public bool showShippingResult;

	[JsonProperty]
	public bool isEditor;

	[JsonProperty]
	public bool openContainerCenter;

	[JsonProperty]
	public string title;

	[JsonProperty]
	public string memo = "";

	[JsonProperty]
	public Player.Pref pref = new Player.Pref();

	[JsonProperty]
	public Player.Stats stats = new Player.Stats();

	[JsonProperty]
	public NumLogManager nums = new NumLogManager();

	[JsonProperty]
	public KnowledgeManager knowledges = new KnowledgeManager();

	[JsonProperty]
	public PopupManager popups = new PopupManager();

	[JsonProperty]
	public WidgetManager.SaveData mainWidgets;

	[JsonProperty]
	public WidgetManager.SaveData subWidgets;

	[JsonProperty]
	public Window.SaveData dataPick = new Window.SaveData();

	[JsonProperty]
	public LayerAbility.Config layerAbilityConfig = new LayerAbility.Config();

	[JsonProperty]
	public Dictionary<string, int> dialogFlags = new Dictionary<string, int>();

	[JsonProperty]
	public Dictionary<string, string> hangIcons = new Dictionary<string, string>();

	[JsonProperty]
	public Dictionary<string, KnownSong> knownSongs = new Dictionary<string, KnownSong>();

	[JsonProperty]
	public Dictionary<string, string> lastRecipes = new Dictionary<string, string>();

	[JsonProperty]
	public Dictionary<string, List<string>> priorityActions = new Dictionary<string, List<string>>();

	[JsonProperty]
	public HashSet<string> trackedCategories = new HashSet<string>();

	[JsonProperty]
	public HashSet<string> trackedCards = new HashSet<string>();

	[JsonProperty]
	public HashSet<int> trackedElements = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> knownBGMs = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> favAbility = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> sketches = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> doneBackers = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> knownCraft = new HashSet<int>();

	[JsonProperty]
	public List<int> domains = new List<int>();

	[JsonProperty]
	public Zone zone;

	[JsonProperty]
	public HotbarManager hotbars = new HotbarManager();

	[JsonProperty]
	public Chara _agent;

	[JsonProperty]
	public Hoard hoard = new Hoard();

	[JsonProperty]
	public Player.Flags flags = new Player.Flags();

	[JsonProperty]
	public RecipeManager recipes = new RecipeManager();

	[JsonProperty]
	public HotItem currentHotItem = new HotItemNoItem();

	[JsonProperty]
	public Point lastZonePos;

	[JsonProperty]
	public Thing eqBait;

	[JsonProperty]
	public Dictionary<string, Window.SaveData> dataWindow;

	[JsonProperty]
	public CinemaConfig cinemaConfig = new CinemaConfig();

	[JsonProperty]
	public CodexManager codex = new CodexManager();

	[JsonProperty]
	public Dictionary<int, int> keyItems = new Dictionary<int, int>();

	[JsonProperty]
	public List<int> uidPickOnLoad = new List<int>();

	[JsonProperty]
	public List<ShippingResult> shippingResults = new List<ShippingResult>();

	[JsonProperty]
	public Dictionary<string, HashSet<string>> noRestocks = new Dictionary<string, HashSet<string>>();

	[JsonProperty]
	public Window.SaveData windowAllyInv;

	public static int seedHallucination;

	public static int realHour;

	public Window.SaveData windowDataCopy;

	public string windowDataName;

	public ZoneTransition lastTransition;

	public List<Point> lastMarkedHighlights = new List<Point>();

	public HotItem lastHotItem;

	public HotItem hotItemToRestore;

	public bool forceTalk;

	public bool altHeldPos;

	public bool instaComplete = true;

	public bool regionMoveWarned;

	public bool waitingInput;

	public bool willEndTurn;

	public bool wasDirtyWeight;

	public bool deathDialog;

	public bool preventDeathPenalty;

	public bool deathZoneMove;

	public bool haltMove;

	public bool invlunerable;

	public bool willAutoSave;

	public bool simulatingZone;

	public bool isAutoFarming;

	public bool enemySpotted;

	public string deathMsg;

	public int countNewline;

	public int lightRadius;

	public int lastTurn;

	public int lastEmptyAlly;

	public float lightPower;

	public float baseActTime;

	public float pickupDelay = 2f;

	public Chara chara;

	public Chara focusedchara;

	public NoticeManager notices = new NoticeManager();

	public QueueManager queues = new QueueManager();

	public Act lastAct;

	public Vector2 nextMove;

	public Vector3 position;

	public Action onStartZone;

	public PlayingSong playingSong;

	public List<Chara> listSummon = new List<Chara>();

	public int tempFame;

	public int autoCombatStartHP;

	public Zone nextZone;

	public Thing renderThing;

	public HotItemNoItem hotItemNoItem = new HotItemNoItem();

	public Chara target;

	public class ReturnInfo
	{
		public int turns;

		public int uidDest;

		public bool isEvac;

		public bool askDest;
	}

	public class Pref
	{
		[JsonIgnore]
		public string lastBuildCategory;

		[JsonIgnore]
		public string lastBuildRecipe;

		public int lastIdTabAbility;

		public bool layerInventory;

		public bool layerAbility;

		public bool layerCraft;

		public bool sort_ascending;

		public bool sort_ascending_shop;

		public UIList.SortMode sortResources;

		public UIList.SortMode sortPeople;

		public UIList.SortMode sortResearch = UIList.SortMode.ByCategory;

		public UIList.SortMode sortInv = UIList.SortMode.ByCategory;

		public UIList.SortMode sortInvShop = UIList.SortMode.ByValue;

		public UIList.SortMode sortAbility = UIList.SortMode.ByID;

		public LayerPeople.ShowMode modePoeple;

		public UIInventory.InteractMode interactMode;
	}

	public class Stats
	{
		public int GetShippingBonus(int _a)
		{
			Player.Stats.<>c__DisplayClass21_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.p = 0;
			CS$<>8__locals1.a = _a;
			CS$<>8__locals1.first = true;
			this.<GetShippingBonus>g__SetBonus|21_0(500000, 5000, ref CS$<>8__locals1);
			this.<GetShippingBonus>g__SetBonus|21_0(250000, 3000, ref CS$<>8__locals1);
			this.<GetShippingBonus>g__SetBonus|21_0(100000, 2000, ref CS$<>8__locals1);
			this.<GetShippingBonus>g__SetBonus|21_0(20000, 1000, ref CS$<>8__locals1);
			this.<GetShippingBonus>g__SetBonus|21_0(6000, 500, ref CS$<>8__locals1);
			this.<GetShippingBonus>g__SetBonus|21_0(2000, 200, ref CS$<>8__locals1);
			this.<GetShippingBonus>g__SetBonus|21_0(0, 100, ref CS$<>8__locals1);
			return CS$<>8__locals1.p;
		}

		[CompilerGenerated]
		private void <GetShippingBonus>g__SetBonus|21_0(int threshold, int div, ref Player.Stats.<>c__DisplayClass21_0 A_3)
		{
			if (A_3.a >= threshold)
			{
				A_3.p += (A_3.a - threshold) / div;
				if (A_3.first)
				{
					this.lastShippingExp = (A_3.a - threshold) % div;
					this.lastShippingExpMax = div;
					A_3.first = false;
				}
				A_3.a = threshold;
			}
		}

		public double timeElapsed;

		public int kumi;

		public int mins;

		public int days;

		public int months;

		public int sieges;

		public int turns;

		public int kills;

		public int taxBills;

		public int taxBillsPaid;

		public int digs;

		public int shipNum;

		public int shipMoney;

		public int slept;

		public int death;

		public int allyDeath;

		public int deepest;

		public int gambleChest;

		public int gambleChestOpen;

		[JsonIgnore]
		public int lastShippingExp;

		[JsonIgnore]
		public int lastShippingExpMax;
	}

	public class Flags : EClass
	{
		public bool gotClickReward
		{
			get
			{
				return this.bits[1];
			}
			set
			{
				this.bits[1] = value;
			}
		}

		public bool welcome
		{
			get
			{
				return this.bits[2];
			}
			set
			{
				this.bits[2] = value;
			}
		}

		public bool fiamaStoryBookGiven
		{
			get
			{
				return this.bits[3];
			}
			set
			{
				this.bits[3] = value;
			}
		}

		public bool fiamaFirstDream
		{
			get
			{
				return this.bits[4];
			}
			set
			{
				this.bits[4] = value;
			}
		}

		public bool helpHighlightDisabled
		{
			get
			{
				return this.bits[5];
			}
			set
			{
				this.bits[5] = value;
			}
		}

		public bool pickedMelilithTreasure
		{
			get
			{
				return this.bits[6];
			}
			set
			{
				this.bits[6] = value;
			}
		}

		public bool isShoesOff
		{
			get
			{
				return this.bits[7];
			}
			set
			{
				this.bits[7] = value;
			}
		}

		public bool backpackHighlightDisabled
		{
			get
			{
				return this.bits[8];
			}
			set
			{
				this.bits[8] = value;
			}
		}

		public bool abilityHighlightDisabled
		{
			get
			{
				return this.bits[9];
			}
			set
			{
				this.bits[9] = value;
			}
		}

		public bool elinGift
		{
			get
			{
				return this.bits[10];
			}
			set
			{
				this.bits[10] = value;
			}
		}

		public bool gotMelilithCurse
		{
			get
			{
				return this.bits[11];
			}
			set
			{
				this.bits[11] = value;
			}
		}

		public bool canComupWithFoodRecipe
		{
			get
			{
				return this.bits[12];
			}
			set
			{
				this.bits[12] = value;
			}
		}

		public bool KilledBossInVoid
		{
			get
			{
				return this.bits[13];
			}
			set
			{
				this.bits[13] = value;
			}
		}

		public bool statueShipped
		{
			get
			{
				return this.bits[14];
			}
			set
			{
				this.bits[14] = value;
			}
		}

		public bool little_saved
		{
			get
			{
				return this.bits[15];
			}
			set
			{
				this.bits[15] = value;
			}
		}

		public bool little_killed
		{
			get
			{
				return this.bits[16];
			}
			set
			{
				this.bits[16] = value;
			}
		}

		public bool gotEtherDisease
		{
			get
			{
				return this.bits[17];
			}
			set
			{
				this.bits[17] = value;
			}
		}

		public bool loytelEscaped
		{
			get
			{
				return this.bits[18];
			}
			set
			{
				this.bits[18] = value;
			}
		}

		public bool toggleHotbarHighlightDisabled
		{
			get
			{
				return this.bits[19];
			}
			set
			{
				this.bits[19] = value;
			}
		}

		public bool debugEnabled
		{
			get
			{
				return this.bits[20];
			}
			set
			{
				this.bits[20] = value;
			}
		}

		public bool magicChestSent
		{
			get
			{
				return this.bits[21];
			}
			set
			{
				this.bits[21] = value;
			}
		}

		public int start
		{
			get
			{
				return this.ints[10];
			}
			set
			{
				this.ints[10] = value;
			}
		}

		public int build
		{
			get
			{
				return this.ints[11];
			}
			set
			{
				this.ints[11] = value;
			}
		}

		public int main
		{
			get
			{
				return this.ints[13];
			}
			set
			{
				this.ints[13] = value;
			}
		}

		public int storyFiama
		{
			get
			{
				return this.ints[14];
			}
			set
			{
				this.ints[14] = value;
			}
		}

		public int lutz
		{
			get
			{
				return this.ints[15];
			}
			set
			{
				this.ints[15] = value;
			}
		}

		public int daysAfterQuestExploration
		{
			get
			{
				return this.ints[16];
			}
			set
			{
				this.ints[16] = value;
			}
		}

		public int landDeedBought
		{
			get
			{
				return this.ints[17];
			}
			set
			{
				this.ints[17] = value;
			}
		}

		public int loytelMartLv
		{
			get
			{
				return this.ints[18];
			}
			set
			{
				this.ints[18] = value;
			}
		}

		[OnSerializing]
		private void _OnSerializing(StreamingContext context)
		{
			this.ints[0] = (int)this.bits.Bits;
		}

		[OnDeserialized]
		private void _OnDeserialized(StreamingContext context)
		{
			this.bits.Bits = (uint)this.ints[0];
		}

		public void OnCreateGame()
		{
		}

		public void OnEnableDebug()
		{
		}

		public void OnBuild(Recipe r)
		{
			if (this.build == 0 && r.id == "workbench")
			{
				this.build = 1;
			}
		}

		public bool IsStoryPlayed(int flag)
		{
			return this.playedStories.Contains(flag);
		}

		public int GetStoryRowID(string idBook, string idStep)
		{
			foreach (Dictionary<string, string> dictionary in this.GetStoryExcelData(idBook).sheets["index"].list)
			{
				if (dictionary["step"] == idStep)
				{
					return dictionary["id"].ToInt();
				}
			}
			return 0;
		}

		public Dictionary<string, string> GetStoryRow(string idBook, int id)
		{
			foreach (Dictionary<string, string> dictionary in this.GetStoryExcelData(idBook).sheets["index"].list)
			{
				if (dictionary["id"].ToInt() == id)
				{
					return dictionary;
				}
			}
			return null;
		}

		public ExcelData GetStoryExcelData(string idBook)
		{
			ExcelData excelData = this.storyExcel.TryGetValue(idBook, null);
			if (excelData == null)
			{
				excelData = new ExcelData();
				excelData.path = CorePath.DramaData + idBook + ".xlsx";
				excelData.BuildList("index");
				this.storyExcel.Add(idBook, excelData);
			}
			return excelData;
		}

		public bool PlayStory(string idBook, int id, bool fromBook = false)
		{
			if (!fromBook && this.playedStories.Contains(id))
			{
				return false;
			}
			Dictionary<string, string> storyRow = this.GetStoryRow(idBook, id);
			SoundManager.ForceBGM();
			LayerDrama.Activate(idBook, storyRow["sheet"], storyRow["step"], null, null, "");
			if (!fromBook && !this.playedStories.Contains(id))
			{
				this.playedStories.Add(id);
			}
			return true;
		}

		public bool PlayStory(int id, bool fromBook = false)
		{
			return this.PlayStory("_main", id, fromBook);
		}

		public void AddStory(int id)
		{
			if (!this.playedStories.Contains(id) && !this.availableStories.Contains(id))
			{
				this.availableStories.Add(id);
				Msg.Say("story_added");
				SE.WriteJournal();
			}
		}

		public void OnEnterZone()
		{
			if (EClass._zone.id == "nymelle")
			{
				if (EClass._zone.lv == -1)
				{
					this.AddStory(10);
				}
				if (EClass._zone.lv == -2)
				{
					this.AddStory(20);
				}
				if (EClass._zone.lv == -3)
				{
					this.AddStory(30);
				}
			}
			if (QuestMain.Phase == 700 && EClass._zone.id == "lothria")
			{
				this.PlayStory(50, false);
			}
			if ((EClass._zone.IsPCFaction || EClass._zone.id == EClass.game.Prologue.idStartZone) && this.availableStories.Count > 0)
			{
				this.PlayStory(this.availableStories[0], false);
				this.availableStories.RemoveAt(0);
			}
		}

		public void OnLeaveZone()
		{
			if (QuestMain.Phase == 700)
			{
				this.PlayStory(40, false);
			}
		}

		public const int MonoDeparture = 100;

		[JsonProperty]
		public int[] ints = new int[50];

		[JsonProperty]
		public List<int> playedStories = new List<int>();

		[JsonProperty]
		public List<int> availableStories = new List<int>();

		[JsonProperty]
		public HashSet<int> pleaseDontTouch = new HashSet<int>();

		[JsonProperty]
		public List<string> reservedTutorial = new List<string>();

		public BitArray32 bits;

		public Dictionary<string, ExcelData> storyExcel = new Dictionary<string, ExcelData>();
	}
}
