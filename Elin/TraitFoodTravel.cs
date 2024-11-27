using System;

public class TraitFoodTravel : TraitFood
{
	public override int DefaultStock
	{
		get
		{
			return 5 + EClass.rnd(10);
		}
	}
}
