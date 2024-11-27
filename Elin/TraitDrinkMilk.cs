using System;

public class TraitDrinkMilk : TraitDrink
{
	public override EffectId IdEffect
	{
		get
		{
			return EffectId.DrinkMilk;
		}
	}

	public override int DefaultStock
	{
		get
		{
			return 3 + EClass.rnd(5);
		}
	}
}
