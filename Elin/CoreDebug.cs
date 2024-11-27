using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ReflexCLI.Attributes;
using Steamworks;
using UnityEngine;

[ConsoleCommandClassCustomizer("")]
public class CoreDebug : EScriptable
{
	public bool godBuild
	{
		get
		{
			return this._godBuild || this.runtimeGodBuild;
		}
	}

	private IEnumerable<string> ZoneIDs()
	{
		List<SourceZone.Row> rows = EClass.sources.zones.rows;
		List<string> list = new List<string>();
		foreach (SourceZone.Row row in rows)
		{
			list.Add(row.id);
		}
		return list;
	}

	public void Init()
	{
		this.InitDebugCommands();
		EInput.disableKeyAxis = Application.isEditor;
	}

	public void QuickStart()
	{
		Game.Create("quick");
		Tutorial.debugSkip = EClass.debug.skipNerun;
		if (!this.startSetting.race.IsEmpty())
		{
			EClass.pc.ChangeRace(this.startSetting.race);
		}
		if (!this.startSetting.job.IsEmpty())
		{
			EClass.pc.ChangeJob(this.startSetting.job);
		}
		CoreDebug.StartScene startScene = this.startScene;
		if (startScene - CoreDebug.StartScene.Zone > 1)
		{
			if (startScene == CoreDebug.StartScene.Home_Cave)
			{
				EClass.game.idPrologue = 2;
			}
		}
		else
		{
			EClass.game.world.date.hour = EClass.debug.startHour;
		}
		EClass.game.StartNewGame();
		EClass.player.flags.OnEnableDebug();
		EClass.player.pref.lastIdTabAbility = 3;
		CoreDebug.<>c__DisplayClass102_0 CS$<>8__locals1;
		CS$<>8__locals1.homeZone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
		CoreDebug.StartParty startParty = this.startParty;
		if (startParty != CoreDebug.StartParty.Farris)
		{
			if (startParty == CoreDebug.StartParty.Full)
			{
				CoreDebug.<QuickStart>g__AddParty|102_0("farris", ref CS$<>8__locals1);
				CoreDebug.<QuickStart>g__AddParty|102_0("wescott", ref CS$<>8__locals1);
			}
		}
		else
		{
			CoreDebug.<QuickStart>g__AddParty|102_0("farris", ref CS$<>8__locals1);
		}
		int lv = this.startSetting.lv;
		if (this.allAbility)
		{
			foreach (SourceElement.Row row in EClass.sources.elements.rows)
			{
				if (row.category == "ability" && row.aliasRef != "mold" && !EClass.pc.HasElement(row.id, 1) && (row.aliasRef.IsEmpty() || row.id <= 10000 || EClass.sources.elements.alias[row.aliasRef].tag.Contains(row.alias.Split('_', StringSplitOptions.None)[0])))
				{
					EClass.pc.elements.ModBase(row.id, lv).vPotential = 30;
				}
			}
		}
		if (this.allSkill)
		{
			foreach (SourceElement.Row row2 in EClass.sources.elements.rows)
			{
				if (row2.isSkill && !row2.tag.Contains("unused"))
				{
					EClass.pc.elements.ModBase(row2.id, 1);
				}
			}
		}
		EClass.pc.elements.ModBase(60, lv / 2);
		EClass.pc.elements.ModBase(61, lv / 2);
		EClass.pc.elements.ModBase(79, lv / 5);
		EClass.player.totalFeat = lv;
		EClass.pc.SetLv(lv - 1);
		EClass.player.fame = EClass.pc.LV * 100;
		CS$<>8__locals1.idx = 0;
		CoreDebug.StartLoadout startLoadout = this.startLoadout;
		if (startLoadout != CoreDebug.StartLoadout.AllItem)
		{
			if (startLoadout == CoreDebug.StartLoadout.FewItem)
			{
				Thing thing = ThingGen.Create("backpack", -1, -1);
				thing.AddCard(ThingGen.Create("rp_food", -1, 10).SetNum(30));
				thing.AddCard(ThingGen.Create("rp_block", -1, 10).SetNum(30));
				for (int i = 0; i < 30; i++)
				{
					thing.AddCard(ThingGen.Create("rp_block", -1, 50));
				}
				EClass.pc.AddCard(thing);
				thing = ThingGen.Create("pouch", -1, -1);
				for (int j = 0; j < 30; j++)
				{
					CardBlueprint.Set(CardBlueprint.DebugEQ);
					Thing thing2 = ThingGen.CreateFromCategory("weapon", EClass.pc.LV);
					thing2.elements.SetBase(653, 1, 0);
					if (!(thing2.trait is TraitAmmo))
					{
						thing.AddCard(thing2);
					}
					thing2 = ThingGen.CreateFromCategory("armor", EClass.pc.LV);
					thing2.elements.SetBase(653, 1, 0);
					thing.AddCard(thing2);
				}
				EClass.pc.AddCard(thing);
				thing = ThingGen.Create("pouch", -1, -1);
				for (int k = 0; k < 30; k++)
				{
					thing.AddCard(ThingGen.Create("mathammer", MATERIAL.GetRandomMaterial(100, null, false).alias));
				}
				EClass.pc.AddCard(thing);
				thing = ThingGen.Create("pouch", -1, -1);
				for (int l = 0; l < 30; l++)
				{
					thing.AddCard(ThingGen.CreateFromCategory("ammo", EClass.pc.LV));
				}
				EClass.pc.AddCard(thing);
				thing = ThingGen.Create("coolerbox", -1, -1);
				for (int m = 0; m < 20; m++)
				{
					thing.AddCard(ThingGen.CreateFromCategory("foodstuff", -1).SetNum(EScriptable.rnd(10) + 1));
				}
				EClass.pc.AddCard(thing);
				List<SourceChara.Row> source = (from a in EClass.sources.charas.map.Values
				where a._idRenderData == "chara"
				select a).ToList<SourceChara.Row>();
				thing = ThingGen.Create("pouch", -1, -1);
				source = (from a in EClass.sources.charas.map.Values
				where a._idRenderData == "chara_L"
				select a).ToList<SourceChara.Row>();
				for (int n = 0; n < 20; n++)
				{
					string id = source.RandomItem<SourceChara.Row>().id;
					Thing thing3 = ThingGen.Create("figure", -1, -1);
					thing3.MakeFigureFrom(id);
					thing.AddCard(thing3);
				}
				EClass.pc.AddCard(thing);
				thing = ThingGen.Create("pouch", -1, -1);
				source = (from a in EClass.sources.charas.map.Values
				where a._idRenderData == "chara"
				select a).ToList<SourceChara.Row>();
				for (int num = 0; num < 20; num++)
				{
					string id2 = source.RandomItem<SourceChara.Row>().id;
					Thing thing4 = ThingGen.Create("figure3", -1, -1);
					thing4.MakeFigureFrom(id2);
					thing.AddCard(thing4);
				}
				EClass.pc.AddCard(thing);
				thing.Dye("oak");
				thing = ThingGen.Create("pouch", -1, -1);
				source = (from a in EClass.sources.charas.map.Values
				where a._idRenderData == "chara_L"
				select a).ToList<SourceChara.Row>();
				for (int num2 = 0; num2 < 20; num2++)
				{
					string id3 = source.RandomItem<SourceChara.Row>().id;
					Thing thing5 = ThingGen.Create("figure3", -1, -1);
					thing5.MakeFigureFrom(id3);
					thing.AddCard(thing5);
				}
				EClass.pc.AddCard(thing);
				thing.Dye("pine");
				thing = ThingGen.Create("pouch", -1, -1);
				thing.AddCard(ThingGen.Create("flour", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("wheat", -1, -1).SetNum(200));
				thing.AddCard(ThingGen.Create("rice_plant", -1, -1).SetNum(200));
				thing.AddCard(ThingGen.Create("noodle", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("rice", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("dough_cake", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("dough_bread", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("salt", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("sugar", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("honey", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("yeast", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("ketchup", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("butter", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("potion_empty", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("bucket_empty", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("battery", -1, -1));
				EClass.pc.AddCard(thing);
				thing.Dye("saphire");
				thing = ThingGen.Create("pouch", -1, -1);
				thing.AddCard(ThingGen.Create("log", -1, -1).SetNum(99));
				thing.AddCard(ThingGen.Create("log", -1, -1).SetNum(99).ChangeMaterial("pine"));
				thing.AddCard(ThingGen.Create("ore", -1, -1).SetNum(99).ChangeMaterial("steel"));
				thing.AddCard(ThingGen.Create("ore", -1, -1).SetNum(99).ChangeMaterial("copper"));
				thing.AddCard(ThingGen.Create("ore_gem", -1, -1).SetNum(99).ChangeMaterial("gold"));
				thing.AddCard(ThingGen.Create("gem", -1, -1).SetNum(99).ChangeMaterial("rubinus"));
				thing.AddCard(ThingGen.Create("flower_white", -1, -1).SetNum(99));
				thing.AddCard(ThingGen.Create("bait", -1, -1).SetNum(10));
				thing.AddCard(ThingGen.Create("seed", -1, -1).SetNum(99));
				EClass.pc.AddCard(thing);
				thing.Dye("rubinus");
				Thing thing6 = ThingGen.Create("quiver", -1, -1);
				thing6.AddCard(ThingGen.Create("bullet", -1, -1).SetNum(250));
				thing6.AddCard(ThingGen.Create("arrow", -1, -1).SetNum(250));
				thing6.AddCard(ThingGen.Create("bolt", -1, -1).SetNum(250));
				thing6.AddCard(ThingGen.Create("bullet_energy", -1, -1).SetNum(250));
				EClass.pc.body.GetEquippedThing(44).AddCard(thing6);
				thing = ThingGen.Create("backpack", -1, -1);
				for (int num3 = 0; num3 < 10; num3++)
				{
					thing.AddThing("book", lv);
					thing.AddThing("parchment", -1);
					thing.AddThing("book_ancient", -1);
				}
				EClass.pc.AddCard(thing);
				thing.Dye("rubinus");
				thing = ThingGen.Create("pouch", -1, -1);
				for (int num4 = 0; num4 < 30; num4++)
				{
					thing.AddThing((EScriptable.rnd(10) != 0) ? "potion" : "drink", lv).SetNum(99);
				}
				EClass.pc.AddCard(thing);
				thing.Dye("steel");
				thing = ThingGen.Create("pouch", -1, -1);
				for (int num5 = 0; num5 < 30; num5++)
				{
					thing.AddThing(ThingGen.CreateFromCategory("rod", lv), true, -1, -1);
				}
				EClass.pc.AddCard(thing);
				thing.Dye("steel");
				thing = ThingGen.Create("pouch", -1, -1);
				for (int num6 = 0; num6 < 30; num6++)
				{
					thing.AddThing("scroll", lv).SetNum(99);
				}
				EClass.pc.AddCard(thing);
				thing.Dye("steel");
				thing = ThingGen.Create("pouch", -1, -1);
				for (int num7 = 0; num7 < 40; num7++)
				{
					thing.AddThing("372", 100);
				}
				EClass.pc.AddCard(thing);
				thing.Dye("steel");
				Thing thing7 = EClass.pc.things.Find("purse", -1, -1);
				thing7.AddThing("casino_coin", -1).SetNum(30000000);
				thing7.AddThing("medal", -1).SetNum(1000);
				thing7.ModCurrency(500, "plat");
				EClass.pc.AddThing("record", -1);
				EClass.pc.AddThing("deed", -1).SetNum(5);
				EClass.pc.AddThing("book_story", -1);
				EClass.pc.AddThing("book_tutorial", -1);
				EClass.pc.AddThing("water", -1).SetNum(20).SetBlessedState(BlessedState.Blessed);
				EClass.pc.AddThing("water", -1).SetNum(20).SetBlessedState(BlessedState.Cursed);
				EClass.pc.AddThing("potion_empty", -1).SetNum(20);
				EClass.pc.ModCurrency(10000000, "money");
				EClass.pc.AddCard(ThingGen.CreateBlock(67, 1).SetNum(99));
				EClass.pc.AddCard(ThingGen.CreateFloor(41, 1, false).SetNum(99));
				Thing thing8 = ThingGen.Create("scroll_random", -1, -1).SetNum(10);
				thing8.refVal = 8220;
				EClass.pc.AddCard(thing8);
				EClass.pc.ModCurrency(50, "money2");
				CoreDebug.<QuickStart>g__AddHotbar|102_1("hoe", ref CS$<>8__locals1);
				CoreDebug.<QuickStart>g__AddHotbar|102_1("shovel", ref CS$<>8__locals1);
				CoreDebug.<QuickStart>g__AddHotbar|102_1("axe", ref CS$<>8__locals1);
				CoreDebug.<QuickStart>g__AddHotbar|102_1("pickaxe", ref CS$<>8__locals1);
				CoreDebug.<QuickStart>g__AddHotbar|102_1("wateringCan", ref CS$<>8__locals1);
				CoreDebug.<QuickStart>g__AddHotbar|102_1("gun_assault", ref CS$<>8__locals1);
				CoreDebug.<QuickStart>g__AddHotbar|102_1("hammer", ref CS$<>8__locals1);
				CoreDebug.<QuickStart>g__AddHotbar|102_1("bow", ref CS$<>8__locals1);
			}
		}
		else
		{
			EClass.pc.ModCurrency(50, "plat");
			EClass.pc.ModCurrency(50, "money2");
			EClass.pc.EQ_CAT("weapon");
			EClass.pc.EQ_CAT((EScriptable.rnd(2) == 0) ? "weapon" : "shield");
			EClass.pc.EQ_CAT("torso");
			EClass.pc.EQ_CAT("arm");
			EClass.pc.EQ_CAT("ring");
			EClass.pc.AddCard(ThingGen.Create("chest2", -1, -1));
			EClass.pc.AddCard(ThingGen.Create("torch_held", -1, -1));
			CoreDebug.<QuickStart>g__AddHotbar|102_1("axe", ref CS$<>8__locals1);
			CoreDebug.<QuickStart>g__AddHotbar|102_1("pickaxe", ref CS$<>8__locals1);
			CoreDebug.<QuickStart>g__AddHotbar|102_1("wateringCan", ref CS$<>8__locals1);
			CoreDebug.<QuickStart>g__AddHotbar|102_1("bow", ref CS$<>8__locals1);
			CoreDebug.<QuickStart>g__AddHotbar|102_1("deed", ref CS$<>8__locals1);
			CoreDebug.<QuickStart>g__AddAbility|102_2("SpellTeleport");
			CoreDebug.<QuickStart>g__AddAbility|102_2("SpellCatsEye");
			CoreDebug.<QuickStart>g__AddHotbar|102_1("map_big", ref CS$<>8__locals1);
		}
		EClass.pc.RestockEquip(true);
		EClass.pc.stamina.value = EClass.pc.stamina.max;
		EClass.pc.mana.value = EClass.pc.mana.max;
		EClass.pc.hp = EClass.pc.MaxHP;
		if (EClass.debug.startScene == CoreDebug.StartScene.Zone)
		{
			EClass._zone.ClaimZone(true);
		}
		SoundManager.ignoreSounds = true;
		EClass.ui.ToggleInventory(false);
		SoundManager.ignoreSounds = true;
		EClass.ui.ToggleAbility(false);
		SoundManager.ignoreSounds = false;
		EClass.ui.layerFloat.GetLayer<LayerAbility>(false).windows[0].SetRect(EClass.core.refs.rects.abilityDebug, false);
		if (this.allRecipe)
		{
			this.AddAllRecipes();
		}
		foreach (Thing thing9 in EClass.pc.things)
		{
			thing9.isNew = false;
		}
		LayerInventory.SetDirtyAll(false);
		EClass.core.actionsNextFrame.Add(new Action(EClass.pc.RecalculateFOV));
		if (this.startScene == CoreDebug.StartScene.Story_Test)
		{
			EClass._zone.ClaimZone(true);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("loytel", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("farris", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("kettle", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("quru", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("corgon", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("demitas", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
			EClass.game.quests.Add("greatDebt", null);
			EClass.game.quests.Add("farris_tulip", null);
			EClass.game.quests.Add("kettle_join", null);
			EClass.game.quests.Add("quru_morning", null);
			EClass.game.quests.Add("vernis_gold", null);
			EClass.game.quests.Add("quru_sing", null);
			EClass.game.quests.Add("quru_past1", null);
			EClass.game.quests.Add("quru_past2", null);
			EClass.game.quests.Add("pre_debt", null);
		}
	}

	public void OnLoad()
	{
		if (this.allRecipe)
		{
			this.AddAllRecipes();
		}
	}

	public void AddAllRecipes()
	{
		foreach (RecipeSource recipeSource in RecipeManager.list)
		{
			if (!EClass.player.recipes.knownRecipes.ContainsKey(recipeSource.id))
			{
				EClass.player.recipes.knownRecipes.Add(recipeSource.id, 1);
			}
		}
	}

	public void SetStartStockpile(Thing container, int num = 100)
	{
		LittlePopper.skipPop = true;
		foreach (SourceMaterial.Row row in EClass.sources.materials.rows)
		{
			if (!(row.alias == "void") && (!this.randomResource || EScriptable.rnd(4) == 0))
			{
				row.CreateByProduct(container, num);
			}
		}
		foreach (SourceThing.Row row2 in EClass.sources.things.rows)
		{
			if ((row2.factory.IsEmpty() || !(row2.factory[0] == "x")) && !row2.isOrigin && row2.Category.tag.Contains("debug"))
			{
				Thing thing = ThingGen.Create(row2.id, -1, -1);
				if (thing.trait.CanStack)
				{
					thing.ModNum(num, true);
				}
				container.AddThing(thing, true, -1, -1);
			}
		}
		for (int i = 0; i < 10; i++)
		{
			container.AddThing(TraitSeed.MakeRandomSeed(false), true, -1, -1);
		}
		LittlePopper.skipPop = false;
	}

	public Thing GetOrCreateDebugContainer()
	{
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.c_altName == "DebugContainer")
			{
				return thing;
			}
		}
		Thing thing2 = EClass._zone.AddThing("barrel", EClass.pc.pos).Thing;
		thing2.ChangeMaterial("obsidian");
		thing2.c_altName = "DebugContainer";
		thing2.isNPCProperty = false;
		thing2.SetPlaceState(PlaceState.installed, false);
		return thing2;
	}

	public Thing GetDebugContainer()
	{
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.c_altName == "DebugContainer")
			{
				return thing;
			}
		}
		return null;
	}

	public void EnableDebugResource()
	{
		this.AddAllRecipes();
		EClass.player.flags.OnEnableDebug();
		this.enable = true;
		if (WidgetHotbar.HotbarBuild)
		{
			WidgetHotbar.HotbarBuild.RebuildPage(-1);
		}
		WidgetSystemIndicator.Refresh();
		Thing orCreateDebugContainer = this.GetOrCreateDebugContainer();
		this.SetStartStockpile(orCreateDebugContainer, this.numResource);
	}

	public void SpawnCheatContainer()
	{
	}

	public void UpdateAlways()
	{
		if (!EClass.debug.enable || EClass.ui.GetLayer<LayerConsole>(false))
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			UIButton.TryShowTip(null, true, true);
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			Steam.GetAchievement(ID_Achievement.test);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			Steam.GetAchievement(ID_Achievement.mew);
		}
		if (Input.GetKeyDown(KeyCode.F11))
		{
			if (Application.isEditor)
			{
				EClass.core.SetLang((Lang.langCode == "EN") ? "JP" : "EN", false);
			}
			else
			{
				EClass.sources.ImportSourceTexts();
			}
			NewsList.dict = null;
			BookList.dict = null;
			BookList.Init();
		}
		if (!EClass.core.IsGameStarted)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			Vector3 offset = EClass.screen.tileMap.rendererInnerBlock.offset;
			Msg.SayRaw(offset.ToString() ?? "");
			EClass.screen.tileMap.rendererInnerBlock.offset = Vector3.zero;
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			Vector3 offset = EClass.screen.tileMap.rendererInnerBlock.offset;
			Msg.SayRaw(offset.ToString() ?? "");
			EClass.screen.tileMap.rendererInnerBlock.offset = new Vector3(0f, 0.54f, -0.5f);
		}
	}

	public void ValidateData()
	{
		if (!EClass.core.IsGameStarted)
		{
			return;
		}
		EClass._map.Stocked.Validate();
		EClass._map.Roaming.Validate();
		EClass._map.Installed.Validate();
	}

	public void UpdateInput()
	{
		if (!this.debugInput || EInput.isInputFieldActive || EClass.ui.GetLayer<LayerConsole>(false))
		{
			return;
		}
		bool key = Input.GetKey(KeyCode.LeftShift);
		bool key2 = Input.GetKey(KeyCode.LeftAlt);
		bool key3 = Input.GetKey(KeyCode.LeftControl);
		TraitStairs traitStairs = null;
		if (Input.GetKeyDown(KeyCode.KeypadPlus))
		{
			traitStairs = EClass._map.FindThing<TraitStairsDown>();
		}
		if (Input.GetKeyDown(KeyCode.KeypadMinus))
		{
			traitStairs = EClass._map.FindThing<TraitStairsUp>();
		}
		if (traitStairs != null)
		{
			traitStairs.MoveZone(false);
		}
		if (EInput.action == EAction.ShowGrid)
		{
			EClass.ui.Find("_Guide").ToggleActive();
		}
		if (Input.GetKey(KeyCode.N))
		{
			if (key)
			{
				SoundManager.current.sourceBGM.pitch = 10f;
			}
			if (key3)
			{
				SoundManager.current.sourceBGM.time = SoundManager.current.currentBGM.length - 10f;
			}
		}
		else if (Input.GetKeyUp(KeyCode.N))
		{
			SoundManager.current.sourceBGM.pitch = 1f;
		}
		if (EClass.scene.actionMode.IsBuildMode)
		{
			string text = "festival";
			if (Input.GetKeyDown(KeyCode.F1))
			{
				EClass.ui.AddLayer<LayerDebug>();
			}
			if (Input.GetKeyDown(KeyCode.F2))
			{
				Zone.forceRegenerate = true;
				EClass.scene.actionMode.Deactivate();
				EClass.scene.Init(Scene.Mode.Zone);
				ActionMode.Inspect.Activate(true, false);
				SE.MoveZone();
			}
			if (Input.GetKeyDown(KeyCode.F4))
			{
				if (!MapSubset.Exist(text))
				{
					SE.Beep();
					return;
				}
				Zone.forceRegenerate = true;
				Zone.forceSubset = text;
				EClass.scene.actionMode.Deactivate();
				EClass.scene.Init(Scene.Mode.Zone);
				ActionMode.Inspect.Activate(true, false);
				SE.MoveZone();
				EClass.ui.Say("Loaded Subset", null);
			}
			if (Input.GetKeyDown(KeyCode.F5))
			{
				MapSubset.Save(text);
				SE.WriteJournal();
				EClass.ui.Say("Exported Subset", null);
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			EClass.ui.AddLayer<LayerDebug>();
			Debug.Log(EClass._zone);
			string id = EClass._zone.id;
			string str = "/";
			Spatial parent = EClass._zone.parent;
			Debug.Log(id + str + ((parent != null) ? parent.ToString() : null));
			Debug.Log(EClass._zone.RegionPos);
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			EClass._zone.TryGenerateEvolved(true, null);
			EClass._zone.SpawnMob(null, SpawnSetting.Boss(100, -1));
			Chara targetChara = EClass.scene.mouseTarget.TargetChara;
			if (targetChara != null)
			{
				EClass.pc.Pick(targetChara.MakeMilk(true, 1, true), true, true);
				EClass.pc.Pick(targetChara.MakeGene(null), true, true);
				EClass.pc.Pick(targetChara.MakeBraineCell(), true, true);
				EClass.pc.Pick(targetChara.MakeEgg(true, 10, true), true, true);
			}
			if (EClass.game.quests.Get<QuestDebt>() == null)
			{
				Chara chara = CharaGen.Create("loytel", -1);
				EClass._zone.AddCard(chara, EClass.pc.pos);
				chara.SetGlobal();
				Quest q = EClass.game.quests.Add("debt", "loytel");
				EClass.game.quests.Start(q);
				EClass.pc.party.RemoveMember(chara);
				chara.hostility = (chara.c_originalHostility = Hostility.Ally);
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.F3))
		{
			for (int i = 0; i < 10; i++)
			{
				Thing thing = ThingGen.Create("egg_fertilized", -1, -1);
				thing.TryMakeRandomItem(40);
				EClass.pc.Pick(thing, true, true);
			}
			foreach (Chara message in EClass._map.deadCharas)
			{
				Debug.Log(message);
			}
			EClass.core.steam.CheckUpdate();
			Player.Flags flags = EClass.player.flags;
			int loytelMartLv = flags.loytelMartLv;
			flags.loytelMartLv = loytelMartLv + 1;
			Msg.Say("loytelmart:" + EClass.player.flags.loytelMartLv.ToString());
			Guild.Fighter.relation.rank = 20;
			Guild.Mage.relation.rank = 20;
			Guild.Thief.relation.rank = 20;
			Guild.Merchant.relation.rank = 20;
			if (EClass.Branch != null)
			{
				EClass.Branch.ModExp(EClass.Branch.GetNextExp(-1));
			}
			foreach (Chara chara2 in EClass.pc.party.members)
			{
				chara2.AddExp(chara2.ExpToNext);
			}
			EClass.pc.PlayEffect("boost", true, 0f, default(Vector3));
			EClass.pc.PlaySound("boost", 1f, true);
			EClass.pc.elements.SetBase(306, 100, 0);
			EClass.pc.elements.SetBase(85, 100, 0);
			EClass.pc.feat += 10;
			EClass.player.totalFeat += 10;
			return;
		}
		if (Input.GetKeyDown(KeyCode.F4))
		{
			EClass.game.backupTime += 3600.0;
			return;
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			string text2 = Input.GetKey(KeyCode.LeftControl) ? "quick3" : (Input.GetKey(KeyCode.LeftShift) ? "quick2" : "quick");
			if (text2 != Game.id)
			{
				IO.DeleteDirectory(GameIO.pathSaveRoot + text2);
				IO.CopyAll(GameIO.pathSaveRoot + Game.id, GameIO.pathSaveRoot + text2, true);
				Game.id = text2;
			}
			EClass.game.Save(false, null, false);
		}
		if (Input.GetKeyDown(KeyCode.F6))
		{
			EClass.core.WaitForEndOfFrame(delegate
			{
				string slot = Input.GetKey(KeyCode.LeftControl) ? "quick3" : (Input.GetKey(KeyCode.LeftShift) ? "quick2" : "quick");
				EClass.scene.Init(Scene.Mode.None);
				Game.Load(slot);
			});
		}
		if (Input.GetKeyDown(KeyCode.F7))
		{
			MiniGame.Activate(MiniGame.Type.Scratch);
			return;
		}
		if (Input.GetKeyDown(KeyCode.F8))
		{
			SE.Click();
			this.EnableDebugResource();
			this.runtimeGodBuild = true;
			WidgetMenuPanel.OnChangeMode();
			EClass.player.hotbars.ResetHotbar(2);
		}
		if (key3)
		{
			if (Input.GetKeyDown(KeyCode.F9))
			{
				this.indexResolution++;
				if (this.indexResolution >= this.resolutions.Count)
				{
					this.indexResolution = 0;
				}
				int[] array = this.resolutions[this.indexResolution];
				int width = array[0];
				int height = array[1];
				Screen.SetResolution(width, height, key);
				WidgetFeed instance = WidgetFeed.Instance;
				if (instance != null)
				{
					instance.Nerun(width.ToString() + "/" + height.ToString(), "UN_nerun");
				}
			}
			if (Input.GetKeyDown(KeyCode.F10))
			{
				int width2 = Screen.width;
				int height2 = width2 / 16 * 9;
				Screen.SetResolution(width2, height2, key);
				WidgetFeed instance2 = WidgetFeed.Instance;
				if (instance2 != null)
				{
					instance2.Nerun(width2.ToString() + "/" + height2.ToString(), "UN_nerun");
				}
			}
		}
		else
		{
			Input.GetKeyDown(KeyCode.F9);
			if (Input.GetKey(KeyCode.F9))
			{
				EClass.scene.paused = false;
				for (int j = 0; j < this.advanceMin; j++)
				{
					EClass.game.updater.FixedUpdate();
				}
				EClass.game.world.date.AdvanceMin(this.advanceMin);
			}
			if (Input.GetKeyDown(KeyCode.F10))
			{
				switch (EScriptable.rnd(5))
				{
				case 0:
					EClass.world.weather.SetCondition(Weather.Condition.Rain, 20, false);
					break;
				case 1:
					EClass.world.weather.SetCondition(Weather.Condition.Snow, 20, false);
					break;
				case 2:
					EClass.world.weather.SetCondition(Weather.Condition.Ether, 20, false);
					break;
				default:
					EClass.world.weather.SetCondition(Weather.Condition.Fine, 20, false);
					break;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.N) && !EInput.isShiftDown && !EInput.isCtrlDown)
		{
			EClass.Sound.NextBGM();
		}
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (key)
			{
				EClass.ui.AddLayer<LayerConsole>();
			}
			else
			{
				EClass.ui.widgets.Toggle("Debug");
			}
		}
		Point hitPoint = Scene.HitPoint;
		if (!hitPoint.IsValid || EClass.ui.IsActive)
		{
			return;
		}
		if (hitPoint.detail != null)
		{
			Area area = hitPoint.area;
		}
		CellDetail detail = hitPoint.detail;
		if (((detail != null) ? detail.things : null) == null)
		{
			new List<Thing>();
		}
		CellDetail detail2 = hitPoint.detail;
		if (((detail2 != null) ? detail2.charas : null) == null)
		{
			new List<Chara>();
		}
		List<Card> list = hitPoint.ListCards(false);
		if (Input.GetMouseButtonDown(2))
		{
			if (EClass.core.releaseMode != ReleaseMode.Public)
			{
				if (key)
				{
					EClass._zone.SpawnMob(hitPoint, null);
					return;
				}
				if (key3)
				{
					EClass._zone.SpawnMob(hitPoint, null);
					return;
				}
				if (key2)
				{
					EClass._zone.SpawnMob(hitPoint, null);
				}
			}
			return;
		}
		if (key2)
		{
			if (Input.GetMouseButtonDown(0))
			{
				foreach (Card card in list)
				{
					Debug.Log(string.Concat(new string[]
					{
						card.Name,
						"/",
						card.dir.ToString(),
						"/",
						card.flipX.ToString(),
						"/",
						card.angle.ToString()
					}));
				}
			}
			if (Input.GetMouseButtonDown(1))
			{
				if (key)
				{
					EClass._map.charas.ForeachReverse(delegate(Chara c)
					{
						if (c.IsHostile(EClass.pc))
						{
							c.DamageHP(9999999, AttackSource.Finish, EClass.pc);
						}
					});
				}
				else if (hitPoint.detail != null)
				{
					for (int k = hitPoint.detail.charas.Count - 1; k >= 0; k--)
					{
						hitPoint.detail.charas[k].DamageHP(9999999, AttackSource.Finish, EClass.pc);
					}
				}
				EInput.Consume(false, 1);
			}
		}
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			if (Input.GetKey(KeyCode.RightControl))
			{
				return;
			}
			if (key)
			{
				using (List<Card>.Enumerator enumerator2 = EClass._map.Roaming.all.GetList<Card>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Card t = enumerator2.Current;
						EClass._zone.RemoveCard(t);
					}
					goto IL_BB6;
				}
			}
			if (hitPoint.detail != null)
			{
				for (int l = hitPoint.detail.things.Count - 1; l >= 0; l--)
				{
					hitPoint.detail.things[l].Destroy();
				}
				if (hitPoint.detail != null)
				{
					for (int m = hitPoint.detail.charas.Count - 1; m >= 0; m--)
					{
						hitPoint.detail.charas[m].Destroy();
					}
				}
			}
		}
		IL_BB6:
		if (Input.GetKeyDown(KeyCode.End) && hitPoint.detail != null)
		{
			for (int n = hitPoint.detail.things.Count - 1; n >= 0; n--)
			{
				Thing thing2 = hitPoint.detail.things[n];
				string[] array2 = new string[13];
				array2[0] = thing2.id;
				array2[1] = "/";
				array2[2] = thing2.Pref.height.ToString();
				array2[3] = "/";
				int num = 4;
				Trait trait = thing2.trait;
				array2[num] = ((trait != null) ? trait.ToString() : null);
				array2[5] = "/";
				array2[6] = thing2.source.tileType.CanStack.ToString();
				array2[7] = "/";
				int num2 = 8;
				TileType tileType = thing2.source.tileType;
				array2[num2] = ((tileType != null) ? tileType.ToString() : null);
				array2[9] = "/";
				array2[10] = thing2.isSynced.ToString();
				array2[11] = "/";
				array2[12] = RenderObject.syncList.Contains(thing2.renderer).ToString();
				Debug.Log(string.Concat(array2));
			}
		}
		if (!Application.isEditor)
		{
			return;
		}
		switch (this.debugHotkeys)
		{
		case CoreDebug.DebugHotkey.Block:
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				EClass._map.SetLiquid(hitPoint.x, hitPoint.z, 1, 9);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				EClass._map.SetLiquid(hitPoint.x, hitPoint.z, 2, 9);
			}
			break;
		case CoreDebug.DebugHotkey.Item:
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Thing thing3 = ThingGen.Create("stairsDown_cave", -1, -1);
				EClass._zone.AddCard(thing3, EClass.pc.pos);
				thing3.SetPlaceState(PlaceState.installed, false);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Thing thing4 = ThingGen.Create("stairs", -1, -1);
				EClass._zone.AddCard(thing4, EClass.pc.pos);
				thing4.SetPlaceState(PlaceState.installed, false);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Thing thing5 = ThingGen.Create("sign", -1, -1);
				EClass._zone.AddCard(thing5, hitPoint);
				thing5.SetPlaceState(PlaceState.installed, false);
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				Thing thing6 = ThingGen.Create("sign2", -1, -1);
				EClass._zone.AddCard(thing6, hitPoint);
				thing6.SetPlaceState(PlaceState.installed, false);
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				Thing thing7 = ThingGen.Create("well", -1, -1);
				EClass._zone.AddCard(thing7, hitPoint);
				thing7.SetPlaceState(PlaceState.installed, false);
			}
			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				Thing thing8 = ThingGen.Create("altar", -1, -1);
				EClass._zone.AddCard(thing8, hitPoint);
				thing8.SetPlaceState(PlaceState.installed, false);
			}
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				Thing t2 = ThingGen.Create("torch", -1, -1);
				EClass._zone.AddCard(t2, hitPoint);
			}
			if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				Thing t3 = ThingGen.Create("street_lamp", -1, -1);
				EClass._zone.AddCard(t3, hitPoint);
			}
			if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				Thing t4 = ThingGen.Create("statue_elin", -1, -1);
				EClass._zone.AddCard(t4, hitPoint);
			}
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				Thing t5 = ThingGen.TestCreate();
				EClass._zone.AddCard(t5, hitPoint);
			}
			if (key && Input.GetKeyDown(KeyCode.Alpha1))
			{
				Chara t6 = CharaGen.Create("korgon", -1);
				EClass._zone.AddCard(t6, hitPoint);
			}
			break;
		case CoreDebug.DebugHotkey.Decal:
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				EClass._map.AddDecal(hitPoint.x, hitPoint.z, EClass.pc.material.decal, 1, true);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
			}
			break;
		case CoreDebug.DebugHotkey.Test:
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 60;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 20;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 30;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 40;
			}
			break;
		case CoreDebug.DebugHotkey.Anime:
		{
			int num3 = -1;
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				num3 = 0;
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				num3 = 1;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				num3 = 2;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				num3 = 3;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				num3 = 4;
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				num3 = 5;
			}
			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				num3 = 6;
			}
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				num3 = 7;
			}
			if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				num3 = 8;
			}
			if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				num3 = 9;
			}
			if (num3 != -1)
			{
				foreach (Card card2 in hitPoint.ListCards(false))
				{
					card2.renderer.PlayAnime(num3.ToEnum<AnimeID>(), default(Vector3), false);
				}
				Debug.Log(num3.ToEnum<AnimeID>());
			}
			break;
		}
		}
		Card card3 = hitPoint.FirstChara ?? hitPoint.FirstThing;
		if (card3 != null)
		{
			CharaActorPCC charaActorPCC = (card3.renderer.actor as CharaActor) as CharaActorPCC;
			if (charaActorPCC)
			{
				if (Input.GetKeyDown(KeyCode.Keypad0))
				{
					charaActorPCC.NextFrame();
					charaActorPCC.RefreshSprite();
				}
				if (Input.GetKeyDown(KeyCode.Keypad1))
				{
					charaActorPCC.NextDir();
					card3.angle = charaActorPCC.provider.angle;
					charaActorPCC.RefreshSprite();
				}
				if (Input.GetKeyDown(KeyCode.Keypad2))
				{
					charaActorPCC.pcc.data.Randomize(null, null, true);
					charaActorPCC.provider.Rebuild(PCCState.Normal);
				}
			}
			if (Input.GetKeyDown(KeyCode.Keypad4))
			{
				this.lastEmo = this.lastEmo.NextEnum<Emo>();
				card3.ShowEmo(Emo.none, 0f, true);
				card3.ShowEmo(this.lastEmo, 0f, true);
			}
			if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				card3.ShowEmo(Emo.none, 0f, true);
				card3.ShowEmo(this.lastEmo, 0f, true);
			}
			if (Input.GetKeyDown(KeyCode.Keypad7))
			{
				this.lastAnime = this.lastAnime.NextEnum<AnimeID>();
				card3.renderer.PlayAnime(this.lastAnime, default(Vector3), false);
			}
			if (Input.GetKeyDown(KeyCode.Keypad8))
			{
				card3.renderer.PlayAnime(this.lastAnime, default(Vector3), false);
				this.animeDest = card3;
			}
			if (Input.GetKeyDown(KeyCode.Keypad9))
			{
				card3.renderer.PlayAnime(AnimeID.Attack, this.animeDest);
			}
		}
	}

	public void InitDebugCommands()
	{
		this.commands.Clear();
		int cat = 0;
		this.<InitDebugCommands>g__Add|113_0(cat, "Save Widgets", delegate
		{
			EClass.ui.widgets.Save(null);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Save Widgets(Dialog)", delegate
		{
			EClass.ui.widgets.DialogSave(null);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Export Zone", delegate
		{
			EClass._zone.Export();
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Export Zone(Dialog)", delegate
		{
			EClass._zone.ExportDialog(null);
		});
		this.<InitDebugCommands>g__Add|113_0(0, "Import Zone(Dialog)", delegate
		{
			EClass._zone.ImportDialog(null);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Validate Backer Contents", delegate
		{
			foreach (SourceBacker.Row row in EClass.sources.backers.rows)
			{
				if (row.valid)
				{
					int type = row.type;
					if (type != 1)
					{
						if (type != 4)
						{
							if (type == 6)
							{
								if (!EClass.sources.religions.map.ContainsKey(row.deity.ToLower()))
								{
									Debug.Log(string.Concat(new string[]
									{
										row.id.ToString(),
										"/",
										row.Name,
										"/follower/",
										row.deity
									}));
								}
							}
						}
						else if (!EClass.sources.charas.map.ContainsKey(row.chara))
						{
							Debug.Log(string.Concat(new string[]
							{
								row.id.ToString(),
								"/pet/",
								row.Name,
								"/",
								row.chara
							}));
						}
					}
					else if (!EClass.sources.things.map.ContainsKey(row.loot))
					{
						Debug.Log(string.Concat(new string[]
						{
							row.id.ToString(),
							"/remain/",
							row.Name,
							"/",
							row.loot
						}));
					}
				}
			}
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Edit PCC", delegate
		{
			EClass.ui.AddLayer<LayerEditPCC>().Activate(EClass.pc, UIPCC.Mode.Full, null, null);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "COM_Teleport", new Action(this.COM_Teleport));
		this.<InitDebugCommands>g__Add|113_0(cat, "LOG_Spatials", new Action(this.LOG_Spatials));
		this.<InitDebugCommands>g__Add|113_0(cat, "Play Start Drama", new Action(this.COM_PlayStartDrama));
		this.<InitDebugCommands>g__Add|113_0(cat, "Fix Floating Items", delegate
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.Pref.Float)
				{
					thing.isFloating = true;
				}
			}
		});
		cat = 1;
		this.<InitDebugCommands>g__Add|113_0(cat, "Add Conditions", delegate
		{
			if (EScriptable.rnd(2) == 0)
			{
				EClass.pc.AddCondition<ConWet>(100, false);
			}
			else
			{
				EClass.pc.AddCondition<ConSuffocation>(100, false);
			}
			if (EScriptable.rnd(2) == 0)
			{
				EClass.pc.AddCondition<ConPoison>(100, false);
				return;
			}
			EClass.pc.AddCondition<ConDisease>(100, false);
		});
		cat = 2;
		this.<InitDebugCommands>g__Add|113_0(cat, "Max Construction", new Action(this.COM_MaxConstruction));
		this.<InitDebugCommands>g__Add|113_0(cat, "Add Reserves", delegate
		{
			EClass.Home.AddReserve(CharaGen.Create("merc_archer", -1));
			EClass.Home.AddReserve(CharaGen.Create("healer", -1));
			EClass.Home.AddReserve(CharaGen.Create("bartender", -1));
			EClass.Home.AddReserve(CharaGen.Create("merchant", -1));
			EClass.Home.AddReserve(CharaGen.CreateFromFilter("c_wilds", -1, -1));
			EClass.Home.AddReserve(CharaGen.CreateFromFilter("c_wilds", -1, -1));
			EClass.Home.AddReserve(CharaGen.CreateFromFilter("c_wilds", -1, -1));
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Add Recruits", delegate
		{
			if (EClass.Branch == null)
			{
				return;
			}
			EClass.Branch.AddRecruit(CharaGen.Create("merc_archer", -1));
			EClass.Branch.AddRecruit(CharaGen.Create("healer", -1));
			EClass.Branch.AddRecruit(CharaGen.Create("bartender", -1));
			EClass.Branch.AddRecruit(CharaGen.Create("merchant", -1));
			EClass.Branch.AddRecruit(CharaGen.CreateFromFilter("c_wilds", -1, -1));
			EClass.Branch.AddRecruit(CharaGen.CreateFromFilter("c_wilds", -1, -1));
			EClass.Branch.AddRecruit(CharaGen.CreateFromFilter("c_wilds", -1, -1));
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Add Resources", delegate
		{
			if (EClass.Branch == null)
			{
				return;
			}
			EClass.Branch.resources.food.Mod(100, true);
			EClass.Branch.resources.knowledge.Mod(100, true);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Add Influence", delegate
		{
			EClass._zone.influence += 100;
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Reroll Hobbies", delegate
		{
			foreach (Chara chara in EClass._map.charas)
			{
				chara.RerollHobby(true);
			}
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Test_Siege", new Action(this.Test_Siege));
		this.<InitDebugCommands>g__Add|113_0(cat, "Test_SiegeGuard", new Action(this.Test_SiegeGuard));
		this.<InitDebugCommands>g__Add|113_0(cat, "Log_BranchMembers", delegate
		{
			if (EClass._zone.IsPCFaction)
			{
				Debug.Log(EClass.Branch.members.Count);
				foreach (Chara o in EClass.Branch.members)
				{
					EClass.debug.Log(o);
				}
			}
		});
		cat = 3;
		this.<InitDebugCommands>g__Add|113_0(cat, "Weather.Fine", delegate
		{
			EClass.world.weather.SetCondition(Weather.Condition.Fine, 20, false);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Weather.Blossom", delegate
		{
			EClass.world.weather.SetCondition(Weather.Condition.Blossom, 20, false);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Weather.Random", delegate
		{
			EClass.world.weather.SetRandomCondition();
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Weather.Ether+50", delegate
		{
			EClass.world.ModEther(50);
			Debug.Log(EClass.world.ether.ToString() + "/" + EClass.world.weather.CurrentCondition.ToString());
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Season.Next", delegate
		{
			EClass.world.season.Next();
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Unreveal Map", delegate
		{
			EClass._map.ForeachCell(delegate(Cell c)
			{
				c.isSeen = false;
			});
			WidgetMinimap.Instance.Reload();
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Test_GodTalk", new Action(this.Test_GodTalk));
		this.<InitDebugCommands>g__Add|113_0(cat, "Test_Filter", new Action(this.Test_Filter));
		this.<InitDebugCommands>g__Add|113_0(cat, "Test_Grow", new Action(this.Test_Grow));
		this.<InitDebugCommands>g__Add|113_0(cat, "Turn On All Lights", delegate
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.trait.IsLighting)
				{
					thing.trait.Toggle(true, false);
				}
			}
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Reset All Custom Lights", delegate
		{
			foreach (Thing thing in EClass._map.things)
			{
				thing.c_lightColor = 0;
				thing.RecalculateFOV();
			}
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Reset All obj materials", delegate
		{
			EClass._map.ForeachCell(delegate(Cell c)
			{
				if (c.HasObj)
				{
					c.objMat = (byte)c.sourceObj.DefaultMaterial.id;
				}
			});
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Fix Floors under Blocks", delegate
		{
			EClass._map.ForeachCell(delegate(Cell c)
			{
				if (c.HasFullBlock)
				{
					SourceBlock.Row sourceBlock = c.sourceBlock;
					SourceFloor.Row row = EClass.sources.floors.alias[sourceBlock.autoFloor];
					EClass._map.SetFloor((int)c.x, (int)c.z, row.DefaultMaterial.id, row.id);
				}
			});
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Bless Inventory", delegate
		{
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				t.SetBlessedState(BlessedState.Blessed);
			}, true);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "Curse Inventory", delegate
		{
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				t.SetBlessedState(BlessedState.Cursed);
			}, true);
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "List Global Charas", delegate
		{
			foreach (KeyValuePair<int, Chara> keyValuePair in EClass.game.cards.globalCharas)
			{
				string[] array = new string[9];
				array[0] = keyValuePair.Key.ToString();
				array[1] = "/";
				array[2] = keyValuePair.Value.Name;
				array[3] = "/";
				array[4] = ((keyValuePair.Value.currentZone == null) ? "NULL" : keyValuePair.Value.currentZone.Name);
				array[5] = "/";
				int num = 6;
				Faction faction = keyValuePair.Value.faction;
				array[num] = ((faction != null) ? faction.ToString() : null);
				array[7] = "/";
				int num2 = 8;
				FactionBranch homeBranch = keyValuePair.Value.homeBranch;
				array[num2] = ((homeBranch != null) ? homeBranch.ToString() : null);
				Debug.Log(string.Concat(array));
			}
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "List Global Charas In Zone", delegate
		{
			foreach (KeyValuePair<int, Chara> keyValuePair in EClass.game.cards.globalCharas)
			{
				if (keyValuePair.Value.currentZone == EClass._zone)
				{
					string str = keyValuePair.Key.ToString();
					string str2 = "/";
					Chara value = keyValuePair.Value;
					Debug.Log(str + str2 + ((value != null) ? value.ToString() : null));
				}
			}
		});
		this.<InitDebugCommands>g__Add|113_0(cat, "List Citizen", delegate
		{
			using (Dictionary<int, string>.Enumerator enumerator = EClass._zone.dictCitizen.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, string> p = enumerator.Current;
					Debug.Log(p.Value);
					Debug.Log(EClass._map.charas.Find((Chara c) => c.uid == p.Key));
					Debug.Log(EClass._map.deadCharas.Find((Chara c) => c.uid == p.Key));
				}
			}
		});
	}

	public void Test_Grow()
	{
		EClass._map.ForeachCell(delegate(Cell c)
		{
			if (c.sourceObj.HasGrowth)
			{
				c.TryGrow(null);
			}
		});
		foreach (Thing thing in EClass._map.things.Copy<Thing>())
		{
			TraitSeed traitSeed = thing.trait as TraitSeed;
			if (traitSeed != null)
			{
				traitSeed.TrySprout(true, false, null);
			}
		}
	}

	public void Test_GodTalk()
	{
		foreach (Religion religion in EClass.game.religions.dictAll.Values)
		{
			religion.Talk("test", null, null);
		}
	}

	public void COM_Teleport()
	{
		for (int i = 0; i < 10000; i++)
		{
			Point point = EClass.pc.pos.Copy();
			point.x += EScriptable.rnd(60) - EScriptable.rnd(60);
			point.z += EScriptable.rnd(60) - EScriptable.rnd(60);
			if (point.IsValid && !point.cell.blocked && point.HasFloor)
			{
				EClass.pc.Teleport(point, false, false);
				return;
			}
		}
	}

	public void COM_PlayStartDrama()
	{
		EClass.ui.CloseLayers();
		EClass.game.world.date.hour = 2;
		EClass.scene.screenElin.RefreshAll();
		LayerDrama.ActivateMain("mono", "1-2", null, null, "");
	}

	public void Test_Filter()
	{
		this.bilinear = !this.bilinear;
		MeshPass[] passes = EClass.scene.passes;
		for (int i = 0; i < passes.Length; i++)
		{
			passes[i].mat.GetTexture("_MainTex").filterMode = (this.bilinear ? FilterMode.Bilinear : FilterMode.Point);
		}
	}

	public void LOG_Spatials()
	{
		foreach (Spatial spatial in EClass.world.region.children)
		{
			Debug.Log(string.Concat(new string[]
			{
				spatial.uid.ToString(),
				"/",
				spatial.Name,
				"/",
				spatial.mainFaction.name,
				"/",
				(spatial == EClass.player.zone).ToString()
			}));
		}
	}

	public void Test_SiegeGuard()
	{
		if (EClass._zone.events.GetEvent<ZoneEventSiegeGuard>() == null)
		{
			EClass._zone.events.Add(new ZoneEventSiegeGuard(), false);
			return;
		}
		EClass._zone.events.Remove<ZoneEventSiegeGuard>();
	}

	public void Test_Siege()
	{
		if (EClass._zone.events.GetEvent<ZoneEventSiege>() == null)
		{
			EClass._zone.events.Add(new ZoneEventSiege(), false);
			return;
		}
		EClass._zone.events.Remove<ZoneEventSiege>();
	}

	public void COM_MaxConstruction()
	{
		if (EClass.pc.homeBranch == null)
		{
			return;
		}
		EClass.pc.homeBranch.owner.elements.ModBase(2003, 50);
	}

	public static bool CheatEnabled()
	{
		return EClass.debug.enable || EClass.game.config.cheat;
	}

	public static string EnableCheat
	{
		get
		{
			return "Enable cheat by typing 'Cheat'";
		}
	}

	[ConsoleCommand("")]
	public static string Cheat()
	{
		EClass.game.config.cheat = true;
		return "Cheat Enabled";
	}

	[ConsoleCommand("")]
	public static string Fix_RemoveDuplicateUnique()
	{
		if (EClass.Branch == null)
		{
			return "No Branch";
		}
		EClass.Branch.members.ForeachReverse(delegate(Chara c)
		{
			if ((c.id == "kettle" || c.id == "quru") && (from c2 in EClass.Branch.members
			where c2.id == c.id
			select c2).Count<Chara>() >= 2)
			{
				EClass.Branch.RemoveMemeber(c);
				c.Destroy();
			}
		});
		return "Fixed!";
	}

	[ConsoleCommand("")]
	public static string Fix_EtherDisease()
	{
		EClass.pc.ModCorruption(-100000);
		Chara chara = CharaGen.Create("chara", -1);
		chara.ChangeRace(EClass.pc.race.id);
		chara.ChangeJob(EClass.pc.job.id);
		string text = "";
		foreach (Element element in chara.elements.dict.Values)
		{
			if (!(element.source.category != "attribute"))
			{
				Element orCreateElement = EClass.pc.elements.GetOrCreateElement(element.id);
				if (element.vBase > orCreateElement.vBase)
				{
					text = string.Concat(new string[]
					{
						text,
						"Fixing Base Value:",
						orCreateElement.Name,
						" Before:",
						orCreateElement.vBase.ToString(),
						" Now:",
						(element.vBase + 1).ToString(),
						Environment.NewLine
					});
					EClass.pc.elements.ModBase(orCreateElement.id, element.vBase - orCreateElement.vBase + 1);
				}
			}
		}
		return text + "Fixed!";
	}

	[ConsoleCommand("")]
	public static string ListChara()
	{
		string text = "";
		foreach (SourceChara.Row row in EClass.sources.charas.rows)
		{
			text = text + row.id + "\n";
		}
		return text;
	}

	[ConsoleCommand("")]
	public static string ListThing()
	{
		string text = "";
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			text = text + row.id + "\n";
		}
		return text;
	}

	[ConsoleCommand("")]
	public static string SetElement(string alias, int value, int potential = 100)
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		SourceElement.Row row = EClass.sources.elements.alias.TryGetValue(alias, null);
		if (row == null)
		{
			return "Element not found.";
		}
		EClass.pc.elements.SetBase(row.id, value, potential);
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string Spawn(string id, int num = 1, string aliasMat = "")
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		if (EClass.sources.things.map.ContainsKey(id))
		{
			Thing thing = ThingGen.Create(id, -1, -1).SetNum(num);
			if (!aliasMat.IsEmpty())
			{
				thing.ChangeMaterial(aliasMat);
			}
			EClass._zone.AddCard(thing, EClass.pc.pos);
			return "Spawned " + thing.Name;
		}
		if (EClass.sources.charas.map.ContainsKey(id))
		{
			Chara chara = CharaGen.Create(id, -1);
			EClass._zone.AddCard(chara, EClass.pc.pos);
			return "Spawned " + chara.Name;
		}
		return "'" + id + "' does not exist in the database.";
	}

	[ConsoleCommand("")]
	public static string TestSpawn(int lv, int num)
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		foreach (Chara chara in (from c in EClass._map.charas
		where c.HasEditorTag(EditorTag.SpawnTest)
		select c).ToList<Chara>())
		{
			chara.Destroy();
		}
		for (int i = 0; i < num; i++)
		{
			EClass._zone.SpawnMob(EClass.pc.pos.GetNearestPoint(false, false, false, true), new SpawnSetting
			{
				filterLv = lv
			}).AddEditorTag(EditorTag.SpawnTest);
		}
		return "Spawned.";
	}

	[ConsoleCommand("")]
	public static string ResetPetUpgrades()
	{
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara.IsPCFaction)
			{
				chara.ResetUpgrade();
			}
		}
		return "Not Implemented.";
	}

	[ConsoleCommand("")]
	public static string GodMode()
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		EClass.pc.Revive(null, false);
		EClass.pc.hp = EClass.pc.MaxHP;
		return "I'm God!";
	}

	[ConsoleCommand("")]
	public static string ModFame(int amount)
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		EClass.player.ModFame(amount);
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string ModKarma(int amount)
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		EClass.player.ModKarma(amount);
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string ModContribution(int amount)
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		Guild currentGuild = Guild.GetCurrentGuild();
		if (currentGuild != null)
		{
			currentGuild.AddContribution(amount);
		}
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string FlyMode()
	{
		EClass.pc.AddCondition<ConLevitate>(100, false);
		return "I can fly!";
	}

	[ConsoleCommand("")]
	public static string RemoveDemitas()
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara.id == "demitas" && chara.currentZone == EClass._zone)
			{
				list.Add(chara);
			}
		}
		if (list.Count > 1)
		{
			Chara chara2 = list[1];
			chara2.homeBranch.BanishMember(chara2, false);
			chara2.Die(null, null, AttackSource.None);
			return "Demitas Removed!";
		}
		return "Not enough Demitas!";
	}

	[ConsoleCommand("")]
	public static string ChangeRace(string id = "?")
	{
		if (EClass.sources.races.map.ContainsKey(id))
		{
			Thing thing = null;
			foreach (BodySlot bodySlot in EClass.pc.body.slots)
			{
				if (bodySlot.thing != null)
				{
					if (bodySlot.thing.blessedState <= BlessedState.Cursed)
					{
						bodySlot.thing.blessedState = BlessedState.Normal;
					}
					if (bodySlot.thing.trait is TraitToolBelt)
					{
						thing = bodySlot.thing;
					}
					EClass.pc.body.Unequip(bodySlot, true);
				}
			}
			EClass.pc.body.RemoveBodyPart(45);
			EClass.pc.body.RemoveBodyPart(44);
			EClass.pc.ChangeRace(id);
			if (EClass.ui.IsInventoryOpen)
			{
				EClass.ui.ToggleInventory(false);
			}
			EClass.pc.body.AddBodyPart(45, null);
			EClass.pc.body.AddBodyPart(44, null);
			EClass.pc.body.Equip(thing, null, true);
			return "Done.";
		}
		string text = "";
		foreach (SourceRace.Row row in EClass.sources.races.rows)
		{
			text = string.Concat(new string[]
			{
				text,
				row.id,
				" ",
				row.GetName(),
				Environment.NewLine
			});
		}
		return text;
	}

	[ConsoleCommand("")]
	public static string ChangeJob(string id = "?")
	{
		if (EClass.sources.jobs.map.ContainsKey(id))
		{
			EClass.pc.ChangeJob(id);
			EClass.player.RefreshDomain();
			return "Done.";
		}
		string text = "";
		foreach (SourceJob.Row row in EClass.sources.jobs.rows)
		{
			text = string.Concat(new string[]
			{
				text,
				row.id,
				" ",
				row.GetName(),
				Environment.NewLine
			});
		}
		return text;
	}

	[ConsoleCommand("")]
	public static string FirstAdventurer()
	{
		DateTime dateTime = new DateTime(2024, 11, 3, 6, 0, 0);
		DateTimeOffset left;
		if (Application.isEditor)
		{
			left = new DateTimeOffset(2024, 11, 6, 1, 9, 0, default(TimeSpan));
		}
		else if (SteamAPI.IsSteamRunning())
		{
			left = DateTimeOffset.FromUnixTimeSeconds((long)((ulong)SteamApps.GetEarliestPurchaseUnixTime(EClass.core.steam.steamworks.settings.applicationId)));
		}
		else
		{
			left = new DateTimeOffset(9999, 1, 1, 1, 9, 0, default(TimeSpan));
		}
		left = left.ToOffset(new TimeSpan(9, 0, 0));
		string text = (left < dateTime) ? "Valid: " : "Invalid: ";
		text = string.Concat(new string[]
		{
			text,
			"Arrived North Tyris on ",
			left.Year.ToString(),
			" ",
			left.Month.ToString(),
			"/",
			left.Day.ToString(),
			" ",
			left.Hour.ToString(),
			":",
			left.Minute.ToString()
		});
		return string.Concat(new string[]
		{
			text,
			"  Eligible by ",
			dateTime.Year.ToString(),
			" ",
			dateTime.Month.ToString(),
			"/",
			dateTime.Day.ToString(),
			" ",
			dateTime.Hour.ToString(),
			":",
			dateTime.Minute.ToString()
		});
	}

	[ConsoleCommand("")]
	public static string RegenerateNames()
	{
		EClass.core.mods.InitLang();
		NameGen.list = null;
		AliasGen.list = null;
		NameGen.Init();
		AliasGen.Init();
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPC)
			{
				if (chara.source.name == "*r")
				{
					chara.c_altName = NameGen.getRandomName();
				}
				if (!chara._alias.IsEmpty())
				{
					chara._alias = AliasGen.GetRandomAlias();
				}
			}
		}
		return "Done!";
	}

	[ConsoleCommand("")]
	public static string AllRecipe(bool forget = false)
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		if (forget)
		{
			EClass.player.recipes.knownRecipes.Clear();
		}
		else
		{
			EClass.debug.AddAllRecipes();
		}
		return "Done!";
	}

	[ConsoleCommand("")]
	public static string LastWish(string name)
	{
		string text = "";
		foreach (SourceBacker.Row row in EClass.sources.backers.rows)
		{
			if (row.name.Contains(name) || row.id.ToString() == name)
			{
				text = string.Concat(new string[]
				{
					text,
					row.name,
					" valid?:",
					row.valid.ToString(),
					" lang:",
					row.lang,
					" type:",
					row.type.ToString(),
					" destroyed:",
					EClass.player.doneBackers.Contains(row.id).ToString(),
					" loc:",
					row.loc.IsEmpty() ? "random" : row.loc,
					Environment.NewLine
				});
			}
		}
		if (text == "")
		{
			return "Not Found";
		}
		text += Resources.Load<TextAsset>("logo2").text;
		return text;
	}

	[ConsoleCommand("")]
	public static string ClearLastWishFlag()
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		EClass.player.doneBackers.Clear();
		return Resources.Load<TextAsset>("logo2").text + Environment.NewLine + "Done!";
	}

	[ConsoleCommand("")]
	public static string Resource()
	{
		if (!CoreDebug.CheatEnabled())
		{
			return CoreDebug.EnableCheat;
		}
		EClass.debug.SpawnCheatContainer();
		return "Resources spawned.";
	}

	public void Log(object o)
	{
		Debug.Log(o);
	}

	public void NextBGM()
	{
		if (!SoundManager.current)
		{
			return;
		}
		SoundManager.current.NextBGM();
	}

	public void ToggleRevealMap()
	{
		this.revealMap = !this.revealMap;
		if (this.revealMap)
		{
			EClass._map.RevealAll(true);
		}
		SE.ClickGeneral();
	}

	public void LoadBroadcast()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Data/Text/broadcast");
		this.blines = textAsset.text.Split('-', StringSplitOptions.None);
	}

	public void BroadcastNext()
	{
		this.LoadBroadcast();
		if (Input.GetKey(KeyCode.LeftControl))
		{
			this.bidx--;
		}
		this.bidx = Mathf.Clamp(this.bidx, 0, this.blines.Length - 1);
		string text = this.blines[this.bidx];
		this.bidx += (Input.GetKey(KeyCode.LeftShift) ? -1 : 1);
		EClass.pc.SayRaw(text.TrimNewLines(), null, null);
	}

	[CompilerGenerated]
	internal static void <QuickStart>g__AddParty|102_0(string id, ref CoreDebug.<>c__DisplayClass102_0 A_1)
	{
		Chara chara = CharaGen.Create(id, -1);
		chara.SetFaction(EClass.Home);
		chara.SetHomeZone(A_1.homeZone);
		if (chara.currentZone != EClass._zone)
		{
			EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(false, false, true, false));
		}
		EClass.pc.party.AddMemeber(chara);
	}

	[CompilerGenerated]
	internal static Thing <QuickStart>g__AddHotbar|102_1(string id, ref CoreDebug.<>c__DisplayClass102_0 A_1)
	{
		Thing thing = EClass.pc.AddThing(ThingGen.Create(id, -1, -1), true, -1, -1);
		thing.invX = A_1.idx;
		thing.invY = 1;
		int idx = A_1.idx;
		A_1.idx = idx + 1;
		return thing;
	}

	[CompilerGenerated]
	internal static Thing <QuickStart>g__AddAbility|102_2(string id)
	{
		Thing thing = EClass.pc.AddThing(ThingGen.Create("catalyst", -1, -1), true, -1, -1);
		thing.c_idAbility = id;
		return thing;
	}

	[CompilerGenerated]
	private void <InitDebugCommands>g__Add|113_0(int cat, string id, Action action)
	{
		CoreDebug.DebugCommand item = new CoreDebug.DebugCommand
		{
			name = id,
			action = action,
			cat = cat
		};
		this.commands.Add(item);
	}

	[Header("Quick Start Setup")]
	public CoreDebug.StartScene startScene;

	public CoreDebug.StartLoadout startLoadout;

	public CoreDebug.StartParty startParty;

	public int startHour;

	public CoreDebug.StartSetting startSetting;

	public string startZone;

	[Header("System(Release)")]
	public bool showSceneSelector;

	[Header("System(Release)")]
	public bool skipModSync;

	public Lang.LangCode langCode;

	[Header("System")]
	public bool ignorePool;

	[Header("System")]
	public bool resetPlayerConfig;

	public bool dontUseThread;

	public bool useNewConfig;

	public bool ignoreAutoSave;

	public bool alwaysResetWindow;

	public bool validateData;

	public bool dontCompressSave;

	public bool skipMod;

	public string command;

	[Header("Input")]
	public bool debugInput;

	public bool keypadDebug;

	public CoreDebug.DebugHotkey debugHotkeys;

	[Header("Util")]
	public int advanceMin;

	public bool testLOS;

	public bool testLOS2;

	public bool debugProps;

	public bool revealMap;

	public bool debugHoard;

	public bool revealInner;

	public bool ignorePopup;

	public bool skipEvent;

	public bool skipNerun;

	public bool showTone;

	public bool showExtra;

	public bool test;

	[Header("Game")]
	public bool godMode;

	public bool randomResource;

	public bool debugScatter;

	public bool _godBuild;

	public bool godCraft;

	public bool godFood;

	public bool ignoreBuildRule;

	public bool ignoreWeight;

	public bool autoIdentify;

	public bool allAbility;

	public bool allSkill;

	public bool allHomeSkill;

	public bool allArt;

	public bool allBGM;

	public bool ignoreEncounter;

	public bool returnAnywhere;

	public bool instaReturn;

	public bool travelAnywhere;

	public bool hidePCItemsInBuild;

	public bool autoAdvanceQuest;

	public bool enableMapPieceEditor;

	public bool testThingQuality;

	public int numResource;

	[Header("Game(Specific)")]
	public bool unlimitedInterest;

	public bool inviteAnytime;

	public bool marryAnytime;

	public bool showFav;

	public bool alwaysFavFood;

	public bool maxQuests;

	[Header("Progress")]
	public bool allRecipe;

	public bool allMenu;

	public bool allPolicy;

	public bool allStory;

	public bool skipInitialQuest;

	[Header("Log")]
	public bool logAdv;

	[Header("Log")]
	public bool logCombat;

	public bool logDice;

	[Header("Once")]
	public bool validatePref;

	[Header("Test")]
	public int param1;

	[Header("Test")]
	public int param2;

	public CoreDebug.MatColorTest matColorTest;

	public bool boradcast;

	public bool testFixedColor;

	public Color32 fixedColor;

	private readonly List<int[]> resolutions = new List<int[]>
	{
		new int[]
		{
			1920,
			1080
		},
		new int[]
		{
			1366,
			768
		},
		new int[]
		{
			1280,
			720
		}
	};

	private Card animeDest;

	[NonSerialized]
	private int bidx;

	private bool bilinear;

	[NonSerialized]
	private string[] blines;

	public List<CoreDebug.DebugCommand> commands = new List<CoreDebug.DebugCommand>();

	[NonSerialized]
	public bool enable;

	public Fov fov = new Fov();

	private int indexResolution;

	private AnimeID lastAnime;

	private Emo lastEmo;

	private bool naked;

	[NonSerialized]
	private bool runtimeGodBuild;

	private int ttt;

	[Serializable]
	public class StartSetting
	{
		public string race;

		public string job;

		public int lv;
	}

	public enum DebugHotkey
	{
		None,
		Block,
		Item,
		Decal,
		Act,
		Test,
		Anime
	}

	public enum StartLoadout
	{
		New,
		AllItem,
		FewItem
	}

	public enum StartParty
	{
		None,
		Farris,
		Full
	}

	public enum StartScene
	{
		Title,
		Zone,
		Home,
		Home_Cave,
		Story_Test,
		MeetFarris,
		NymelleBoss,
		AfterNymelle,
		Melilith,
		Tefra
	}

	[Serializable]
	public class MatColorTest
	{
		public void Update()
		{
			foreach (SourceMaterial.Row row in EClass.sources.materials.rows)
			{
				if (row != MATERIAL.sourceWaterSea)
				{
					MatColors matColors = this.profile.matColors.TryGetValue(row.alias, null);
					row.matColor = matColors.main;
					row.altColor = matColors.alt;
				}
			}
		}

		public bool enable;

		public Color color;

		public int matColor;

		public ColorProfile profile;
	}

	public class DebugCommand
	{
		public Action action;

		public int cat;

		public string name;
	}
}
