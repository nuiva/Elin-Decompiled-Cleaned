using System;

public class QuestSupply : QuestDeliver
{
	public override string TextExtra2
	{
		get
		{
			if (!(this is QuestSupplyCat))
			{
				return "";
			}
			return "deliverCategory".lang();
		}
	}

	public override bool ForbidTeleport
	{
		get
		{
			return false;
		}
	}

	public override bool IsDeliver
	{
		get
		{
			return false;
		}
	}

	public override string RewardSuffix
	{
		get
		{
			return "SupplyCost";
		}
	}

	public override Quest.DifficultyType difficultyType
	{
		get
		{
			return Quest.DifficultyType.Supply;
		}
	}

	public override int GetBonus(Thing t)
	{
		return (int)((float)t.GetPrice(CurrencyType.Money, true, PriceType.Shipping, EClass.pc) * 1.5f);
	}
}
