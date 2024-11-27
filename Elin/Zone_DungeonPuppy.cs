using System;

public class Zone_DungeonPuppy : Zone_Dungeon
{
	public override bool WillAutoSave
	{
		get
		{
			return false;
		}
	}

	public int LvPoppy
	{
		get
		{
			return -2;
		}
	}

	public override bool RegenerateOnEnter
	{
		get
		{
			return true;
		}
	}

	public override bool LockExit
	{
		get
		{
			return base.lv <= this.LvPoppy;
		}
	}

	public override float BigDaddyChance
	{
		get
		{
			return 0f;
		}
	}

	public override float ShrineChance
	{
		get
		{
			return 0f;
		}
	}

	public override int ExpireDays
	{
		get
		{
			return 1;
		}
	}

	public override void OnGenerateMap()
	{
		if (base.lv <= this.LvPoppy)
		{
			Quest quest = EClass.game.quests.Get("puppy");
			if (quest != null && quest.phase == 0)
			{
				Chara chara = EClass.game.cards.globalCharas.Find("poppy") ?? CharaGen.Create("poppy", -1);
				chara.AddEditorTag(EditorTag.Invulnerable);
				EClass._zone.AddCard(chara, EClass._map.bounds.GetRandomSpawnPos());
			}
		}
		base.OnGenerateMap();
	}

	public override void OnBeforeSimulate()
	{
		if (base.visitCount == 0)
		{
			Tutorial.Reserve("eq", null);
		}
	}
}
