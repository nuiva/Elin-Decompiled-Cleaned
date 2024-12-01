using Newtonsoft.Json;

public class QuestHarvest : QuestInstance
{
	[JsonProperty]
	public int weightDelivered;

	[JsonProperty]
	public int destWeight;

	public override DifficultyType difficultyType => DifficultyType.Farm;

	public override string IdZone => "instance_harvest";

	public override string RewardSuffix => "Harvest";

	public override string RefDrama2 => Lang._weight(destWeight);

	public override bool FameContent => true;

	public override int KarmaOnFail => 0;

	public override int DangerLv => 3;

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventHarvest();
	}

	public override string GetTextProgress()
	{
		return "progressHarvest".lang(Lang._weight(weightDelivered), Lang._weight(destWeight));
	}

	public override void OnInit()
	{
		destWeight = difficulty * 30 * 1000;
		destWeight += EClass.rnd(destWeight / 5) / 100 * 100;
	}

	public override void OnBeforeComplete()
	{
		bonusMoney += weightDelivered * 4 / 1000;
	}
}
