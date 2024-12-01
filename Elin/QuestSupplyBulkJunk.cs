using UnityEngine;

public class QuestSupplyBulkJunk : QuestSupplyBulk
{
	public override string idCat => "junk";

	public override int GetDestNum()
	{
		return Mathf.Max(1, difficulty / 2 + EClass.rnd(2));
	}
}
