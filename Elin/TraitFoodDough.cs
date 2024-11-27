using System;

public class TraitFoodDough : TraitFood
{
	public override int DefaultStock
	{
		get
		{
			return 2 + EClass.rnd(5);
		}
	}
}
