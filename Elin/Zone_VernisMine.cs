using System;

public class Zone_VernisMine : Zone_Dungeon
{
	public bool IsBossLv
	{
		get
		{
			return base.lv == -8;
		}
	}

	public override bool LockExit
	{
		get
		{
			return base.lv == -1 && EClass.game.quests.GetPhase<QuestVernis>() < 7;
		}
	}

	public override float OreChance
	{
		get
		{
			return 4f;
		}
	}

	public override string idExport
	{
		get
		{
			if (base.lv != -8)
			{
				return base.idExport;
			}
			return "vernis_mine_boss";
		}
	}

	public override string GetDungenID()
	{
		return "DungeonMine";
	}

	public override void OnGenerateMap()
	{
		if (this.IsBossLv)
		{
			return;
		}
		base.PlaceRail(Zone_Dungeon.RailType.Mine);
		EClass._map.ForeachCell(delegate(Cell c)
		{
			if (EClass.rnd(5) == 0 || c._block == 0 || c.HasObj || c.isSurrounded || c.hasDoor)
			{
				return;
			}
			c.GetSharedPoint().SetObj(24, 1, EClass.rnd(3));
		});
		base.OnGenerateMap();
	}

	public const int LvBoss = -8;
}
