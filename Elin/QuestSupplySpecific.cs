using System;
using System.Collections.Generic;

public class QuestSupplySpecific : QuestSupply
{
	public virtual string idCat
	{
		get
		{
			return "meal";
		}
	}

	public virtual bool UseWeight
	{
		get
		{
			return true;
		}
	}

	public override string RewardSuffix
	{
		get
		{
			return "Supply";
		}
	}

	public override int GetExtraMoney()
	{
		return (int)((float)(base.sourceThing.value * this.num) * 0.1f) + 5 * this.num;
	}

	public override int GetBonus(Thing t)
	{
		int num = (int)((float)t.GetPrice(CurrencyType.Money, true, PriceType.Shipping, EClass.pc) * 1.2f) * this.num - this.rewardMoney;
		if (num <= 0)
		{
			return 0;
		}
		return num;
	}

	public override void SetIdThing()
	{
		List<SourceThing.Row> list = new List<SourceThing.Row>();
		int num = EClass.pc.Evalue(287) * 150 / 100 + 5;
		for (int i = 0; i < 2; i++)
		{
			foreach (SourceThing.Row row in EClass.sources.things.rows)
			{
				if ((i != 0 || !(this is QuestMeal) || row.LV <= num || EClass.rnd(4) == 0) && row.category == this.idCat && !row.isOrigin)
				{
					list.Add(row);
				}
			}
			if (list.Count != 0)
			{
				break;
			}
		}
		string id;
		if (!this.UseWeight)
		{
			id = list.RandomItem<SourceThing.Row>().id;
		}
		else
		{
			id = list.RandomItemWeighted((SourceThing.Row a) => (float)a.chance).id;
		}
		this.idThing = id;
		if (this.difficultyType == Quest.DifficultyType.Meal)
		{
			this.difficulty = base.sourceThing.LV / 10 + 1;
		}
	}
}
