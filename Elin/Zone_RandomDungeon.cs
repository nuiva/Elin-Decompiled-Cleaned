using System;

public class Zone_RandomDungeon : Zone_Dungeon
{
	public override bool AlwaysLowblock
	{
		get
		{
			return true;
		}
	}

	public override bool IsNefia
	{
		get
		{
			return true;
		}
	}

	public override bool AddPrefix
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
			return false;
		}
	}

	public override int MinLv
	{
		get
		{
			return this.LvBoss;
		}
	}

	public override bool ScaleMonsterLevel
	{
		get
		{
			return (EClass.player.flags.KilledBossInVoid && this.DangerLv >= 50) || this.DangerLv >= 200;
		}
	}

	public override string IDPlayList
	{
		get
		{
			if (base.lv != this.LvBoss)
			{
				return base.IDPlayList;
			}
			return "Dungeon_Boss";
		}
	}

	public virtual int LvBoss
	{
		get
		{
			Rand.SetSeed(base.GetTopZone().uid);
			int result = -2 - EClass.rnd(4);
			Rand.SetSeed(-1);
			return result;
		}
	}

	public override void OnGenerateMap()
	{
		if (base.lv == this.LvBoss)
		{
			base.Boss = base.SpawnMob(null, SpawnSetting.Boss(this.DangerLv, this.DangerLv));
			base.Boss.hostility = (base.Boss.c_originalHostility = Hostility.Enemy);
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.IsHostile())
				{
					chara.enemy = EClass.pc.party.members.RandomItem<Chara>();
				}
			}
		}
		base.OnGenerateMap();
	}

	public override string TextWidgetDate
	{
		get
		{
			if (base.Boss == null)
			{
				return "";
			}
			return "bossLevel".lang(base.Boss.Name, null, null, null, null);
		}
	}
}
