using System;

public class TraitFoodSeasoning : TraitFood
{
	public override int DefaultStock
	{
		get
		{
			return 2 + EClass.rnd(5);
		}
	}
}
