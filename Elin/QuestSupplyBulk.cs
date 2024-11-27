using System;

public class QuestSupplyBulk : QuestSupplySpecific
{
	public override string RefDrama4
	{
		get
		{
			return "_unit2".lang(this.num.ToString() ?? "", base.sourceThing.GetText("unit", false), null, null, null);
		}
	}

	public override int GetDestNum()
	{
		return 1 + this.difficulty * 3 + EClass.rnd(5);
	}

	public override Quest.DifficultyType difficultyType
	{
		get
		{
			return Quest.DifficultyType.Bulk;
		}
	}
}
