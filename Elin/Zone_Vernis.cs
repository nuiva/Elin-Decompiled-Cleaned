public class Zone_Vernis : Zone_Civilized
{
	public override bool ShouldRegenerate => false;

	public override bool RestrictBuild => false;

	public override string IDBaseLandFeat => "bfHill,bfCoal,bfRuin";

	public override bool isClaimable => EClass.game.quests.GetPhase<QuestVernis>() == 4;

	public override string GetNewZoneID(int level)
	{
		if (level <= -1)
		{
			return "vernis_mine";
		}
		return base.GetNewZoneID(level);
	}
}
