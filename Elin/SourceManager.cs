using System;
using System.Collections.Generic;
using System.Reflection;

public class SourceManager : EMono
{
	public ExcelData dataGodTalk
	{
		get
		{
			ExcelData result;
			if ((result = this._dataGodTalk) == null)
			{
				result = (this._dataGodTalk = new ExcelData(Lang.setting.dir + "Data/god_talk.xlsx", 3));
			}
			return result;
		}
	}

	public void InitLang()
	{
		this.langGeneral.Init();
		this.langGame.Init();
		this.langList.Init();
		this.langNote.Init();
		this.langWord.Init();
		Lang.General = this.langGeneral;
		Lang.Game = this.langGame;
		Lang.List = this.langList;
		Lang.Note = this.langNote;
		WordGen.source = this.langWord;
	}

	public void OnChangeLang()
	{
		this._dataGodTalk = null;
	}

	public void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		this.list.Clear();
		foreach (FieldInfo fieldInfo in base.GetType().GetFields())
		{
			if (typeof(SourceData).IsAssignableFrom(fieldInfo.FieldType))
			{
				this.list.Add((SourceData)fieldInfo.GetValue(this));
			}
		}
		this.elements.Init();
		this.materials.Init();
		this.charas.Init();
		this.things.Init();
		this.thingV.Init();
		this.foods.Init();
		this.cards.Init();
		this.checks.Init();
		this.races.Init();
		this.persons.Init();
		this.categories.Init();
		this.spawnLists.Init();
		this.religions.Init();
		this.factions.Init();
		this.jobs.Init();
		this.hobbies.Init();
		this.floors.Init();
		this.blocks.Init();
		this.cellEffects.Init();
		this.objs.Init();
		this.stats.Init();
		this.areas.Init();
		this.zones.Init();
		this.zoneAffixes.Init();
		this.researches.Init();
		this.homeResources.Init();
		this.globalTiles.Init();
		this.floors.OnAfterInit();
		this.quests.Init();
		this.charaText.Init();
		this.calc.Init();
		this.recipes.Init();
		this.backers.Init();
		this.tactics.Init();
		this.keyItems.Init();
		ACT.Init();
		TimeTable.Init();
		Element.ListAttackElements.Clear();
		for (int j = 910; j < 925; j++)
		{
			Element.ListAttackElements.Add(EMono.sources.elements.map[j]);
		}
	}

	public void ExportSourceTexts(string path)
	{
		foreach (SourceData sourceData in this.list)
		{
			sourceData.ExportTexts(path, false);
		}
	}

	public void UpdateSourceTexts(string path)
	{
		foreach (SourceData sourceData in this.list)
		{
			sourceData.ExportTexts(path, true);
		}
	}

	public void ImportSourceTexts()
	{
		foreach (SourceData sourceData in this.list)
		{
			if (sourceData is SourceThingV)
			{
				this.things.ImportTexts(sourceData.nameSheet);
			}
			else
			{
				sourceData.ImportTexts(null);
			}
		}
	}

	public void ValidateLang()
	{
		Log.system = "";
		foreach (SourceData sourceData in this.list)
		{
			sourceData.ValidateLang();
		}
		string text = Lang.setting.dir + "validation.txt";
		IO.SaveText(text, Log.system);
		Util.Run(text);
	}

	public LangGeneral langGeneral;

	public LangList langList;

	public LangGame langGame;

	public LangWord langWord;

	public LangNote langNote;

	public SourceCard cards = new SourceCard();

	public SourceChara charas;

	public SourceCharaText charaText;

	public SourceTactics tactics;

	public SourcePerson persons;

	public SourceThing things;

	public SourceElement elements;

	public SourceCalc calc;

	public SourceCheck checks;

	public SourceRace races;

	public SourceCategory categories;

	public SourceMaterial materials;

	public SourceSpawnList spawnLists;

	public SourceBlock blocks;

	public SourceFloor floors;

	public SourceCellEffect cellEffects;

	public SourceObj objs;

	public SourceFaction factions;

	public SourceReligion religions;

	public SourceJob jobs;

	public SourceHobby hobbies;

	public SourceQuest quests;

	public SourceStat stats;

	public SourceArea areas;

	public SourceZone zones;

	public SourceZoneAffix zoneAffixes;

	public SourceResearch researches;

	public SourceHomeResource homeResources;

	public SourceCollectible collectibles;

	public SourceGlobalTile globalTiles;

	public SourceThingV thingV;

	public SourceFood foods;

	public SourceRecipe recipes;

	public SourceBacker backers;

	public SourceAsset asset;

	public SourceKeyItem keyItems;

	private ExcelData _dataGodTalk;

	private List<SourceData> list = new List<SourceData>();

	public bool initialized;
}
