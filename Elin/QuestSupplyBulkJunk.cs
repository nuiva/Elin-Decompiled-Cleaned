using System;
using UnityEngine;

public class QuestSupplyBulkJunk : QuestSupplyBulk
{
	public override string idCat
	{
		get
		{
			return "junk";
		}
	}

	public override int GetDestNum()
	{
		return Mathf.Max(1, this.difficulty / 2 + EClass.rnd(2));
	}
}
