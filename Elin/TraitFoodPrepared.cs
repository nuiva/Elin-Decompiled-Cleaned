using System;

public class TraitFoodPrepared : TraitFood
{
	public override int DefaultStock
	{
		get
		{
			return 5 + EClass.rnd(5);
		}
	}
}
