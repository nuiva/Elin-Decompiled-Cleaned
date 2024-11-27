using System;

public class QuestShowSupply : QuestSupply
{
	public override bool ConsumeGoods
	{
		get
		{
			return false;
		}
	}
}
