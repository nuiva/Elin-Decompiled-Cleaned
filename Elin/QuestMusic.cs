using System;
using Newtonsoft.Json;

public class QuestMusic : QuestInstance
{
	public override Quest.DifficultyType difficultyType
	{
		get
		{
			return Quest.DifficultyType.Music;
		}
	}

	public override string IdZone
	{
		get
		{
			return "instance_music";
		}
	}

	public override string RewardSuffix
	{
		get
		{
			return "Music";
		}
	}

	public override string RefDrama2
	{
		get
		{
			return this.destScore.ToString() ?? "";
		}
	}

	public override int KarmaOnFail
	{
		get
		{
			return 0;
		}
	}

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventMusic();
	}

	public override string GetTextProgress()
	{
		return "progressMusic".lang(this.score.ToString() ?? "", this.destScore.ToString() ?? "", null, null, null);
	}

	public override int GetRewardPlat(int money)
	{
		return this.difficulty + EClass.rnd(2);
	}

	public override void OnInit()
	{
		this.destScore = this.difficulty * 150;
		this.destScore += EClass.rnd(this.destScore / 5);
	}

	[JsonProperty]
	public int score;

	[JsonProperty]
	public int destScore = 10;

	[JsonProperty]
	public int sumMoney;
}
