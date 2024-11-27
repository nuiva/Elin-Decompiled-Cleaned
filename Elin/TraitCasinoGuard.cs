using System;

public class TraitCasinoGuard : TraitGuard
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Specific;
		}
	}
}
