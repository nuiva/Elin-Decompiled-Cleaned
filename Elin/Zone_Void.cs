using System;

public class Zone_Void : Zone_Dungeon
{
	public bool IsBossLV
	{
		get
		{
			return base.lv % 5 == 0 && base.GetTopZone().GetDeepestLv() == base.lv;
		}
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

	public override int MinLv
	{
		get
		{
			return -10000;
		}
	}

	public override bool ScaleMonsterLevel
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
			return this.IsBossLV;
		}
	}

	public override bool CanUnlockExit
	{
		get
		{
			return base.Boss == null || base.Boss.isDead;
		}
	}

	public override string IDPlayList
	{
		get
		{
			if (!this.IsBossLV)
			{
				return base.IDPlayList;
			}
			return "Dungeon_Boss";
		}
	}

	public override void OnGenerateMap()
	{
		base._dangerLv = 50;
		if (this.IsBossLV)
		{
			base.Boss = base.SpawnMob(null, SpawnSetting.Boss(this.DangerLv, -1));
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
}
