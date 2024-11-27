using System;

public class QuestMeal : QuestSupplySpecific
{
	public override bool UseWeight
	{
		get
		{
			return false;
		}
	}

	public override Quest.DifficultyType difficultyType
	{
		get
		{
			return Quest.DifficultyType.Meal;
		}
	}
}
