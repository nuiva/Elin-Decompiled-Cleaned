using System;
using Newtonsoft.Json;
using UnityEngine;

public class ZoneEventDefenseGame : ZoneEventQuest
{
	public ZoneInstanceRandomQuest instance
	{
		get
		{
			return EClass._zone.instance as ZoneInstanceRandomQuest;
		}
	}

	public bool CanRetreat
	{
		get
		{
			return this.rounds >= 100 && this.hornTimer <= 0;
		}
	}

	public bool CanCallAlly
	{
		get
		{
			return this.allyCall == 0 && this.rounds >= 50;
		}
	}

	public override string TextWidgetDate
	{
		get
		{
			return "defenseWave".lang(this.wave.ToString() ?? "", this.kills.ToString() ?? "", null, null, null) + ((this.instance != null && this.retreated) ? "defenseRetreating" : (this.CanRetreat ? "defenseRetreat" : "")).lang() + (this.CanCallAlly ? "defenseAlly" : "").lang();
		}
	}

	public virtual Chara CreateChara()
	{
		return CharaGen.CreateFromFilter("c_wilds", -1, -1);
	}

	public override void OnVisit()
	{
		if (EClass.game.isLoading)
		{
			QuestDefenseGame.lastWave = this.wave;
			QuestDefenseGame.bonus = this.bonus;
			return;
		}
		EClass._zone._dangerLv = 5;
		Point nearestPoint = EClass._map.GetCenterPos().GetNearestPoint(false, false, true, false);
		EClass._zone.AddCard(ThingGen.Create("stone_defense", -1, -1), nearestPoint).Install().isNPCProperty = true;
		EClass._zone.AddCard(ThingGen.Create("core_defense", -1, -1), nearestPoint).Install().isNPCProperty = false;
		EClass._zone.SetBGM(107, true);
		Msg.Say("defense_start");
		this.NextWave(0);
	}

	public void NextWave(int add = 0)
	{
		this.wave++;
		this.turns = 0;
		EClass._zone._dangerLv += ((this.wave >= 20) ? 10 : 5);
		SE.Play("warhorn");
		Msg.Say("defense_wave", this.wave.ToString() ?? "", EClass._zone.DangerLv.ToString() ?? "", null, null);
		base.Spawn(2 + base.quest.difficulty + add);
		base.AggroEnemy(100);
	}

	public override void _OnTickRound()
	{
		QuestDefenseGame.lastWave = this.wave;
		QuestDefenseGame.bonus = this.bonus;
		Debug.Log(string.Concat(new string[]
		{
			"wave:",
			this.wave.ToString(),
			" turns:",
			this.turns.ToString(),
			" rounds:",
			this.rounds.ToString()
		}));
		this.turns++;
		if (this.hornTimer > 0)
		{
			this.hornTimer--;
		}
		if (this.turns <= 3 + base.quest.difficulty)
		{
			base.Spawn(1);
		}
		if (this.turns == 10 && this.wave % 5 == 0)
		{
			Rand.SetSeed(this.wave + base.quest.uid);
			base.SpawnBoss(((this.wave < 10) ? 0 : (this.wave * 2)) > EClass.rnd(100));
			Rand.SetSeed(-1);
		}
		if (this.turns == 20)
		{
			this.NextWave(0);
		}
		base.AggroEnemy(100);
		if (this.allyTimer > 0)
		{
			this.allyTimer--;
			if (this.allyTimer == 0)
			{
				this.allyCall--;
			}
		}
	}

	public void Horn_Next()
	{
		Msg.Say("defense_start");
		int add = 0;
		if (this.turns <= 3)
		{
			Msg.Say("defense_bonus", this.bonus.ToString() ?? "", null, null, null);
			this.bonus++;
			add = 1 + base.quest.difficulty;
		}
		this.NextWave(add);
		this.hornTimer += 10;
		if (this.hornTimer > 100)
		{
			this.hornTimer = 100;
		}
	}

	public void Horn_Retreat()
	{
		SE.Play("warhorn");
		Msg.Say("defense_retreat");
		this.retreated = true;
		this.instance.status = ZoneInstance.Status.Success;
		ActEffect.Proc(EffectId.Evac, EClass.pc, null, 100, default(ActRef));
	}

	public void Horn_Ally()
	{
		SE.Play("warhorn");
		ActEffect.ProcAt(EffectId.Summon, 100, BlessedState.Normal, EClass.pc, EClass.pc, EClass.pc.pos, false, new ActRef
		{
			n1 = "special_force"
		});
		this.allyCall++;
		this.allyTimer += 100;
	}

	public override void OnCharaDie(Chara c)
	{
		if (c.IsPCParty || c.IsPCPartyMinion)
		{
			return;
		}
		this.kills++;
	}

	[JsonProperty]
	public int wave;

	[JsonProperty]
	public int turns;

	[JsonProperty]
	public int hornTimer;

	[JsonProperty]
	public int kills;

	[JsonProperty]
	public int allyCall;

	[JsonProperty]
	public int allyTimer;

	[JsonProperty]
	public int bonus;

	[JsonProperty]
	public bool retreated;
}
