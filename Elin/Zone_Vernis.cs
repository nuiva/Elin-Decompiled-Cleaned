using System;

public class Zone_Vernis : Zone_Civilized
{
	public override bool ShouldRegenerate
	{
		get
		{
			return false;
		}
	}

	public override bool RestrictBuild
	{
		get
		{
			return false;
		}
	}

	public override string IDBaseLandFeat
	{
		get
		{
			return "bfHill,bfCoal,bfRuin";
		}
	}

	public override bool isClaimable
	{
		get
		{
			return EClass.game.quests.GetPhase<QuestVernis>() == 4;
		}
	}

	public override string GetNewZoneID(int level)
	{
		if (level <= -1)
		{
			return "vernis_mine";
		}
		return base.GetNewZoneID(level);
	}
}
