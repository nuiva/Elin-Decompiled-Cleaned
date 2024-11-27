using System;
using Newtonsoft.Json;

public class QuestHarvest : QuestInstance
{
	public override Quest.DifficultyType difficultyType
	{
		get
		{
			return Quest.DifficultyType.Farm;
		}
	}

	public override string IdZone
	{
		get
		{
			return "instance_harvest";
		}
	}

	public override string RewardSuffix
	{
		get
		{
			return "Harvest";
		}
	}

	public override string RefDrama2
	{
		get
		{
			return Lang._weight(this.destWeight, true, 0);
		}
	}

	public override bool FameContent
	{
		get
		{
			return true;
		}
	}

	public override int KarmaOnFail
	{
		get
		{
			return 0;
		}
	}

	public override int DangerLv
	{
		get
		{
			return 3;
		}
	}

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventHarvest();
	}

	public override string GetTextProgress()
	{
		return "progressHarvest".lang(Lang._weight(this.weightDelivered, true, 0), Lang._weight(this.destWeight, true, 0), null, null, null);
	}

	public override void OnInit()
	{
		this.destWeight = this.difficulty * 30 * 1000;
		this.destWeight += EClass.rnd(this.destWeight / 5) / 100 * 100;
	}

	public override void OnBeforeComplete()
	{
		this.bonusMoney += this.weightDelivered * 4 / 1000;
	}

	[JsonProperty]
	public int weightDelivered;

	[JsonProperty]
	public int destWeight;
}
