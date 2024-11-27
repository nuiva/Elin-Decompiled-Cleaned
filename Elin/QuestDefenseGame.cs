using System;
using Newtonsoft.Json;
using UnityEngine;

public class QuestDefenseGame : QuestInstance
{
	public override string IdZone
	{
		get
		{
			return "instance_arena";
		}
	}

	public override string RefDrama1
	{
		get
		{
			return this.thing.NameSimple;
		}
	}

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventDefenseGame();
	}

	public override string RewardSuffix
	{
		get
		{
			return "Defense";
		}
	}

	public override int FameOnComplete
	{
		get
		{
			return (QuestDefenseGame.lastWave * 8 + this.difficulty * 10) * (100 + QuestDefenseGame.bonus * 5) / 100;
		}
	}

	public override void OnInit()
	{
		this.thing = ThingGen.CreateFromFilter("thing", 30);
	}

	public override void OnBeforeComplete()
	{
		Debug.Log("QuestDefenseGame: " + QuestDefenseGame.lastWave.ToString() + "/" + QuestDefenseGame.bonus.ToString());
		this.bonusMoney += EClass.rndHalf(QuestDefenseGame.lastWave * 400 * (100 + QuestDefenseGame.bonus * 5) / 100);
	}

	public override string GetTextProgress()
	{
		return "progressDefenseGame".lang(QuestDefenseGame.lastWave.ToString() ?? "", null, null, null, null);
	}

	public static int lastWave;

	public static int bonus;

	[JsonProperty]
	public Thing thing;
}
