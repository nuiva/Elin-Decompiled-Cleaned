using System;

public class Zone_CursedManor : Zone_Civilized
{
	public override bool LockExit
	{
		get
		{
			return !EClass.game.quests.IsStarted<QuestCursedManor>();
		}
	}

	public override FlockController.SpawnType FlockType
	{
		get
		{
			return FlockController.SpawnType.Bat;
		}
	}

	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Right;
		}
	}

	public override string GetNewZoneID(int level)
	{
		if (level <= -2)
		{
			return "cursed_manor_dungeon";
		}
		return base.GetNewZoneID(level);
	}

	public override void OnGenerateMap()
	{
		base.OnGenerateMap();
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCParty && chara.IsMinion && chara.id == "sister_undead")
			{
				chara.AddCondition<ConSuspend>(100, true);
			}
		}
	}
}
