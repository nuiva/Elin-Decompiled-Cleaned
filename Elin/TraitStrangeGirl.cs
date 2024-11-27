using System;

public class TraitStrangeGirl : TraitUniqueChara
{
	public override ShopType ShopType
	{
		get
		{
			if (!(EClass._zone is Zone_LittleGarden))
			{
				return ShopType.None;
			}
			return ShopType.StrangeGirl;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Influence;
		}
	}
}
