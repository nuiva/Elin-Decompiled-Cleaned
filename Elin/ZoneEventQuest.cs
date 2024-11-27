using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ZoneEventQuest : ZoneEvent
{
	public Quest quest
	{
		get
		{
			return EClass.game.quests.Get(this.uidQuest);
		}
	}

	public virtual bool WarnBoss
	{
		get
		{
			return true;
		}
	}

	public virtual bool CountEnemy
	{
		get
		{
			return false;
		}
	}

	public virtual int TimeLimit
	{
		get
		{
			return -1;
		}
	}

	public void Spawn(int num = 1)
	{
		for (int i = 0; i < num; i++)
		{
			Point nearestPoint = EClass._map.bounds.GetRandomEdge(3).GetNearestPoint(false, false, true, false);
			Chara chara = EClass._zone.SpawnMob(nearestPoint, SpawnSetting.DefenseEnemy(EClass._zone.DangerLv));
			chara.hostility = (chara.c_originalHostility = Hostility.Enemy);
			if (this.CountEnemy)
			{
				this.enemies.Add(chara.uid);
			}
		}
	}

	public void SpawnBoss(bool evolve = false)
	{
		Point nearestPoint = EClass._map.bounds.GetRandomEdge(3).GetNearestPoint(false, false, true, false);
		Chara chara = evolve ? EClass._zone.TryGenerateEvolved(true, nearestPoint) : EClass._zone.SpawnMob(nearestPoint, SpawnSetting.Boss(EClass._zone.DangerLv, -1));
		chara.hostility = (chara.c_originalHostility = Hostility.Enemy);
		if (this.CountEnemy)
		{
			this.enemies.Add(chara.uid);
		}
		if (this.WarnBoss)
		{
			Msg.Say("defense_boss", chara.Name, null, null, null);
			EClass.game.Pause(null);
		}
	}

	public void AggroEnemy(int chance = 100)
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCFactionOrMinion && !chara.IsInCombat && EClass.rnd(100) <= chance)
			{
				chara.SetEnemy(EClass.pc.party.members.RandomItem<Chara>());
				chara.SetAIAggro();
			}
		}
	}

	public sealed override void OnTickRound()
	{
		this._OnTickRound();
		if (this.TimeLimit != -1 && this.minElapsed >= this.TimeLimit)
		{
			EClass._zone.instance.status = this.OnReachTimeLimit();
			EClass.game.Pause(delegate
			{
				EClass.pc.MoveZone(EClass._zone.parent as Zone, ZoneTransition.EnterState.Auto);
			});
		}
	}

	public virtual void _OnTickRound()
	{
	}

	public virtual ZoneInstance.Status OnReachTimeLimit()
	{
		return ZoneInstance.Status.Running;
	}

	[JsonProperty]
	public int uidQuest;

	[JsonProperty]
	public List<int> enemies = new List<int>();
}
