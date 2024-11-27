using System;

public class Zone_Nymelle : Zone_Dungeon
{
	public override string idExport
	{
		get
		{
			if (base.lv == this.LvBoss)
			{
				return "nymelle_boss";
			}
			if (base.lv != this.LvCrystal)
			{
				return base.source.id;
			}
			return "nymelle_crystal";
		}
	}

	public int LvBoss
	{
		get
		{
			return -5;
		}
	}

	public int LvCrystal
	{
		get
		{
			return -6;
		}
	}

	public bool IsBossLv
	{
		get
		{
			return base.lv == this.LvBoss;
		}
	}

	public bool IsCrystalLv
	{
		get
		{
			return base.lv == this.LvCrystal;
		}
	}

	public override bool LockExit
	{
		get
		{
			return (base.lv == -2 && EClass.game.quests.GetPhase<QuestExploration>() < 1) || (base.lv == this.LvBoss + 1 && EClass.game.quests.GetPhase<QuestExploration>() < 2) || (base.lv == this.LvBoss && EClass.game.quests.GetPhase<QuestExploration>() < 3);
		}
	}

	public override bool UseFog
	{
		get
		{
			return !this.IsBossLv && !this.IsCrystalLv;
		}
	}

	public override bool RevealRoom
	{
		get
		{
			return this.IsBossLv || this.IsCrystalLv;
		}
	}

	public override float PrespawnRate
	{
		get
		{
			if (!this.IsBossLv && !this.IsCrystalLv)
			{
				return base.PrespawnRate;
			}
			return 0f;
		}
	}

	public override string GetNewZoneID(int level)
	{
		if (level == this.LvBoss)
		{
			return "nymelle_boss";
		}
		if (level == this.LvCrystal)
		{
			return "nymelle_crystal";
		}
		return base.GetNewZoneID(level);
	}

	public override void OnBeforeSimulate()
	{
		if (base.visitCount == 0)
		{
			if (this.IsBossLv)
			{
				EClass._zone.AddChara("isygarad", 40, 37);
				SoundManager.ForceBGM();
				LayerDrama.ActivateMain("mono", "nymelle_boss", null, null, "");
			}
			if (this.IsCrystalLv)
			{
				Chara chara = EClass.game.cards.globalCharas.Find("fiama");
				chara.MoveHome(EClass._zone, 43, 67);
				chara.AddEditorTag(EditorTag.AINoMove);
				return;
			}
			if (base.lv == -2 && EClass.game.cards.globalCharas.Find("farris") == null)
			{
				Chara chara2 = CharaGen.Create("farris", -1);
				chara2.SetGlobal();
				Thing thing = EClass._map.props.installed.Find<TraitStairsLocked>();
				EClass._zone.AddCard(chara2, thing.pos.x, thing.pos.z);
				chara2.AddEditorTag(EditorTag.AINoMove);
				chara2.AddEditorTag(EditorTag.Invulnerable);
			}
		}
	}
}
